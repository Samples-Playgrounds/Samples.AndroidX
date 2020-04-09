using System;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Calendar.ContextualMenu
{
    public struct CalendarMenuAction : IEquatable<CalendarMenuAction>
    {
        public ContextualMenuType MenuType { get; }
        public CalendarMenuActionKind ActionKind { get; }
        public string Title { get; }
        public ViewAction MenuItemAction { get; }

        public CalendarMenuAction(ContextualMenuType menuType, CalendarMenuActionKind actionKind, string title, ViewAction menuItemAction)
        {
            Ensure.Argument.IsNotNull(menuType, nameof(menuType));
            Ensure.Argument.IsNotNull(actionKind, nameof(actionKind));
            Ensure.Argument.IsNotNull(title, nameof(title));
            Ensure.Argument.IsNotNull(menuItemAction, nameof(menuItemAction));
            
            MenuType = menuType;
            ActionKind = actionKind;
            Title = title;
            MenuItemAction = menuItemAction;
        }

        public bool Equals(CalendarMenuAction other)
        {
            return MenuType == other.MenuType 
                   && ActionKind == other.ActionKind 
                   && Title == other.Title;
        }

        public override bool Equals(object obj)
        {
            return obj is CalendarMenuAction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MenuType, ActionKind, Title, MenuItemAction);
        }
    }
}
