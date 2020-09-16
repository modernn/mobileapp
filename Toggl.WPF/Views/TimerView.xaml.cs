using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Extensions;
using Toggl.WPF.Extensions;
using Toggl.WPF.Extensions.Reactive;

namespace Toggl.WPF.Views
{
    public partial class TimerView : ReactiveUserControl<TimerViewModel>
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public TimerView()
        {
            InitializeComponent();
        }

        public void Bind(TimerViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.Initialize().GetAwaiter().GetResult();

            ViewModel.ElapsedTime
                .DistinctUntilChanged()
                .Subscribe(Duration.Rx().TextObserver())
                .DisposedBy(disposeBag);

            ViewModel.CurrentRunningTimeEntry
                .Select(te => te?.Description ?? "")
                .DistinctUntilChanged()
                .Subscribe(Description.Rx().TextObserver())
                .DisposedBy(disposeBag);

            ViewModel.CurrentRunningTimeEntry
                .Select(te => te?.Project)
                .Subscribe(project =>
                {
                    if (project == null)
                    {
                        Project.Text = "";
                    }
                    else
                    {
                        var projectColor = Shared.Color.ParseAndAdjustToLabel(project.Color, isInDarkMode: false);
                        Project.Text = project.Name;
                        Project.Foreground = new SolidColorBrush(projectColor.ToNativeColor());
                    }
                }).DisposedBy(disposeBag);

            ViewModel.CurrentRunningTimeEntry
                .Select(te => te != null)
                .Subscribe(StopButton.Rx().IsVisible())
                .DisposedBy(disposeBag);

            ViewModel.CurrentRunningTimeEntry
                .Select(te => te == null)
                .Subscribe(StartButton.Rx().IsVisible())
                .DisposedBy(disposeBag);

            StartButton.Command = ViewModel.StartTimeEntry.ToCommand();
            StopButton.Command = ViewModel.StopTimeEntry.ToCommand();
            StopButton.CommandParameter = TimeEntryStopOrigin.Manual;
        }
    }
}
