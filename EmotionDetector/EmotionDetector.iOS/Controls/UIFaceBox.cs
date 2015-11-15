using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.ComponentModel;

namespace EmotionDetector.iOS
{
	[Register("UIFaceBox"), DesignTimeVisible(true)]
	public class UIFaceBox : UIControl
	{
		public UIFaceBox(IntPtr handle)
			: base(handle)
		{
		}

		public UIFaceBox ()
		{
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);
			EmotionDetectorStyles.DrawFaceRect (rect);
		}
	}
}

