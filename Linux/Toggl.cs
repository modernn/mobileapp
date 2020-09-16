using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Running;
using Qml.Net;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Linux
{
    public class Toggl : IPresenter
    {
        private string _status;
        private string _error;
        private List<string> _tabs;
        private QMLTimeEntry _runningTimeEntry;
        private TimeEntriesViewModel _timeEntries;
        private List<QMLTimeEntry> _timeEntries2;
        [NotifySignal]
        public string Status {
            get {
                return _status;
            }
            set {
                _status = value;
                _ = this.ActivateSignal("statusChanged");
            }
        }
        [NotifySignal]
        public string Error {
            get {
                return _error;
            }
            set {
                _error = value;
                _ = this.ActivateSignal("errorChanged");
            }
        }
        [NotifySignal]
        public List<string> Tabs
        {
            get
            {
                return _tabs;
            }
            set
            {
                _tabs = value;
                _ = this.ActivateSignal("tabsChanged");
            }
        }
        [NotifySignal]
        public QMLTimeEntry RunningTimeEntry
        {
            get
            {
                return _runningTimeEntry;
            }
            set
            {
                _runningTimeEntry = value;
                _ = this.ActivateSignal("runningTimeEntryChanged");
            }
        }
        [NotifySignal]
        public TimeEntriesViewModel TimeEntries
        {
            get
            {
                return _timeEntries;
            }
            set
            {
                _timeEntries = value;
                _ = this.ActivateSignal("timeEntriesChanged");
            }
        }
        [NotifySignal]
        public List<QMLTimeEntry> TimeEntries2
        {
            get
            {
                return _timeEntries2;
            }
            set
            {
                _timeEntries2 = value;
                _ = this.ActivateSignal("timeEntries2Changed");
            }
        }
        public Toggl()
        {
            QmlNetConfig.ShouldEnsureUIThread = false; // TODO this could break stuff

            Status = "Just constructed";
        }
        public void Bootup()
        {
            Status = "Booting up";
            var app = new AppStart(LinuxDependencyContainer.Instance);
            app.LoadLocalizationConfiguration();
            var accessLevel = app.GetAccessLevel();
            app.SetupBackgroundSync();
            app.SetFirstOpened();

            if (accessLevel == AccessLevel.LoggedIn)
                app.ForceFullSync();

            var navigationService = LinuxDependencyContainer.Instance.NavigationService;

            switch (accessLevel)
            {
                case AccessLevel.AccessRestricted:
                    break;
                case AccessLevel.NotLoggedIn:
                    navigationService.Navigate<LoginViewModel, CredentialsParameter, Unit>(CredentialsParameter.Empty, null);
                    break;
                case AccessLevel.TokenRevoked:
                    break;
                case AccessLevel.LoggedIn:
                    navigationService.Navigate<MainTabBarViewModel, MainTabBarParameters, Unit>(MainTabBarParameters.Default, null);
                    break;
            }
        }

        private LoginViewModel loginView;
        public void Login(string username, string password)
        {
            if (loginView != null)
            {
                Error = null;
                loginView.PasswordErrorMessage.Subscribe(errorMessage => { if (errorMessage.Length > 0) Error = errorMessage; });
                loginView.EmailErrorMessage.Subscribe(errorMessage => { if (errorMessage.Length > 0) Error = errorMessage; });
                loginView.LoginErrorMessage.Subscribe(errorMessage => { if (errorMessage.Length > 0) Error = errorMessage; });
                Console.Error.Write(Error);
                loginView.Email.Accept(Email.From(username));
                loginView.Password.Accept(Password.From(password));
                loginView.Login.Execute();
            }
        }
        public void Logout()
        {
        }
        public void Sync()
        {
            LinuxDependencyContainer
                .Instance
                .SyncManager
                .ForceFullSync()
                .Subscribe();
        }
        private MainTabBarViewModel mainTabBarView;

        public bool CanPresent<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
        {
            return true;
        }

        public async Task Present<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            switch(viewModel)
            {
                case LoginViewModel loginViewModel:
                    Status = "LOGIN VIEW";
                    loginView = loginViewModel;
                    mainTabBarView = null;
                    break;
                case MainTabBarViewModel mainTabBarViewModel:
                    Status = "MAIN TAB BAR VIEW";
                    loginView = null;
                    mainTabBarView = mainTabBarViewModel;
                    var tabNames = new List<string>();
                    foreach (var i in mainTabBarView.Tabs)
                    {
                        var val = i.Value;
                        lock (val) ;
                        if (val as MainViewModel != null)
                        {
                            tabNames.Add(typeof(MainViewModel).Name);
                            var vm = i.Value as MainViewModel;
                            if (vm.TimeEntriesViewModel == null)
                            {
                                Console.Error.WriteLine("NOOO");
                            }
                            else
                            {
                                Console.Error.WriteLine(".............");
                                TimeEntries = vm.TimeEntriesViewModel;
                            }
                        }
                        else if (val as ReportsViewModel != null)
                            tabNames.Add(typeof(ReportsViewModel).Name);
                        else if (val as CalendarViewModel != null)
                            tabNames.Add(typeof(CalendarViewModel).Name);
                        else
                            tabNames.Add("N/A");
                    }
                    LinuxDependencyContainer.Instance.DataSource.TimeEntries.CurrentlyRunningTimeEntry.Subscribe(te =>
                    {
                        if (te != null)
                        {
                            RunningTimeEntry = new QMLTimeEntry(te);
                        }
                        else
                        {
                            RunningTimeEntry = null;
                        }

                    });
                    LinuxDependencyContainer.Instance.DataSource.TimeEntries.GetAll().Subscribe(tes => {
                        var newTes = new List<QMLTimeEntry>();
                        foreach (var i in tes)
                        {
                            if (i.Duration == null || i.Duration < 0)
                            {
                            }
                            else
                            {
                                newTes.Add(new QMLTimeEntry(i));
                            }
                            Console.Error.WriteLine(i.Description);
                        }
                        TimeEntries2 = newTes;
                    });
                    Tabs = tabNames;
                    break;
                case OutdatedAppViewModel outdated:
                    Status = "OUTDATED";
                    loginView = null;
                    mainTabBarView = null;
                    break;
                default:
                    Status = "Something else";
                    loginView = null;
                    break;
            }
        }

        public bool ChangePresentation(IPresentationChange presentationChange)
        {
            return true;
        }
    }
}
