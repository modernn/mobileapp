using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.Core.UI.ViewModels.MainLog.Identity;

namespace Toggl.WPF.Views
{
    public partial class TimeEntriesView
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public TimeEntriesView(TimeEntriesViewModel timeEntriesViewModel)
        {
            ViewModel = timeEntriesViewModel;
            InitializeComponent();

        }

        private void TimeEntriesView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.TimeEntries.Subscribe(UpdateTimeEntriesList)
                .DisposeWith(disposeBag);
        }

        private void UpdateTimeEntriesList(
            IImmutableList<AnimatableSectionModel<MainLogSectionViewModel, MainLogItemViewModel, IMainLogKey>> list)
        {

        }
    }
}

