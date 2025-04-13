using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FlaschenpostTestMAUI.Models
{
    public class TodoItem : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        private bool _isCompleted = false;
        private DateTime _createdAt;
        private DateTime _dueDate;
        private DateTime _completedAt;
        private Priority _priority;
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
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                OnNotifyPropertyChanged("IsCompleted");
            }
        }
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set
            {
                _createdAt = value;
                OnNotifyPropertyChanged("CreatedAt");
            }
        }
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value ?? DateTime.Now;
                OnNotifyPropertyChanged("DueDate");
            }
        }
        public DateTime? CompletedAt
        {
            get { return _completedAt; }
            set
            {
                _completedAt = value ?? DateTime.Now;
                OnNotifyPropertyChanged("CompletedAt");
            }
        }
        public Priority Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                OnNotifyPropertyChanged("Priority");
            }
        }
        public int ProjectId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnNotifyPropertyChanged(String info)
        {
            PropertyChangedEventHandler tmp = Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (tmp != null) { tmp(this, new PropertyChangedEventArgs(info)); }
        }
    }
    public enum Priority
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
}