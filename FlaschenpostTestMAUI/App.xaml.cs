namespace FlaschenpostTestMAUI
{
    public partial class App : Application
    {
        public static AppModel AppModel { get; private set; } = new AppModel();
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}