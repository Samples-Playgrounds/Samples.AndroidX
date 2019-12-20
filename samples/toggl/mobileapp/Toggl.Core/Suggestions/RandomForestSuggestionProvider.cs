using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Accord.MachineLearning.DecisionTrees;
using Toggl.Core.DataSources;
using Toggl.Shared;
using Toggl.Storage.Models;
using Toggl.Storage;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Suggestions
{
    public sealed class RandomForestSuggestionProvider : ISuggestionProvider
    {
        private readonly ITogglDataSource dataSource;
        private readonly ITimeService timeService;

        private readonly int maxNumberOfTimeEntriesForTraining = 1000;
        private readonly int minNumberOfTimeEntriesForTraining = 100;
        private readonly int forestNumberOfTrees = 5;
        private readonly int forestJoin = 100;

        public RandomForestSuggestionProvider(
            ITogglDataSource dataSource,
            ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dataSource = dataSource;
            this.timeService = timeService;
        }

        public IObservable<Suggestion> GetSuggestions()
            => dataSource.TimeEntries
                .GetAll()
                .Select(timeEntries => timeEntries.Where(te => te.SyncStatus == SyncStatus.InSync && isTimeEntryActive(te)))
                .Select(predictUsingRandomForestClassifier)
                .SelectMany(toSuggestions)
                .OnErrorResumeEmpty();

        private IEnumerable<Suggestion> toSuggestions(IEnumerable<IDatabaseTimeEntry> timeEntries)
            => timeEntries.Select(timeEntry => new Suggestion(timeEntry, SuggestionProviderType.RandomForest));

        private IEnumerable<IDatabaseTimeEntry> predictUsingRandomForestClassifier(IEnumerable<IDatabaseTimeEntry> timeEntriesForPrediction)
        {
            var timeEntries = timeEntriesForPrediction
                .Where(te => te.ProjectId.HasValue && !string.IsNullOrWhiteSpace(te.Project.Name))
                .Take(maxNumberOfTimeEntriesForTraining)
                .ToList();

            if (timeEntries.Count >= minNumberOfTimeEntriesForTraining)
                return predictUsing2Steps(timeEntries);

            timeEntries = timeEntriesForPrediction
                .Where(te => !string.IsNullOrWhiteSpace(te.Description) || (te.ProjectId.HasValue && !string.IsNullOrWhiteSpace(te.Project.Name)))
                .Take(maxNumberOfTimeEntriesForTraining)
                .ToList();

            if (timeEntries.Count >= minNumberOfTimeEntriesForTraining)
                return predictUsing1Step(timeEntries);

            return new List<IDatabaseTimeEntry>();
        }

        private IEnumerable<IDatabaseTimeEntry> predictUsing2Steps(List<IDatabaseTimeEntry> timeEntries)
        {
            var predictedProject = predictProjectID(timeEntries); //Step 1 Predict Projects
        
            var timeEntriesFromProject = timeEntries.Where(te => te.ProjectId == predictedProject).ToList();

            var predictedTimeEntry = predictTimeEntryID(timeEntriesFromProject); //Step 2 Predict the TimeEntry
         
            return new[] { timeEntries.FirstOrDefault(te => te.Id == predictedTimeEntry) };
        }

        private IEnumerable<IDatabaseTimeEntry> predictUsing1Step(List<IDatabaseTimeEntry> timeEntries)
        {
            var predictedTimeEntry = predictTimeEntryID(timeEntries);
          
            return new[] { timeEntries.FirstOrDefault(te => te.Id == predictedTimeEntry) };
        }

        private double[] toFeatures(DateTimeOffset dateTimeOffset)
        {
            var start = dateTimeOffset;
            var dayOfWeek = (int)start.DayOfWeek;
            var startHour = TimeZoneInfo.ConvertTime(start, TimeZoneInfo.Local).Hour;

            var dayOfWeekNormalized = (double)dayOfWeek / 6;
            var isWeekend = dayOfWeek == 0 || dayOfWeek == 6 ? 1 : 0;
            var isBusinessHours = startHour > 8 && startHour < 18 ? 1 : 0;
            var startHourNormalized = (double)startHour / 23;

            var features = new[]
            {
                dayOfWeekNormalized,
                isWeekend,
                isBusinessHours,
                startHourNormalized
            };

            return features;
        }

        private double[][] featureRemovingUnchangingValues(double[][] input)
        {
            var features = input;
            if (features.Length == 0)
                return features;

            if (features[0].Length == 0)
                return features;

            int rowLength = features.Length;
            int colLength = features[0].Length;

            bool[] indexesWithSameValues = Enumerable.Repeat(true, colLength).ToArray();

            double[] firstRow = features[0];

            for (int i = 1; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    if (System.Math.Abs(features[i][j] - firstRow[j]) > 0)
                    {
                        indexesWithSameValues[j] = false;
                    }
                }
            }

            for (int i = 0; i < rowLength; i++)
            {
                var newRow = new List<double>();
                for (int j = 0; j < colLength; j++)
                {
                    if (!indexesWithSameValues[j])
                    {
                        newRow.Add(features[i][j]);
                    }
                }
                features[i] = newRow.ToArray();
            }

            return features;
        }

        private int predict(double[][] trainingInputs, int[] trainingOutputs, double[][] inputToPredict)
        {
            var teacher = new RandomForestLearning()
            {
                NumberOfTrees = forestNumberOfTrees,
                Join = forestJoin
            };

            var forest = teacher.Learn(trainingInputs, trainingOutputs);

            int[] predicted = forest.Decide(inputToPredict);

            return predicted[0];
        }

        private double[][] prepareInputsFor(IList<IDatabaseTimeEntry> timeEntries)
            => timeEntries.Select(te => toFeatures(te.Start)).ToArray();

        private int[] prepareProjectOutputsFor(IList<IDatabaseTimeEntry> timeEntries)
            => timeEntries.Select(te => (int)te.ProjectId.GetValueOrDefault()).ToArray();

        private int[] prepareTimeEntryOutputsFor(IList<IDatabaseTimeEntry> timeEntries)
            => timeEntries.Select(te => (int)te.Id).ToArray();

        // This method does to 2 things
        // 1. Returns a list of all unique values of the `outputs` parameter
        // 2. Changes the `outputs` parameter values to be the same as the index of that value in the returned list.
        // This is needed to reduce complexity of the data for the random forest algorithm so that it runs faster.
        // Instead of using long values of the IDs it will use smaller int basd on the index.
        private int[] uniqueOutputsFor(int[] outputs)
        {
            int[] uniqueOutputs = outputs.Distinct().ToArray();

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = Array.IndexOf(uniqueOutputs, outputs[i]);
            }

            return uniqueOutputs;
        }

        private int predictProjectID(IEnumerable<IDatabaseTimeEntry> timeEntriesForPrediction)
        {
            var timeEntries = timeEntriesForPrediction.ToList();

            double[][] inputs = featureRemovingUnchangingValues(prepareInputsFor(timeEntries));
            int[] outputs = prepareProjectOutputsFor(timeEntries);
            int[] uniqueOutputs = uniqueOutputsFor(outputs); // This also updates the outputs to have values same as the index in the uniqueOutputs

            var inputToPredict = new double[][] { toFeatures(timeService.CurrentDateTime) };
            var prediction = predict(inputs, outputs, inputToPredict);

            return uniqueOutputs[prediction];
        }

        private int predictTimeEntryID(IEnumerable<IDatabaseTimeEntry> timeEntriesForPrediction)
        {
            var timeEntries = timeEntriesForPrediction.ToList();

            double[][] inputs = featureRemovingUnchangingValues(prepareInputsFor(timeEntries));
            int[] outputs = prepareTimeEntryOutputsFor(timeEntries);
            int[] uniqueOutputs = uniqueOutputsFor(outputs); // This also updates the outputs to have values same as the index in the uniqueOutputs

            var inputToPredict = new double[][] { toFeatures(timeService.CurrentDateTime) };
            var prediction = predict(inputs, outputs, inputToPredict);

            return uniqueOutputs[prediction];
        }

        private bool isTimeEntryActive(IDatabaseTimeEntry timeEntry)
            => timeEntry.IsDeleted == false
               && timeEntry.IsInaccessible == false
               && (timeEntry.Project?.Active ?? true);
    }
}
