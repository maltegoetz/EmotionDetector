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
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace EmotionDetector.WinPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture captureManager;

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
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();
            capturePreview.Source = captureManager;
            await captureManager.StartPreviewAsync();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            capturePreview.Visibility = Visibility.Collapsed;

            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "TestPhoto.jpg",
                CreationCollisionOption.GenerateUniqueName);
            
            await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);
            
            BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));
            
            imagePreview.Source = bmpImage;

            var evm = new EmotionViewModel();
            await evm.Load((await file.OpenReadAsync()).AsStream());

            imagePreview.Height = bmpImage.PixelHeight;
            imagePreview.Width = bmpImage.PixelWidth;

            await Task.Delay(1);

            foreach (Emotion.Contract.Emotion emo in evm.Emotions)
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
                resultCanvas.Children.Add(rect);
            }
        }
    }
}
