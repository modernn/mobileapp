using Foundation;
using System;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class SelectableExternalCalendarViewCell : BaseTableViewCell<SelectableExternalCalendarViewModel>
    {
        public static readonly string Identifier = nameof(SelectableExternalCalendarViewCell);
        public static readonly NSString Key = new NSString(nameof(SelectableExternalCalendarViewCell));
        public static readonly UINib Nib;

        public InputAction<SelectableExternalCalendarViewModel> SelectCalendar { get; set; }

        static SelectableExternalCalendarViewCell()
        {
            Nib = UINib.FromName(nameof(SelectableExternalCalendarViewCell), NSBundle.MainBundle);
        }

        protected SelectableExternalCalendarViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            IsSelectedSwitch.Rx()
                .BindAction(() => SelectCalendar?.Execute(Item));

            FadeView.FadeRight = true;
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            CalendarNameLabel.Text = Item.Name;
            IsSelectedSwitch.SetState(Item.Selected, animated: false);
        }
    }
}
