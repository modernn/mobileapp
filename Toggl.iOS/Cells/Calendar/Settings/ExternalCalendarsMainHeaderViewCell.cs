using System;

using Foundation;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Cells.Calendar.Settings
{
    public sealed partial class ExternalCalendarsMainHeaderViewCell : BaseTableHeaderFooterView<ExternalCalendarsMainSectionViewModel>
    {
        public static readonly string Identifier = nameof(ExternalCalendarsMainHeaderViewCell);
        public static readonly NSString Key = new NSString(nameof(ExternalCalendarsMainHeaderViewCell));
        public static readonly UINib Nib;

        static ExternalCalendarsMainHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(ExternalCalendarsMainHeaderViewCell), NSBundle.MainBundle);
        }

        protected ExternalCalendarsMainHeaderViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Resources.ExternalCalendarsTitle;
            SubitleLabel.Text = Resources.NativeCalendarsSubtitle;
        }
    }
}
