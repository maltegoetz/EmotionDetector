using System;

using UIKit;
using Xamarin.Media;
using System.Threading.Tasks;
using CoreGraphics;

namespace EmotionDetector.iOS
{
	public partial class ViewController : UIViewController
	{
		EmotionViewModel _vm;
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			_vm = new EmotionViewModel();
			btChooseImage.TouchUpInside += BtChooseImage_TouchUpInside;
			btDrawRects.TouchUpInside += BtDrawRects_TouchUpInside;
		}

		void BtDrawRects_TouchUpInside (object sender, EventArgs e)
		{
			
			DrawTestRect ();
		}
			
		void DrawTestRect()
		{
			//Find top left corner of the Image
			var imageFrame = ivImage.Frame;
			var imageSize = ivImage.Image.Size;
			nfloat scale = 0f;

			if (imageFrame.Width / imageSize.Width >= imageFrame.Height / imageSize.Height)
			{
				//Hochformat
				scale = imageSize.Height / imageFrame.Height;
				//scale = imageFrame.Width / imageSize.Width;
			} else
			{
				//Querformat
				scale = imageSize.Width / imageFrame.Width;
				//scale = imageFrame.Height / imageSize.Height;
			}

			var relX = (imageFrame.Width - imageSize.Width / scale ) / 2;
			var relY = (imageFrame.Height - imageSize.Height / scale) / 2;

			/*UILabel l = new UILabel(new CGRect(relX, relY, 90, 80));
			l.BackgroundColor = UIColor.Blue;
			l.Text = "Hello World";
			ivImage.AddSubview (l);*/

			var box = new UIFaceBox ();
			ivImage.AddSubview (box);
		}

		void BtChooseImage_TouchUpInside (object sender, EventArgs e)
		{
			var picker = new MediaPicker();
			picker.TakePhotoAsync(new StoreCameraMediaOptions {
				Name = "emo.jpg",
				Directory = "EmotionDetector"
			}).ContinueWith (t => {
				MediaFile file = t.Result;
				ivImage.Image = UIImage.FromFile(file.Path);
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

