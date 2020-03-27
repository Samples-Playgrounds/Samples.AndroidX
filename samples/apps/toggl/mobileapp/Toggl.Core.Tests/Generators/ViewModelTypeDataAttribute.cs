using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Calendar.ContextualMenu;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared.Extensions;
using Xunit.Sdk;

namespace Toggl.Core.Tests.Generators
{
    public sealed class ViewModelTypeDataAttribute : DataAttribute
    {
        private readonly HashSet<Type> exceptionViewModels = new HashSet<Type>()
        {
            typeof(ReportsBarChartViewModel),
            typeof(CalendarDayViewModel),
            typeof(CalendarContextualMenuViewModel)
        };

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            => typeof(MainViewModel).Assembly
                .GetTypes()
                .Where(isAValidViewModel)
                .Select(viewModelType => new object[] { viewModelType });

        private bool isAValidViewModel(Type type)
            => type.IsAbstract == false &&
               type.Name != nameof(IViewModel) &&
               type.ImplementsOrDerivesFrom<IViewModel>() &&
               !exceptionViewModels.Contains(type);
    }
}
