using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlaschenpostTestMAUI.Interfaces;
using FlaschenpostTestMAUI.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.PageModels
{
    public partial class MainPageModel : ObservableObject, IProjectTaskPageModel
    {
        private bool _isNavigatedTo;
        private bool _dataLoaded;
        private readonly ModalErrorHandler _errorHandler;
        private readonly IServiceManager<TodoItem> _todoItemServiceManager;
        private readonly IServiceManager<Project> _projectServiceManager;
        private readonly IServiceManager<Category> _categoryServiceManager;
        private ObservableCollection<TodoItem> _tasksAll = [];

        [ObservableProperty]
        private ObservableCollection<TodoItem> _tasks = [];

        [ObservableProperty]
        private ObservableCollection<Project> _projects = [];

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        bool _isRefreshing;
        [ObservableProperty]
        bool _isOpenChecked;
        [ObservableProperty]
        bool _isCompletedChecked;
        [ObservableProperty]
        bool _isAllChecked;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd, MMM d");

        public bool HasCompletedTasks
            => Tasks?.Any(t => t.IsCompleted) ?? false;

        IAsyncRelayCommand<TodoItem> IProjectTaskPageModel.NavigateToTaskCommand => throw new NotImplementedException();

        public MainPageModel(IServiceManager<Project> projectServiceManager, IServiceManager<TodoItem> todoItemServiceManager, IServiceManager<Category> categoryServiceManager, ModalErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            _todoItemServiceManager = todoItemServiceManager;
            _categoryServiceManager = categoryServiceManager;
            _projectServiceManager = projectServiceManager;
            _dataLoaded = true;
            LoadData();
        }

        private async void  LoadData()
        {
            try
            {
                IsBusy = true;

                var res  = await _projectServiceManager.GetAllAsync();
               if(res != null)
                {
                    Projects = new ObservableCollection<Project>(res.ToList());
                }

               
                //var chartData = new ObservableCollection<CategoryChartData>();
                var chartColors = new ObservableCollection<Brush>();
                var resItem = await _todoItemServiceManager.GetAllAsync();
                if (resItem != null)
                {
                    Tasks = new ObservableCollection<TodoItem>(resItem);
                    _tasksAll = new ObservableCollection<TodoItem>(Tasks);
                    foreach (var project in Projects)
                    {
                        project.Tasks = new ObservableCollection<TodoItem>(Tasks.Where(t => t.ProjectId == project.Id).ToList());
                    }
                    Filter();
                }
                var categories = await _categoryServiceManager.GetAllAsync();
                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        foreach (var project in Projects)
                        {
                            if(project.CategoryId == category.Id)
                            {
                                project.Category = category;
                            }
                        }
                    }
                }
            }
            finally
            {
                IsBusy = false;
                _dataLoaded = false;
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }
        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                 LoadData();
              
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void NavigatedTo() =>
            _isNavigatedTo = true;

        [RelayCommand]
        private void NavigatedFrom() =>
            _isNavigatedTo = false;

        [RelayCommand]
        private async Task Appearing()
        {
            if (!_dataLoaded)
            {
                _dataLoaded = true;
                await Refresh();
            }
            // This means we are being navigated to
            //else if (!_isNavigatedTo)
            //{
            //    await Refresh();
            //}
        }

        [RelayCommand]
        private Task TaskCompleted(TodoItem task)
        {
            OnPropertyChanged(nameof(HasCompletedTasks));
            return _todoItemServiceManager.Update(task);
        }

        [RelayCommand]
        private Task AddTask()
            => Shell.Current.GoToAsync($"task");

        [RelayCommand]
        private Task NavigateToProject(Project project)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                    { "Project", project }
            };
            return Shell.Current.GoToAsync($"project?id={project.Id}", navigationParameter);
        }

        [RelayCommand]
        private Task NavigateToTask(TodoItem task)
        {
            var project = Projects.Where(t => t.Id == task.ProjectId).FirstOrDefault(); 
            var navigationParameter = new Dictionary<string, object>
            {
                    { "Project", project }
            };
            return Shell.Current.GoToAsync($"task?id={task.Id}", navigationParameter);
        }

        [RelayCommand]
        private async Task CleanTasks()
        {
            var completedTasks = Tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks)
            {
                await _todoItemServiceManager.Delete(task);
                Tasks.Remove(task);
            }

            OnPropertyChanged(nameof(HasCompletedTasks));
            Tasks = new(Tasks);
            await AppShell.DisplayToastAsync("All cleaned up!");
        }

        [RelayCommand]
        private void Filter()
        {
            if (IsOpenChecked)
            {
                Tasks = new ObservableCollection<TodoItem>(_tasksAll.Where(t => !t.IsCompleted));
                IsAllChecked = false;
                IsCompletedChecked = false;
            }
            else if (IsCompletedChecked)
            {
                Tasks = new ObservableCollection<TodoItem>(_tasksAll.Where(t => t.IsCompleted));
                IsAllChecked = false;
                IsOpenChecked = false;
            }
            else
            {
                Tasks = new ObservableCollection<TodoItem>(_tasksAll);
                IsCompletedChecked = false;
                IsOpenChecked = false;
            }
        }

    }
}