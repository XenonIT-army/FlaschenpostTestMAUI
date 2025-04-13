using FlaschenpostTestMAUI.Models;
using FlaschenpostTestMAUI.PageModels;

namespace FlaschenpostTestMAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        private MainPageModel mainPage;
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
            mainPage = model;
        }

        private void MyButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            mainPage.FilterCommand.Execute(null);
        }
    }
}