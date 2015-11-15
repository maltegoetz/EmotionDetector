using System;
using UIKit;
using Foundation;
using System.ComponentModel;
using Model = EmotionDetector.Emotion.Contract;

namespace EmotionDetector.iOS
{
	[Register("UIEmotionStat"), DesignTimeVisible(true)]
	public class UIEmotionStat : UIView
	{
		private NSTimer timer;

		private Model.Scores _scores;
		public Model.Scores Scores {
			get{ return _scores; }
			set
			{
				_scores = value;
				SetNeedsDisplay ();
				//AnimateNewValues ();
			}
		}

		public UIEmotionStat ()
		{
		}

		public UIEmotionStat(IntPtr handle):base(handle){
		}

		public void Reset()
		{
			Scores = null;
		}

		public override void Draw (CoreGraphics.CGRect rect)
		{
			base.Draw (rect);
			if (Scores != null)
				StyleKit.DrawEmotionStatView (rect, Scores.Anger * 100, Scores.Contempt * 100, 
					Scores.Disgust * 100, Scores.Fear * 100, Scores.Happiness * 100, 
					Scores.Neutral * 100, Scores.Sadness * 100, Scores.Surprise * 100);
			else
				StyleKit.DrawEmotionStatView(rect, 0, 0, 0, 0, 0, 0, 0, 0);
		}

		void AnimateNewValues()
		{
			var run = 100;
			if (Scores != null)
			{
				_scores.Anger = _scores.Anger / run;
				_scores.Contempt = _scores.Contempt / run;
				_scores.Disgust = _scores.Disgust / run;
				_scores.Fear = _scores.Disgust / run;
				_scores.Happiness = _scores.Happiness / run;
				_scores.Neutral = _scores.Neutral / run;
				_scores.Sadness = _scores.Sadness / run;
				_scores.Surprise = _scores.Surprise / run;

				timer = NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromMilliseconds (10), delegate
				{
					SetNeedsDisplay ();
					run -= 1;
					if (run == 0)
						timer.Invalidate ();
					
					_scores.Anger = _scores.Anger * (100 - run);
					_scores.Contempt = _scores.Contempt * (100 - run);
					_scores.Disgust = _scores.Disgust * (100 - run);
					_scores.Fear = _scores.Disgust * (100 - run);
					_scores.Happiness = _scores.Happiness * (100 - run);
					_scores.Neutral = _scores.Neutral * (100 - run);
					_scores.Sadness = _scores.Sadness * (100 - run);
					_scores.Surprise = _scores.Surprise * (100 - run);
				});
			}
		}
	}
}

