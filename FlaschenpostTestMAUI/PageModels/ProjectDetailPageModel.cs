using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlaschenpostTestMAUI.Interfaces;
using FlaschenpostTestMAUI.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.PageModels
{
    public partial class ProjectDetailPageModel : ObservableObject, IQueryAttributable, IProjectTaskPageModel
    {
        private Project? _project;
        private readonly IServiceManager<Project> _projectServiceManager;
        private readonly IServiceManager<TodoItem> _todoItemServiceManager;
        private readonly IServiceManager<Category> _categoryServiceManager;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TodoItem> _tasks = [];

        [ObservableProperty]
        private ObservableCollection<Category> _categories = [];

        [ObservableProperty]
        private Category? _category;

        [ObservableProperty]
        private int _categoryIndex = -1;

      

        [ObservableProperty]
        private string _icon = FluentUI.ribbon_24_regular;

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        private List<string> _icons =
        [
            FluentUI.ribbon_24_regular,
            FluentUI.ribbon_star_24_regular,
            FluentUI.trophy_24_regular,
            FluentUI.badge_24_regular,
            FluentUI.book_24_regular,
            FluentUI.people_24_regular,
            FluentUI.bot_24_regular
        ];

        public bool HasCompletedTasks
            => _project?.Tasks.Any(t => t.IsCompleted) ?? false;

        public ProjectDetailPageModel(IServiceManager<Project> projectServiceManager, IServiceManager<TodoItem> todoItemServiceManager, IServiceManager<Category> categoryServiceManager, ModalErrorHandler errorHandler)
        {
            _projectServiceManager = projectServiceManager;
            _todoItemServiceManager = todoItemServiceManager;
            _categoryServiceManager = categoryServiceManager;
            _errorHandler = errorHandler;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Project", out var project))
            {
                _project = (Project)project;
                LoadData().FireAndForgetSafeAsync(_errorHandler);
            }
            else if (query.ContainsKey("refresh"))
            {
                RefreshData().FireAndForgetSafeAsync(_errorHandler);
            }
            else
            {
                //Task.WhenAll(LoadCategories()).FireAndForgetSafeAsync(_errorHandler);
                Categories = new ObservableCollection<Category>(App.AppModel.Categories);
                _project = new();
                _project.Tasks = [];
                Tasks = new ObservableCollection<TodoItem>(_project.Tasks);
            }
        }
        private async Task RefreshData()
        {
            if (_project.IsNullOrNew())
            {
                if (_project is not null)
                    Tasks = new(_project.Tasks);

                return;
            }
            Tasks = new ObservableCollection<TodoItem>(App.AppModel.Tasks.Where(t => t.ProjectId == _project.Id));
            _project.Tasks = Tasks;
        }

        private async Task LoadData()
        {
            try
            {
                IsBusy = true;

                if (_project.IsNullOrNew())
                {
                    _errorHandler.HandleError(new Exception($"Project with id {_project.Id} could not be found."));
                    return;
                }

                Title = _project.Title;
                Description = _project.Description;
                Tasks = new ObservableCollection<TodoItem>(_project.Tasks);

                Icon = _project.Icon;
                Categories = new ObservableCollection<Category>(App.AppModel.Categories);
                Category = Categories?.FirstOrDefault(c => c.Id == _project.CategoryId);
                CategoryIndex = App.AppModel.Categories?.FindIndex(c => c.Id == _project.CategoryId) ?? -1;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }

        [RelayCommand]
        private async Task TaskCompleted(TodoItem task)
        {
            await _todoItemServiceManager.AddAsync(task);
            App.AppModel.Tasks.Add(task);
            OnPropertyChanged(nameof(HasCompletedTasks));
        }


        [RelayCommand]
        private async Task Save()
        {
            if (_project is null)
            {
                _errorHandler.HandleError(
                    new Exception("Project is null. Cannot Save."));

                return;
            }

            _project.Title = Title;
            _project.Description = Description;
            _project.CategoryId = Category?.Id ?? 0;
            _project.Icon = Icon ?? FluentUI.ribbon_24_regular;
            if (_project.Id > 0)
            {
                _projectServiceManager.Update(_project).FireAndForgetSafeAsync(_errorHandler);
                var item = App.AppModel.Projects.Where(x => x.Id == _project.Id).FirstOrDefault();
                if (item != null)
                {
                    item = _project;
                }
            }
            else
            {
                var res = await _projectServiceManager.AddAsync(_project);
                App.AppModel.Projects.Add(res);
            }
            foreach (var task in _project.Tasks)
            {
                if (task.Id == 0)
                {
                    task.ProjectId = _project.Id;
                    var item = await _todoItemServiceManager.AddAsync(task);
                    App.AppModel.Tasks.Add(item);
                }
            }

            await Shell.Current.GoToAsync("..");
            await AppShell.DisplayToastAsync("Project saved");
        }

        [RelayCommand]
        private async Task AddTask()
        {
            if (_project is null)
            {
                _errorHandler.HandleError(
                    new Exception("Project is null. Cannot navigate to task."));

                return;
            }
            await Shell.Current.GoToAsync($"task",
                new ShellNavigationQueryParameters(){
                    {TaskDetailPageModel.ProjectQueryKey, _project}
                });
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (_project.IsNullOrNew())
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            await _projectServiceManager.Delete(_project);
            App.AppModel.Projects.Remove(_project);
            await Shell.Current.GoToAsync("..");
            await AppShell.DisplayToastAsync("Project deleted");
        }

        [RelayCommand]
        private Task NavigateToTask(TodoItem task) =>
            Shell.Current.GoToAsync($"task?id={task.Id}");

        [RelayCommand]
        private async Task CleanTasks()
        {
            var completedTasks = Tasks.Where(t => t.IsCompleted).ToArray();
            foreach (var task in completedTasks)
            {
                await _todoItemServiceManager.Delete(task);
                App.AppModel.Tasks.Remove(task);
                Tasks.Remove(task);
            }

            Tasks = new(Tasks);
            OnPropertyChanged(nameof(HasCompletedTasks));
            await AppShell.DisplayToastAsync("All cleaned up!");
        }
    }
}
