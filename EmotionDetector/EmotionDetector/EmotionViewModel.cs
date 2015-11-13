using EmotionDetector.Emotion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionDetector
{
    public class EmotionViewModel : INotifyPropertyChanged
    {
        private bool _IsDataLoading;
        private Emotion.Contract.Emotion[] _Emotions;
        public EmotionViewModel()
        {
            IsDataLoading = false;
        }
        public async Task Load(Stream stream)
        {
            IsDataLoading = true;
            EmotionServiceClient esc = new EmotionServiceClient("4064c52bfb044805a39d2d3c33749f44");
            Emotions = await esc.RecognizeAsync(stream);
            
            IsDataLoading = false;
        }
        public bool IsDataLoading
        {
            get { return _IsDataLoading; }
            set {
                _IsDataLoading = value;
                OnPropertyChanged("IsDataLoading");
            }
        }
        public Emotion.Contract.Emotion[] Emotions {
            get { return _Emotions; }
            set {
                _Emotions = value;
                OnPropertyChanged("Emotions");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
