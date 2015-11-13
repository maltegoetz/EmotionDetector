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

namespace EmotionDetector.Droid
{
    [Activity(Label = "Emotion Detector", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ImageView _imgView;
        EmotionViewModel _vm;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            _vm = new EmotionViewModel();
            Button photobutton = FindViewById<Button>(Resource.Id.myButton);
            photobutton.Click += TakeAPicture;
            CreateDirectoryForPictures();
            _imgView = FindViewById<ImageView>(Resource.Id.imageView1);
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }
        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            
                // Make it available in the gallery

                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                // Display in ImageView. We will resize the bitmap to fit the display.
                // Loading the full sized image will consume to much memory
                // and cause the application to crash.

                int height = Resources.DisplayMetrics.HeightPixels;
                int width = _imgView.Height;
                App.bitmap = BitmapHelpers.LoadAndResizeBitmap(App._file.Path, width, height);
                if (App.bitmap != null)
                {
                    _imgView.SetImageBitmap(App.bitmap);
                    Task.Run(async () =>
                    {
                        try
                        {
                            using (var stream = new MemoryStream())
                            {
                                App.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                                await _vm.Load(stream);
                                stream.Close();
                            }
                            App.bitmap = null;
                        }
                        catch (Exception e) {
                        }
                    });
                    
                }
                // Dispose of the Java side bitmap.
                GC.Collect();
            
        }
    }
    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }
}


