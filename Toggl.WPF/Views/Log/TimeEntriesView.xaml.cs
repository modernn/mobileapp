using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.Core.UI.ViewModels.MainLog.Identity;

namespace Toggl.WPF.Views.Log
{
    public partial class TimeEntriesView
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public TimeEntriesView()
        {
            InitializeComponent();

        }

        public void Bind(TimeEntriesViewModel timeEntriesViewModel)
        {
            ViewModel = timeEntriesViewModel;
            ViewModel.TimeEntries.Subscribe(UpdateTimeEntriesList)
                .DisposeWith(disposeBag);
        }

        private void UpdateTimeEntriesList(
            IImmutableList<AnimatableSectionModel<MainLogSectionViewModel, MainLogItemViewModel, IMainLogKey>> list)
        {
            this.TimeEntriesPanel.Children.Clear();
            foreach (var item in list)
            {
                this.TimeEntriesPanel.Children.Add(new DayHeader((DaySummaryViewModel)item.Header, item.Items));
            }
        }
    }
}

