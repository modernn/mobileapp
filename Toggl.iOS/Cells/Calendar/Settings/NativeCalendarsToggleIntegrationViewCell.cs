using System;
using Foundation;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class NativeCalendarsToggleIntegrationViewCell : BaseTableViewCell<NativeCalendarsToggleIntegrationViewModel>
    {
        public static readonly string Identifier = nameof(NativeCalendarsToggleIntegrationViewCell);
        public static readonly NSString Key = new NSString(nameof(NativeCalendarsToggleIntegrationViewCell));
        public static readonly UINib Nib;

        public ViewAction ToggleNativeCalendarIntegration { get; set; }

        static NativeCalendarsToggleIntegrationViewCell()
        {
            Nib = UINib.FromName(nameof(NativeCalendarsToggleIntegrationViewCell), NSBundle.MainBundle);
        }

        protected NativeCalendarsToggleIntegrationViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            AllowAccessLabel.Text = Resources.AllowCalendarAccess;
            AllowAccessSwitch.Rx()
                .BindAction(() => ToggleNativeCalendarIntegration.Execute());
            ContentView.InsertSeparator();
        }


        protected override void UpdateView()
        {
            AllowAccessSwitch.SetState(Item.NativeCalendarIntegrationEnabled, true);
        }
    }
}
