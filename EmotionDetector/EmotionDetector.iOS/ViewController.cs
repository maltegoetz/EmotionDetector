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
			} else
			{
				//Querformat
				scale = imageSize.Width / imageFrame.Width;
			}

			var relX = (imageFrame.Width - imageSize.Width / scale ) / 2 + imageFrame.X;
			var relY = (imageFrame.Height - imageSize.Height / scale) / 2 + imageFrame.Y;

			foreach (Model.Emotion emotion in emotions)
			{
				var b = new UIFaceButton (); 
				b.Frame = new CGRect (relX + emotion.FaceRectangle.Left / scale, 
					        relY + emotion.FaceRectangle.Top / scale, emotion.FaceRectangle.Width / scale, 
					        emotion.FaceRectangle.Width / scale);
				b.Scores = emotion.Scores;
				b.TouchUpInside += FaceRect_TouchUpInside;
				View.AddSubview (b);
			}
		}

		void BtChooseImage_TouchUpInside (object sender, EventArgs e)
		{
			var picker = new MediaPicker();
			picker.TakePhotoAsync(new StoreCameraMediaOptions {
				Name = "emo.jpg",
				Directory = "EmotionDetector"
			}).ContinueWith (async t => {
				MediaFile file = t.Result;
				ivImage.Image = MaxResizeImage(UIImage.FromFile(file.Path), 640, 640);
				await _vm.Load (ivImage.Image.AsPNG().AsStream());
				DrawRects (_vm.Emotions);
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		void FaceRect_TouchUpInside (object sender, EventArgs e)
		{
			var bt = sender as UIFaceButton;

			var alert = new UIAlertView ("DON'T TOUCH ME!", $"Happy: {bt.Scores.Happiness}", null, "OK", null);
			alert.Show ();
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