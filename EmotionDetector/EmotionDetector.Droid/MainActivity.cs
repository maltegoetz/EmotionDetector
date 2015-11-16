using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.IO;
using Android.Graphics;
using Android.Provider;
using Android.Content.PM;
using System.Collections.Generic;
using EmotionDetector.Droid.Common;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Media;
using Android.Graphics.Drawables;

namespace EmotionDetector.Droid
{
    [Activity(Label = "Emotion Detector", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        EmotionViewModel _vm;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            /* Snippet 1 Start */
            _vm = new EmotionViewModel();
            Button photobutton = FindViewById<Button>(Resource.Id.myButton);
            photobutton.Click += TakeAPicture;
            /* Snippet 1 Ende */
        }
        /* Snippet 2 Start */
        /// <summary>
        /// Startet ein Intent zum Aufnehmen eines Fotos
        /// </summary>
        /// <param name="sender">Button sender</param>
        /// <param name="eventArgs">Event Arguments</param>
        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            var picker = new MediaPicker(this);
            var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
            {
                Name = "test.jpg",
                Directory = "CameraAppDemo"
            });
            StartActivityForResult(intent, 1);
        }
        /* Snippet 2 Ende */
        /// <summary>
        /// Aufgenommenes Foto durch Project Oxford analysieren lassen und das Ergebnis visualisieren
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            var progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            try
            {
                // User canceled
                if (resultCode == Result.Canceled)
                    return;

                MediaFile file = await data.GetMediaFileExtraAsync(this);

                //Alte Face-Rectangles entfernen
                var rellayout = FindViewById<RelativeLayout>(Resource.Id.relativeLayout1);
                for (int i = 2; i < rellayout.ChildCount; i++)
                {
                    rellayout.RemoveViewAt(i);
                }
                
                progressBar.Visibility = ViewStates.Visible;

                //Foto in ImageView anzeigen
                var bmp = BitmapFactory.DecodeFile(file.Path);
                ImageView _imgView = FindViewById<ImageView>(Resource.Id.imageView1);
                _imgView.SetImageBitmap(bmp);

                //Bild Analyse durch Project Oxford
                await _vm.Load(file.GetStream());

                //Brechnen der Face Rectangle Positionen
                float scale;
                if (_imgView.Width / bmp.Width >= _imgView.Height / bmp.Height)
                {
                    scale = (float)bmp.Height / _imgView.Height;
                }
                else
                {
                    scale = (float)bmp.Width / _imgView.Width;
                }

                var relX = (_imgView.Width - bmp.Width / scale) / 2;
                var relY = (_imgView.Height - bmp.Height / scale) / 2;
                foreach (Emotion.Contract.Emotion emo in _vm.Emotions)
                {
                    //Zeichnen der Face Rectangles
                    var butt = new Button(this);
                    GradientDrawable drawable = new GradientDrawable();
                    drawable.SetShape(ShapeType.Rectangle);
                    drawable.SetStroke(5, Color.Rgb(34, 135, 202));
                    drawable.SetColor(Color.Transparent);
                    butt.Background = drawable;
                    var layoutparams = new RelativeLayout.LayoutParams((int)Math.Ceiling(emo.FaceRectangle.Width / scale), (int)Math.Ceiling(emo.FaceRectangle.Height / scale));
                    layoutparams.SetMargins((int)Math.Ceiling(emo.FaceRectangle.Left / scale + relX), (int)Math.Ceiling(emo.FaceRectangle.Top / scale + relY), 0, 0);

                    butt.LayoutParameters = layoutparams;
                    butt.SetPadding(0, 0, 0, 0);
                    butt.Click += (e, s) =>
                    {
                        //Bei Klick auf Face Rectangle Emotionswerte anzeigen
                        (FindViewById<ProgressBar>(Resource.Id.angerProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Anger * 100);
                        (FindViewById<ProgressBar>(Resource.Id.contemptProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Contempt * 100);
                        (FindViewById<ProgressBar>(Resource.Id.disgustProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Disgust * 100);
                        (FindViewById<ProgressBar>(Resource.Id.fearProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Fear * 100);
                        (FindViewById<ProgressBar>(Resource.Id.happinessProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Happiness * 100);
                        (FindViewById<ProgressBar>(Resource.Id.neutralProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Neutral * 100);
                        (FindViewById<ProgressBar>(Resource.Id.sadnessProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Sadness * 100);
                        (FindViewById<ProgressBar>(Resource.Id.surpriseProgressBar)).Progress = (int)Math.Ceiling(emo.Scores.Surprise * 100);
                    };
                    rellayout.AddView(butt);
                }
            }
            finally{ progressBar.Visibility = ViewStates.Invisible; }
        }
        
    }
    
}


