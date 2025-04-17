#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlaschenpostTestMAUI.Data;
using FlaschenpostTestMAUI.Interfaces;
using FlaschenpostTestMAUI.Models;
using FlaschenpostTestMAUI.ServiceManagers;
using FlaschenpostTestMAUI.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.PageModels
{
    public partial class ProjectListPageModel : ObservableObject
    {
        private readonly IServiceManager<Project> _projectServiceManager;
        private readonly IServiceManager<TodoItem> _todoItemServiceManager;
        private readonly IServiceManager<Category> _categoryServiceManager;
        [ObservableProperty]
        private ObservableCollection<CategoryChartData> _todoCategoryData = [];

        [ObservableProperty]
        private ObservableCollection<Brush> _todoCategoryColors = [];

        [ObservableProperty]
        private ObservableCollection<Project> _projects = [];

        public ProjectListPageModel(IServiceManager<Category> categoryServiceManager,IServiceManager<Project> projectServiceManager, IServiceManager<TodoItem> todoItemServiceManager)
        {
            _projectServiceManager = projectServiceManager;
            _todoItemServiceManager = todoItemServiceManager;
            _categoryServiceManager = categoryServiceManager;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            Projects = new ObservableCollection<Project>(App.AppModel.Projects);

            var tasks = App.AppModel.Tasks;
            foreach (var project in Projects)
            {
                project.Tasks = new ObservableCollection<TodoItem>(tasks.Where(t => t.ProjectId == project.Id).ToList());
            }

            TodoCategoryColors.Clear();
            TodoCategoryData.Clear();
            var categories = App.AppModel.Categories;
            foreach (var project in Projects)
            {
                project.Category = categories.Where(x => x.Id == project.CategoryId).FirstOrDefault();
            }

            foreach (var category in categories)
            {
                TodoCategoryColors.Add(category.ColorBrush);
                var ps = Projects.Where(p => p.CategoryId == category.Id).ToList();
                int tasksCount = ps.SelectMany(p => p.Tasks).Count();
                TodoCategoryData.Add(new(category.Title, tasksCount));
            }

        }

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
        async Task AddProject()
        {
            await Shell.Current.GoToAsync($"project");
        }
    }
}