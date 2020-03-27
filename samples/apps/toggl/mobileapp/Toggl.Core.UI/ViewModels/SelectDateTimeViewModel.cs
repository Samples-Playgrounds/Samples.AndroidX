using System;
using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDateTimeViewModel : ViewModel<DateTimePickerParameters, DateTimeOffset>
    {
        private DateTimeOffset defaultResult;

        public DateTimeOffset MinDate { get; private set; }
        public DateTimeOffset MaxDate { get; private set; }
        public DateTimePickerMode Mode { get; private set; }
        public bool Is24HoursFormat { get; private set; } = true;
        public BehaviorRelay<DateTimeOffset> CurrentDateTime { get; private set; }

        public ViewAction Save { get; }

        public SelectDateTimeViewModel(IRxActionFactory rxActionFactory, INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            Save = rxActionFactory.FromAction(save);
        }

        public override Task Initialize(DateTimePickerParameters dateTimePicker)
        {
            defaultResult = dateTimePicker.CurrentDate;

            Mode = dateTimePicker.Mode;
            MinDate = dateTimePicker.MinDate;
            MaxDate = dateTimePicker.MaxDate;
            CurrentDateTime = new BehaviorRelay<DateTimeOffset>(dateTimePicker.CurrentDate, sanitizeBasedOnMode);

            return base.Initialize(dateTimePicker);
        }

        public override Task<bool> CloseWithDefaultResult()
        {
            Close(defaultResult);
            return Task.FromResult(true);
        }

        private DateTimeOffset sanitizeBasedOnMode(DateTimeOffset dateTime)
        {
            var result = DateTimeOffset.MinValue;

            switch (Mode)
            {
                case DateTimePickerMode.Date:
                    result = defaultResult.ToUniversalTime().WithDate(dateTime.ToUniversalTime());
                    break;

                case DateTimePickerMode.Time:
                    result = defaultResult.ToUniversalTime().WithTime(dateTime.ToUniversalTime());
                    break;

                case DateTimePickerMode.DateTime:
                    result = dateTime;
                    break;

                default:
                    throw new NotSupportedException("Invalid DateTimePicker mode");
            }

            return result.Clamp(MinDate, MaxDate);
        }

        private void save()
        {
            Close(CurrentDateTime.Value);
        }
    }
}
