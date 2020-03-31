using System.Globalization;
using System.Reflection;
using System.Threading;
using Toggl.Core.UI.Helper;
using Xunit.Sdk;

namespace Toggl.Core.Tests.UI
{
    public class DiscardCultureChanges : BeforeAfterTestAttribute
    {
        private CultureInfo originalCultureInfo;
        private CultureInfo originalCultureInfoUI;
        private CultureInfo originalDateFormatCultureInfo; 
            
        public override void Before(MethodInfo methodUnderTest)
        {
            originalCultureInfo = Thread.CurrentThread.CurrentCulture;
            originalCultureInfoUI = Thread.CurrentThread.CurrentUICulture;
            originalDateFormatCultureInfo = DateFormatCultureInfo.CurrentCulture;
        }
            
        public override void After(MethodInfo methodUnderTest)
        {
            Thread.CurrentThread.CurrentCulture = originalCultureInfo;
            Thread.CurrentThread.CurrentUICulture = originalCultureInfoUI;
            DateFormatCultureInfo.CurrentCulture = originalDateFormatCultureInfo;
        }
    }
}