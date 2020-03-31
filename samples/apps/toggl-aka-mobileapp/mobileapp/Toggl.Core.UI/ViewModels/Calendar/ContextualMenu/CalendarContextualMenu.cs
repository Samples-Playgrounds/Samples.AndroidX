using System.Collections.Immutable;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Calendar.ContextualMenu
{
    public struct CalendarContextualMenu
    {
        public ContextualMenuType Type { get; }
        public ViewAction Dismiss { get; }
        public ImmutableList<CalendarMenuAction> Actions { get; }

        public CalendarContextualMenu(ContextualMenuType type, ImmutableList<CalendarMenuAction> actions, ViewAction dismissAction)
        {
            Type = type;
            Actions = actions;
            Dismiss = dismissAction;
        }
    }
}