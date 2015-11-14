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
            //Intent intent = new Intent(MediaStore.ActionImageCapture);
            //App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            //intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));

            var picker = new MediaPicker(this);

            var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
            {
                Name = "test.jpg",
                Directory = "MediaPickerSample"
            });
            StartActivityForResult(intent, 1);
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                // User canceled
                if (resultCode == Result.Canceled)
                    return;

                MediaFile file = await data.GetMediaFileExtraAsync(this);

                _imgView.SetImageBitmap(BitmapFactory.DecodeFile(file.Path));
                
                await _vm.Load(file.GetStream());
            }
            catch (Exception)
            {
                
            }
        }
    }

    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }
}


