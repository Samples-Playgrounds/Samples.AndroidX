using System;
using Toggl.Shared.Extensions;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core.UI.Extensions
{
    public static class StringExtensions
    {
        public static bool IsSameCaseInsensitiveTrimedTextAs(this string self, string tagText)
            => self.Trim().Equals(tagText.Trim(), StringComparison.CurrentCultureIgnoreCase);

        public static bool IsAllowedTagByteSize(this string self)
            => self.LengthInBytes() <= MaxTagNameLengthInBytes;
    }
}
