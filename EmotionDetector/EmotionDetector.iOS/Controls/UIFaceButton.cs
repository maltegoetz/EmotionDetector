using System;
using UIKit;
using Foundation;
using System.ComponentModel;
using CoreGraphics;
using Model = EmotionDetector.Emotion.Contract;

namespace EmotionDetector.iOS
{
	[Register("UIFaceButton"), DesignTimeVisible(true)]
	public class UIFaceButton : UIButton
	{
		// Default Values
		private int cornerRadius = 10;
		private UIColor borderColor = UIColor.Red;
		private int borderWidth = 3;

		public Model.Scores Scores { get; set; }

		public UIFaceButton () : base (){
			AwakeFromNib ();
		}

		public UIFaceButton(CGRect rect):base(rect){}

		public UIFaceButton(IntPtr handle):base(handle){}

		public override void AwakeFromNib() {
			base.AwakeFromNib();

			Layer.CornerRadius = cornerRadius;
			Layer.BorderColor = borderColor.CGColor;
			Layer.BorderWidth = borderWidth;
			ClipsToBounds = true;
		}

		/*[Export("CornerRadius"), Browsable(true)]
		public int CornerRadius {
			get { return this.cornerRadius; }
			set { cornerRadius = value; }
		}

		[Export("BorderColor"), Browsable(true)]
		public UIColor BorderColor {
			get { return this.borderColor; }
			set { borderColor = value; }
		}

		[Export("BorderWidth"), Browsable(true)]
		public int BorderWidth {
			get { return this.borderWidth; }
			set { borderWidth = value; }
		}*/
	}
}