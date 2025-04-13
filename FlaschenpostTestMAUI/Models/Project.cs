using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FlaschenpostTestMAUI.Models
{
    public class Project : INotifyPropertyChanged
    {
        private Category _category;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _icon = string.Empty;
        private ObservableCollection<TodoItem> _tasks;
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

        public int CategoryId { get; set; }
        public string Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                OnNotifyPropertyChanged("Icon");
            }
        }
        [JsonIgnore]
        public Category Category 
        {
            get { return _category; }

            set
            {
                _category = value;
                OnNotifyPropertyChanged("Category");
            }
        
        }
        [JsonIgnore]
        public ObservableCollection<TodoItem> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnNotifyPropertyChanged("Tasks");
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