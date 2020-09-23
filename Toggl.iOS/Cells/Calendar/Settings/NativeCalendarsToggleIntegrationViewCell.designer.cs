// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Cells.Calendar
{
	[Register ("NativeCalendarsToggleIntegrationViewCell")]
	partial class NativeCalendarsToggleIntegrationViewCell
	{
		[Outlet]
		UIKit.UILabel AllowAccessLabel { get; set; }

		[Outlet]
		UIKit.UISwitch AllowAccessSwitch { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AllowAccessLabel != null) {
				AllowAccessLabel.Dispose ();
				AllowAccessLabel = null;
			}

			if (AllowAccessSwitch != null) {
				AllowAccessSwitch.Dispose ();
				AllowAccessSwitch = null;
			}
		}
	}
}
