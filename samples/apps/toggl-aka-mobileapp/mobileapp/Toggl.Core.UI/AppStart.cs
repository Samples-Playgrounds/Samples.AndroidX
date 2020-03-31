using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Toggl.Core.UI.Helper;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core.UI
{
    public sealed class AppStart
    {
        private const char dotNetLanguageCodeSeparator = '-';
        private const char nativeLanguageCodeSeparator = '_';

        private readonly UIDependencyContainer dependencyContainer;

        public AppStart(UIDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public void LoadLocalizationConfiguration()
        {
            var platformInfo = dependencyContainer.PlatformInfo;

            var currentLanguageCode = platformInfo.CurrentNativeLanguageCode ?? DefaultLanguageCode;
            var nonEmptyCurrentLanguageCode = string.IsNullOrEmpty(currentLanguageCode) ? DefaultLanguageCode : currentLanguageCode;
            var dotNetLanguageCode = convertNativeLanguageCodeToDotNetStandards(nonEmptyCurrentLanguageCode);

            CultureInfo cultureInfo = null;
            CultureInfo dateFormatCultureInfo = null;
            try
            {
                if (CultureInfo.GetCultures(CultureTypes.AllCultures).Any(info => info.Name == dotNetLanguageCode))
                {
                    cultureInfo = new CultureInfo(dotNetLanguageCode);
                }
                else
                {
                    var twoLettersLanguageCode = getTwoLettersLanguageCode(dotNetLanguageCode);
                    if (CultureInfo.GetCultures(CultureTypes.NeutralCultures).Any(info => info.TwoLetterISOLanguageName == twoLettersLanguageCode))
                        cultureInfo = new CultureInfo(twoLettersLanguageCode);
                }

                if (SupportedTwoLettersLanguageCodes.Contains(cultureInfo.TwoLetterISOLanguageName, StringComparer.InvariantCultureIgnoreCase))
                {
                    dateFormatCultureInfo = cultureInfo;
                }
            }
            catch (Exception)
            {
                cultureInfo = new CultureInfo(DefaultLanguageCode);
            }
            finally
            {
                setLocale(cultureInfo ?? new CultureInfo(DefaultLanguageCode), dateFormatCultureInfo ?? new CultureInfo(DefaultLanguageCode));
            }
        }

        public void SetupBackgroundSync()
        {
            var backgroundService = dependencyContainer.BackgroundSyncService;
            backgroundService.SetupBackgroundSync(dependencyContainer.UserAccessManager);
        }

        public void SetFirstOpened()
        {
            var timeService = dependencyContainer.TimeService;
            var onboardingStorage = dependencyContainer.OnboardingStorage;
            onboardingStorage.SetFirstOpened(timeService.CurrentDateTime);
        }

        public AccessLevel GetAccessLevel()
        {
            var accessRestrictionStorage = dependencyContainer.AccessRestrictionStorage;

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
                return AccessLevel.AccessRestricted;

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
                return AccessLevel.NotLoggedIn;

            var apiToken = dependencyContainer.UserAccessManager.GetSavedApiToken();
            if (accessRestrictionStorage.IsUnauthorized(apiToken))
                return AccessLevel.TokenRevoked;

            return AccessLevel.LoggedIn;
        }

        public void UpdateOnboardingProgress()
        {
            const int newUserThreshold = 60;
            var now = dependencyContainer.TimeService.CurrentDateTime;
            var lastUsed = dependencyContainer.OnboardingStorage.GetLastOpened();
            dependencyContainer.OnboardingStorage.SetLastOpened(now);
            if (!lastUsed.HasValue)
                return;

            var offset = now - lastUsed;
            if (offset < TimeSpan.FromDays(newUserThreshold))
                return;

            dependencyContainer.OnboardingStorage.SetIsNewUser(false);
        }

        public void ForceFullSync()
        {
            dependencyContainer.SyncManager.ForceFullSync().Subscribe();
        }

        private string getTwoLettersLanguageCode(string dotNetLanguageCode)
            => dotNetLanguageCode.Split(dotNetLanguageCodeSeparator)[0];

        private string convertNativeLanguageCodeToDotNetStandards(string currentLanguageCode)
            => currentLanguageCode.Replace(nativeLanguageCodeSeparator, dotNetLanguageCodeSeparator);

        private void setLocale(CultureInfo cultureInfo, CultureInfo dateFormatCultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            DateFormatCultureInfo.CurrentCulture = dateFormatCultureInfo;
        }
    }
}
