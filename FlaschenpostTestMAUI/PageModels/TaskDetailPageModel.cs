using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlaschenpostTestMAUI.Data;
using FlaschenpostTestMAUI.Interfaces;
using FlaschenpostTestMAUI.Models;
using FlaschenpostTestMAUI.Services;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.PageModels
{
    public partial class TaskDetailPageModel : ObservableObject, IQueryAttributable
    {
        public const string ProjectQueryKey = "Project";
        private TodoItem? _task;
        private bool _canDelete;
        private readonly IServiceManager<Project> _projectServiceManager;
        private readonly IServiceManager<TodoItem> _todoItemServiceManager;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private bool _isCompleted;

        [ObservableProperty]
        private DateTime? _dueDate;
        [ObservableProperty]
        private TimeSpan _timeDue;

        [ObservableProperty]
        private Priority _priorityEnum;

        [ObservableProperty]
        private ObservableCollection<Project> _projects = [];

        [ObservableProperty]
        private Project? _project;

        [ObservableProperty]
        private int _selectedProjectIndex = -1;


        [ObservableProperty]
        private bool _isExistingProject;

        public List<string> PriorityList
        {
            get
            {
                return Enum.GetNames(typeof(Priority)).ToList();
            }
        }

        public TaskDetailPageModel(IServiceManager<Project> projectServiceManager, IServiceManager<TodoItem> todoItemServiceManager, ModalErrorHandler errorHandler)
        {
            _projectServiceManager = projectServiceManager;
            _todoItemServiceManager = todoItemServiceManager;
            _errorHandler = errorHandler;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            LoadTaskAsync(query).FireAndForgetSafeAsync(_errorHandler);
        }

        private async Task LoadTaskAsync(IDictionary<string, object> query)
        {
            if (query.TryGetValue(ProjectQueryKey, out var project))
                Project = (Project)project;

            int taskId = 0;

            if (query.ContainsKey("id"))
            {
                taskId = Convert.ToInt32(query["id"]);
                _task = await _todoItemServiceManager.GetByIdAsync(taskId);

                if (_task is null)
                {
                    _errorHandler.HandleError(new Exception($"Task Id {taskId} isn't valid."));
                    return;
                }
            }
            else
            {
                _task = new TodoItem();
            }

            // If the project is new, we don't need to load the project dropdown
            if (Project?.Id == 0)
            {
                IsExistingProject = false;
            }
            else
            {
                var res = await _projectServiceManager.GetAllAsync();
                if(res != null)
                {
                    Projects = new ObservableCollection<Project>(res.ToList());
                }
                IsExistingProject = true;
            }

            if (Project is not null)
                SelectedProjectIndex = Projects.ToList().FindIndex(p => p.Id == Project.Id);
            else if (_task?.ProjectId > 0)
                SelectedProjectIndex = Projects.ToList().FindIndex(p => p.Id == _task.ProjectId);

            if (taskId > 0)
            {
                if (_task is null)
                {
                    _errorHandler.HandleError(new Exception($"Task with id {taskId} could not be found."));
                    return;
                }

                Title = _task.Title;
                Description = _task.Description;
                IsCompleted = _task.IsCompleted;
                PriorityEnum = _task.Priority;
                if(_task.DueDate != null)
                {
                    DueDate = _task.DueDate.Value.Date;
                    TimeDue = _task.DueDate.Value.TimeOfDay;
                }
                else
                {
                    DueDate = DateTime.Now;
                    TimeDue = DateTime.Now.TimeOfDay;
                }
                CanDelete = true;
            }
            else
            {
                _task = new TodoItem()
                {
                    ProjectId = Project?.Id ?? 0
                };
            }
        }

        public bool CanDelete
        {
            get => _canDelete;
            set
            {
                _canDelete = value;
                DeleteCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (_task is null)
            {
                _errorHandler.HandleError(
                    new Exception("Task or project is null. The task could not be saved."));

                return;
            }

            _task.Title = Title;
            _task.Description = Description;

            int projectId = Project?.Id ?? 0;

            if (Projects.Count > SelectedProjectIndex && SelectedProjectIndex >= 0)
                _task.ProjectId = projectId = Projects[SelectedProjectIndex].Id;

            _task.IsCompleted = IsCompleted;
            _task.Priority = PriorityEnum;

            _task.DueDate = DueDate + TimeDue;

            if (_task.IsCompleted && _task.CompletedAt == null)
            {
                _task.CompletedAt = DateTime.Now;
            }

            if (_task.Id > 0)
                _todoItemServiceManager.Update(_task).FireAndForgetSafeAsync(_errorHandler);
            else
            {
                _task.CreatedAt = DateTime.Now;
                var res = await _todoItemServiceManager.AddAsync(_task);
                _task = res;
                if(res != null)
                {
                    _task = res;
                    if (_task.Id > 0)
                        await AppShell.DisplayToastAsync("Task saved");
                }
            }

            await Shell.Current.GoToAsync("../?refresh=true");
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task Delete()
        {
            if (_task is null || Project is null)
            {
                _errorHandler.HandleError(
                    new Exception("Task is null. The task could not be deleted."));

                return;
            }

            if (Project.Tasks.Contains(_task))
                Project.Tasks.Remove(_task);

            if (_task.Id > 0)
                await _todoItemServiceManager.Delete(_task);

            await Shell.Current.GoToAsync("..?refresh=true");
            await AppShell.DisplayToastAsync("Task deleted");
        }
    }
}