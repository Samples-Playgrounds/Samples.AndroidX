using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using Toggl.Droid.Activities;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Toggl.Droid.Presentation
{
    public sealed class ActivityPresenter : AndroidPresenter
    {
        private const ActivityFlags clearBackStackFlags = ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask;

        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(AboutViewModel),
            typeof(CalendarSettingsViewModel),
            typeof(EditDurationViewModel),
            typeof(EditProjectViewModel),
            typeof(EditTimeEntryViewModel),
            typeof(ForgotPasswordViewModel),
            typeof(LoginViewModel),
            typeof(LicensesViewModel),
            typeof(MainTabBarViewModel),
            typeof(OutdatedAppViewModel),
            typeof(SelectClientViewModel),
            typeof(SelectCountryViewModel),
            typeof(SelectProjectViewModel),
            typeof(SelectTagsViewModel),
            typeof(SendFeedbackViewModel),
            typeof(SignupViewModel),
            typeof(StartTimeEntryViewModel),
            typeof(TokenResetViewModel),
            typeof(SettingsViewModel)
        };

        private readonly Dictionary<Type, ActivityPresenterInfo> presentableActivitiesInfos = new Dictionary<Type, ActivityPresenterInfo>
        {
            [typeof(AboutViewModel)] = new ActivityPresenterInfo(typeof(AboutActivity)),
            [typeof(CalendarSettingsViewModel)] = new ActivityPresenterInfo(typeof(CalendarSettingsActivity)),
            [typeof(EditDurationViewModel)] = new ActivityPresenterInfo(typeof(EditDurationActivity)),
            [typeof(EditProjectViewModel)] = new ActivityPresenterInfo(typeof(EditProjectActivity)),
            [typeof(EditTimeEntryViewModel)] = new ActivityPresenterInfo(typeof(EditTimeEntryActivity)),
            [typeof(ForgotPasswordViewModel)] = new ActivityPresenterInfo(typeof(ForgotPasswordActivity)),
            [typeof(LoginViewModel)] = new ActivityPresenterInfo(typeof(LoginActivity), clearBackStackFlags),
            [typeof(LicensesViewModel)] = new ActivityPresenterInfo(typeof(LicensesActivity)),
            [typeof(MainTabBarViewModel)] = new ActivityPresenterInfo(typeof(MainTabBarActivity), clearBackStackFlags),
            [typeof(OutdatedAppViewModel)] = new ActivityPresenterInfo(typeof(OutdatedAppActivity), clearBackStackFlags),
            [typeof(SelectClientViewModel)] = new ActivityPresenterInfo(typeof(SelectClientActivity)),
            [typeof(SelectCountryViewModel)] = new ActivityPresenterInfo(typeof(SelectCountryActivity)),
            [typeof(SelectProjectViewModel)] = new ActivityPresenterInfo(typeof(SelectProjectActivity)),
            [typeof(SelectTagsViewModel)] = new ActivityPresenterInfo(typeof(SelectTagsActivity)),
            [typeof(SendFeedbackViewModel)] = new ActivityPresenterInfo(typeof(SendFeedbackActivity)),
            [typeof(SettingsViewModel)] = new ActivityPresenterInfo(typeof(SettingsActivity)),
            [typeof(SignupViewModel)] = new ActivityPresenterInfo(typeof(SignUpActivity), clearBackStackFlags),
            [typeof(StartTimeEntryViewModel)] = new ActivityPresenterInfo(typeof(StartTimeEntryActivity)),
            [typeof(TokenResetViewModel)] = new ActivityPresenterInfo(typeof(TokenResetActivity), clearBackStackFlags)
        };

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            var viewModelType = viewModel.GetType();

            if (!presentableActivitiesInfos.TryGetValue(viewModelType, out var presentableInfo))
                throw new Exception($"Failed to start Activity for viewModel with type {viewModelType.Name}");

            var intent = new Intent(Application.Context, presentableInfo.ActivityType).AddFlags(presentableInfo.Flags);

            if (presentableInfo.Flags == clearBackStackFlags)
            {
                AndroidDependencyContainer.Instance.ViewModelCache.ClearAll();
            }

            AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .Cache(viewModel);

            getContextFromView(sourceView).StartActivity(intent);
        }

        private Context getContextFromView(IView view)
        {
            if (view is Activity activity)
                return activity;

            if (view is Fragment fragment)
                return fragment.Activity;

            return Application.Context;
        }

        private struct ActivityPresenterInfo
        {
            public Type ActivityType { get; }
            public ActivityFlags Flags { get; }

            public ActivityPresenterInfo(Type activityType, ActivityFlags flags = 0)
            {
                ActivityType = activityType;
                Flags = flags;
            }
        }
    }
}
