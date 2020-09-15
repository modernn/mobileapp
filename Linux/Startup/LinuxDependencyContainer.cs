using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NSubstitute;
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
using Toggl.Storage.Realm;
using Toggl.Storage.Settings;

namespace Linux
{
    public sealed class LinuxDependencyContainer : UIDependencyContainer
    {
        private const ApiEnvironment environment = ApiEnvironment.Staging;
        private static readonly UserAgent userAgent = new UserAgent("LinuxMobileApp", "1.0.0.0");
        private readonly CompositePresenter presenter;

        private readonly Lazy<RealmConfigurator> realmConfigurator = new Lazy<RealmConfigurator>(() => new RealmConfigurator());
        public new static LinuxDependencyContainer Instance { get; private set; }
        public static void EnsureInitialized(Toggl toggl)
        {
            Instance = new LinuxDependencyContainer(ApiEnvironment.Staging, userAgent, toggl);
            UIDependencyContainer.Instance = Instance;
        }

        private LinuxDependencyContainer(ApiEnvironment environment, UserAgent userAgent, Toggl toggl)
            : base(environment, userAgent)
        {
            presenter = new CompositePresenter(toggl);
        }

        protected override IAccessibilityService CreateAccessibilityService() => Substitute.For<IAccessibilityService>();

        protected override IAccessRestrictionStorage CreateAccessRestrictionStorage() => Substitute.For<IAccessRestrictionStorage>();

        protected override IAnalyticsService CreateAnalyticsService() => Substitute.For<IAnalyticsService>();

        protected override IBackgroundSyncService CreateBackgroundSyncService() => Substitute.For<IBackgroundSyncService>();

        protected override ICalendarService CreateCalendarService() => Substitute.For<ICalendarService>();

        protected override ITogglDatabase CreateDatabase() => new Database(realmConfigurator.Value);

        protected override IFetchRemoteConfigService CreateFetchRemoteConfigService() => Substitute.For<IFetchRemoteConfigService>();

        protected override HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            var header = new ProductHeaderValue(userAgent.Name, userAgent.Version);
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(header));
            return client;
        }

        protected override IKeyValueStorage CreateKeyValueStorage() => Substitute.For<IKeyValueStorage>();

        protected override ILastTimeUsageStorage CreateLastTimeUsageStorage() => new LinuxLastTimeUsageStorage();

        protected override ILicenseProvider CreateLicenseProvider() => Substitute.For<ILicenseProvider>();

        protected override INavigationService CreateNavigationService() => new NavigationService(presenter, ViewModelLoader, AnalyticsService);

        protected override INotificationService CreateNotificationService() => new LinuxNotificationService();

        protected override IOnboardingStorage CreateOnboardingStorage() => Substitute.For<IOnboardingStorage>();

        protected override IPermissionsChecker CreatePermissionsChecker() => new LinuxPermissionsChecker();

        protected override IPlatformInfo CreatePlatformInfo() => Substitute.For<IPlatformInfo>();

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService() => Substitute.For<IPrivateSharedStorageService>();

        protected override IPushNotificationsTokenService CreatePushNotificationsTokenService() => Substitute.For<IPushNotificationsTokenService>();

        protected override IQueryFactory CreateQueryFactory() => Substitute.For<IQueryFactory>();

        protected override IRatingService CreateRatingService() => Substitute.For<IRatingService>();

        protected override ISchedulerProvider CreateSchedulerProvider() => new LinuxSchedulerProvider();

        protected override IApplicationShortcutCreator CreateShortcutCreator() => Substitute.For<IApplicationShortcutCreator>();

        protected override IUserPreferences CreateUserPreferences() => Substitute.For<IUserPreferences>();

        protected override IWidgetsService CreateWidgetsService() => Substitute.For<IWidgetsService>();
    }
}
