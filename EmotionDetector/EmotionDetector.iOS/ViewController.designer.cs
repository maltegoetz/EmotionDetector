// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace EmotionDetector.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btChooseImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIEmotionStat esStat { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView ivImage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btChooseImage != null) {
				btChooseImage.Dispose ();
				btChooseImage = null;
			}
			if (esStat != null) {
				esStat.Dispose ();
				esStat = null;
			}
			if (ivImage != null) {
				ivImage.Dispose ();
				ivImage = null;
			}
		}
	}
}
