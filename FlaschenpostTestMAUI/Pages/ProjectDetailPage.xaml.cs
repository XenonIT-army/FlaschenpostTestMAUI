using FlaschenpostTestMAUI.Models;

namespace FlaschenpostTestMAUI.Pages
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage(ProjectDetailPageModel model)
        {
            InitializeComponent();

            BindingContext = model;
        }
    }

   
}