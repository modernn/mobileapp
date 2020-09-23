using System;
using Foundation;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class ExternalCalendarsCardInfoViewCell : BaseTableViewCell<ExternalCalendarsCardInfoViewModel>
    {
        public static readonly string Identifier = nameof(ExternalCalendarsCardInfoViewCell);
        public static readonly NSString Key = new NSString(nameof(ExternalCalendarsCardInfoViewCell));
        public static readonly UINib Nib;

        static ExternalCalendarsCardInfoViewCell()
        {
            Nib = UINib.FromName(nameof(ExternalCalendarsCardInfoViewCell), NSBundle.MainBundle);
        }

        protected ExternalCalendarsCardInfoViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Resources.ExternalCalendarsCardInfoTitle;
            BodyLabel.Text = Resources.ExternalCalendarsCardInfoBody;
        }
    }
}
