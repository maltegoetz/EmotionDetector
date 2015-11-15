using System;

using UIKit;
using Xamarin.Media;
using System.Threading.Tasks;
using CoreGraphics;
using System.Drawing;
using Model = EmotionDetector.Emotion.Contract;

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

		async void BtDrawRects_TouchUpInside (object sender, EventArgs e)
		{
			await _vm.Load (ivImage.Image.AsPNG().AsStream());
			DrawRects (_vm.Emotions);
			DrawTestRect ();
		}
			
		void DrawRects(Model.Emotion[] emotions)
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

			foreach (Model.Emotion emotion in emotions)
			{
				UILabel l = new UILabel(new CGRect(relX + emotion.FaceRectangle.Left / scale, 
					relY + emotion.FaceRectangle.Top / scale, emotion.FaceRectangle.Width / scale, 
					emotion.FaceRectangle.Width / scale));
				l.BackgroundColor = UIColor.Blue;
				l.Text = "";
				ivImage.AddSubview (l);
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new CGSize(width, height));
			sourceImage.Draw(new CGRect(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}
	}
}

