#if !USE_PRODUCTION_API
using System;
using Toggl.Core.UI.Extensions;
using Toggl.Droid.Debug;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    public partial class SettingsActivity
    {
        protected override void OnResume()
        {
            base.OnResume();

            aboutView.Rx().LongPress()
                .Subscribe(showErrorTriggeringView)
                .DisposedBy(DisposeBag);
        }

        private void showErrorTriggeringView()
        {
            new ErrorTriggeringFragment()
                .Show(SupportFragmentManager, nameof(ErrorTriggeringFragment));
        }
    }
}
#endif
