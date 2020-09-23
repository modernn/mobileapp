using System;

using Foundation;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Cells.Calendar.Settings
{
    public sealed partial class NativeCalendarsMainHeaderViewCell : BaseTableHeaderFooterView<NativeCalendarsMainSectionViewModel>
    {
        public static readonly string Identifier = nameof(NativeCalendarsMainHeaderViewCell);
        public static readonly NSString Key = new NSString(nameof(NativeCalendarsMainHeaderViewCell));
        public static readonly UINib Nib;

        static NativeCalendarsMainHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(NativeCalendarsMainHeaderViewCell), NSBundle.MainBundle);
        }

        protected NativeCalendarsMainHeaderViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            TitleLabel.Text = Resources.NativeCalendarsTitle;
            SubitleLabel.Text = Resources.NativeCalendarsSubtitle;
        }
    }
}
