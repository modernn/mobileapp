using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Networking;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Queries;
using Toggl.Storage.Settings;
using NSubstitute;
using Toggl.Storage.Realm;

namespace Toggl.WinUI
{
    public class WinUIDependencyContainer : UIDependencyContainer
    {
        private const ApiEnvironment environment =
#if USE_PRODUCTION_API
            ApiEnvironment.Production;
#else
            ApiEnvironment.Staging;
#endif
        private static readonly UserAgent userAgent = new UserAgent("MobileIntegrationTests", "1.0.0.0");

        private readonly Lazy<RealmConfigurator> realmConfigurator
            = new Lazy<RealmConfigurator>(() => new RealmConfigurator());

        public override IViewModelLoader ViewModelLoader => new ViewModelLoader(this);

        public static void Create()
        {
            Instance = new WinUIDependencyContainer(environment, userAgent);
        }

        protected WinUIDependencyContainer(ApiEnvironment environment, UserAgent userAgent) : base(environment, userAgent)
        {
        }

        protected override IAccessibilityService CreateAccessibilityService()
        {
            return Substitute.For<IAccessibilityService>();
        }

        protected override IAccessRestrictionStorage CreateAccessRestrictionStorage()
        {
            return Substitute.For<IAccessRestrictionStorage>();
        }

        protected override IAnalyticsService CreateAnalyticsService()
        {
            return Substitute.For<IAnalyticsService>();
        }

        protected override IBackgroundSyncService CreateBackgroundSyncService()
        {
            return Substitute.For<IBackgroundSyncService>();
        }

        protected override ICalendarService CreateCalendarService()
        {
            return Substitute.For<ICalendarService>();
        }

        protected override ITogglDatabase CreateDatabase()
        {
            return new Storage.Realm.Database(realmConfigurator.Value);
        }

        protected override IFetchRemoteConfigService CreateFetchRemoteConfigService()
        {
            return Substitute.For<IFetchRemoteConfigService>();
        }

        protected override HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            var header = new ProductHeaderValue(userAgent.Name, userAgent.Version);
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(header));
            return client;
        }

        protected override IKeyValueStorage CreateKeyValueStorage()
        {
            return Substitute.For<IKeyValueStorage>();
        }

        protected override ILastTimeUsageStorage CreateLastTimeUsageStorage()
        {
            return Substitute.For<ILastTimeUsageStorage>();
        }

        protected override ILicenseProvider CreateLicenseProvider()
        {
            return Substitute.For<ILicenseProvider>();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Substitute.For<INavigationService>();
        }

        protected override INotificationService CreateNotificationService()
        {
            return Substitute.For<INotificationService>();
        }

        protected override IOnboardingStorage CreateOnboardingStorage()
        {
            return Substitute.For<IOnboardingStorage>();
        }

        protected override IPermissionsChecker CreatePermissionsChecker()
        {
            return Substitute.For<IPermissionsChecker>();
        }

        protected override IPlatformInfo CreatePlatformInfo()
        {
            return Substitute.For<IPlatformInfo>();
        }

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService()
        {
            return Substitute.For<IPrivateSharedStorageService>();
        }

        protected override IPushNotificationsTokenService CreatePushNotificationsTokenService()
        {
            return Substitute.For<IPushNotificationsTokenService>();
        }

        protected override IQueryFactory CreateQueryFactory()
        {
            return Substitute.For<IQueryFactory>();
        }

        protected override IRatingService CreateRatingService()
        {
            return Substitute.For<IRatingService>();
        }

        protected override ISchedulerProvider CreateSchedulerProvider()
        {
            return Substitute.For<ISchedulerProvider>();
        }

        protected override IApplicationShortcutCreator CreateShortcutCreator()
        {
            return Substitute.For<IApplicationShortcutCreator>();
        }

        protected override IUserPreferences CreateUserPreferences()
        {
            return Substitute.For<IUserPreferences>();
        }

        protected override IWidgetsService CreateWidgetsService()
        {
            return Substitute.For<IWidgetsService>();
        }
    }
}
