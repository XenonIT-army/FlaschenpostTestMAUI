using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FlaschenpostTestMAUI.Models
{
    public class Category : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        public int Id { get; set; }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnNotifyPropertyChanged("Title");
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnNotifyPropertyChanged("Description");
            }
        }
        public string Color { get; set; } = "Green";

        [JsonIgnore]
        public Brush ColorBrush
        {
            get
            {
                if(!string.IsNullOrEmpty(Color))
                {
                    return new SolidColorBrush(Microsoft.Maui.Graphics.Color.Parse(Color));
                }
                else
                {
                    return new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb("#FF0000"));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnNotifyPropertyChanged(String info)
        {
            PropertyChangedEventHandler tmp = Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (tmp != null) { tmp(this, new PropertyChangedEventArgs(info)); }
        }
        public override string ToString() => $"{Title}";
    }
}