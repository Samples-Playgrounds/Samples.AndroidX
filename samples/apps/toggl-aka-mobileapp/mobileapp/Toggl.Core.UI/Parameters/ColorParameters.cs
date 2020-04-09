using Toggl.Shared;

namespace Toggl.Core.UI.Parameters
{
    public sealed class ColorParameters
    {
        public Color Color { get; set; }

        public bool AllowCustomColors { get; set; }

        public static ColorParameters Create(Color color, bool allowCustomColor) => new ColorParameters
        {
            Color = color,
            AllowCustomColors = allowCustomColor
        };
    }
}
