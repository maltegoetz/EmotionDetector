using EmotionDetector.Emotion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using System.Threading.Tasks;
using Windows.Storage.Streams;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace EmotionDetector.WinPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture captureManager;
        EmotionViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Loaded += MainPage_Loaded;
            vm = new EmotionViewModel();
            DataContext = vm;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();
            captureManager.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
            capturePreview.Source = captureManager;
            await captureManager.StartPreviewAsync();
        }

        private async void photoButton_Click(object sender, RoutedEventArgs e)
        {
            photoButton.Visibility = Visibility.Collapsed;
            backButton.Visibility = Visibility.Visible;
            imagePreview.Source = null;
            for(int i = 1; i < resultCanvas.Children.Count; i++)
            {
                resultCanvas.Children.RemoveAt(i);
            }
            capturePreview.Visibility = Visibility.Collapsed;

            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "TestPhoto.jpg",
                CreationCollisionOption.GenerateUniqueName);

            using (var imageStream = new InMemoryRandomAccessStream())
            {
                //generate stream from MediaCapture
                await captureManager.CapturePhotoToStreamAsync(imgFormat, imageStream);

                //create decoder and encoder
                BitmapDecoder dec = await BitmapDecoder.CreateAsync(imageStream);
                BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(imageStream, dec);

                //roate the image
                enc.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees;

                //write changes to the image stream
                await enc.FlushAsync();

                //save the image
                StorageFolder folder = KnownFolders.SavedPictures;
                
                //store stream in file
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    try
                    {
                        //because of using statement stream will be closed automatically after copying finished
                        await RandomAccessStream.CopyAsync(imageStream, fileStream.AsOutputStream());
                    }
                    catch
                    {

                    }
                }
            }

            BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));
            
            imagePreview.Source = bmpImage;
            
            await vm.Load((await file.OpenReadAsync()).AsStream());

            imagePreview.Height = bmpImage.PixelHeight;
            imagePreview.Width = bmpImage.PixelWidth;
            

            foreach (Emotion.Contract.Emotion emo in vm.Emotions)
            {
                var rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Transparent);
                rect.Stroke = new SolidColorBrush(Colors.Gray);
                rect.StrokeThickness = 4;
                rect.Height = emo.FaceRectangle.Height;
                rect.Width = emo.FaceRectangle.Width;
                rect.Margin = new Thickness(emo.FaceRectangle.Left, emo.FaceRectangle.Top, 0, 0);
                rect.HorizontalAlignment = HorizontalAlignment.Left;
                rect.VerticalAlignment = VerticalAlignment.Top;
                rect.Tapped += (s, args) => {
                    dataGrid.DataContext = emo.Scores;
                };
                resultCanvas.Children.Add(rect);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            photoButton.Visibility = Visibility.Visible;
            backButton.Visibility = Visibility.Collapsed;
            capturePreview.Visibility = Visibility.Visible;
        }
    }
}
