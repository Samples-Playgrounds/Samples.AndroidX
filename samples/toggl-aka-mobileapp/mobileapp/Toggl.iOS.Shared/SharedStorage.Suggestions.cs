using System.Collections.Generic;
using System.Linq;
using Foundation;
using Toggl.iOS.Shared.Models;
using Toggl.iOS.Shared.Extensions;

namespace Toggl.iOS.Shared
{
    public partial class SharedStorage
    {
        private const string tagsSeparator = ",";
        private const string suggestionWorkspaceId = "WorkspaceId";
        private const string suggestionDescription = "Description";
        private const string suggestionProjectId = "ProjectId";
        private const string suggestionProjectName = "ProjectName";
        private const string suggestionProjectColor = "ProjectColor";
        private const string suggestionTaskId = "TaskId";
        private const string suggesitonTaskName = "TaskName";
        private const string suggestionClientName = "ClientName";
        private const string suggestionIsBillable = "IsBillable";
        private const string suggestionTagIds = "TagIds";

        public void SetCurrentSuggestions(IList<Suggestion> suggestions)
        {
            var array = NSArray.FromObjects(suggestions.Select(toDictionary).ToArray());
            userDefaults[new NSString(suggestionsKey)] = array;
        }

        public IList<Suggestion> GetCurrentSuggestions()
        {
            var array = userDefaults.ArrayForKey(suggestionsKey);
            return array?.Cast<NSDictionary>().Select(fromDictionary).ToList();
        }

        private NSDictionary toDictionary(Suggestion suggestion)
        {
            var dict = new NSMutableDictionary();
            dict.SetLongForKey(suggestion.WorkspaceId, suggestionWorkspaceId);
            dict.SetStringForKey(suggestion.Description, suggestionDescription);

            if (suggestion.ProjectId.HasValue)
            {
                dict.SetLongForKey(suggestion.ProjectId.Value, suggestionProjectId);
                dict.SetStringForKey(suggestion.ProjectName, suggestionProjectName);
                dict.SetStringForKey(suggestion.ProjectColor, suggestionProjectColor);
                dict.SetStringForKey(suggestion.ClientName, suggestionClientName);
            }

            if (suggestion.TaskId.HasValue)
            {
                dict.SetLongForKey(suggestion.TaskId.Value, suggestionTaskId);
                dict.SetStringForKey(suggestion.TaskName, suggesitonTaskName);
            }

            var tagIds = string.Join(tagsSeparator, suggestion.TagIds.ToArray());
            dict.SetStringForKey(tagIds, suggestionTagIds);
            dict.SetBoolForKey(suggestion.IsBillable, suggestionIsBillable);

            return dict;
        }

        private Suggestion fromDictionary(NSDictionary dict)
        {
            if (dict == null)
                return null;

            var workspaceId = dict.GetLongForKey(suggestionWorkspaceId).Value;
            var description = dict.GetStringForKey(suggestionDescription);
            var projectId = dict.GetLongForKey(suggestionProjectId);
            var projectName = dict.GetStringForKey(suggestionProjectName);
            var projectColor = dict.GetStringForKey(suggestionProjectColor);
            var taskId = dict.GetLongForKey(suggestionTaskId);
            var taskName = dict.GetStringForKey(suggesitonTaskName);
            var clientName = dict.GetStringForKey(suggestionClientName);
            var isBillable = dict.GetBoolForKey(suggestionIsBillable).Value;

            var tagIdsString = dict.GetStringForKey(suggestionTagIds);
            var tagIds = string.IsNullOrEmpty(tagIdsString)
                ? new long[0]
                : tagIdsString.Split(tagsSeparator).Select(long.Parse).ToArray();

            return new Suggestion(
                workspaceId,
                description,
                projectId,
                projectName,
                projectColor,
                taskId,
                taskName,
                clientName,
                isBillable,
                tagIds);
        }
    }
}
