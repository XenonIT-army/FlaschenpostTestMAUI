using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlaschenpostTestMAUI.Data;
using FlaschenpostTestMAUI.Interfaces;
using FlaschenpostTestMAUI.Models;
using FlaschenpostTestMAUI.Services;

namespace FlaschenpostTestMAUI.PageModels
{
    public partial class ManageMetaPageModel : ObservableObject
    {
        private readonly IServiceManager<Category> _categoryServiceManager;

        [ObservableProperty]
        private ObservableCollection<Category> _categories = [];

        public ManageMetaPageModel(IServiceManager<Category> categoryServiceManager)
        {
            _categoryServiceManager = categoryServiceManager;
        }

        private async Task LoadData()
        {
            var categoriesList = await _categoryServiceManager.GetAllAsync();
            if (categoriesList is null)
                return;
            Categories = new ObservableCollection<Category>(categoriesList);
        }

        [RelayCommand]
        private Task Appearing()
            => LoadData();

        [RelayCommand]
        private async Task SaveCategories()
        {
            foreach (var category in Categories)
            {
                await _categoryServiceManager.Update(category);
            }

            await AppShell.DisplayToastAsync("Categories saved");
        }

        [RelayCommand]
        private async Task DeleteCategory(Category category)
        {
            Categories.Remove(category);
            await _categoryServiceManager.Delete(category);
            await AppShell.DisplayToastAsync("Category deleted");
        }

        [RelayCommand]
        private async Task AddCategory()
        {
            var category = new Category();
            category = await _categoryServiceManager.AddAsync(category);

            Categories.Add(category);
            await AppShell.DisplayToastAsync("Category added");
        }
    }
}
