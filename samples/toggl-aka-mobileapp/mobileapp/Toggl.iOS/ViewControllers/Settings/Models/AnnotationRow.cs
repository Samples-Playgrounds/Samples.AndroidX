using System;
using System.Threading.Tasks;
using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class AnnotationRow : ISettingRow
    {
        public string Title { get; }
        public ViewAction Action { get; }

        public AnnotationRow(string text)
        {
            Title = text;
        }
    }
}
