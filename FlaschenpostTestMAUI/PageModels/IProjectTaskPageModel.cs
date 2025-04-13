using CommunityToolkit.Mvvm.Input;
using FlaschenpostTestMAUI.Models;

namespace FlaschenpostTestMAUI.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<TodoItem> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}