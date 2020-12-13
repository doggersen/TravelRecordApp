namespace TravelRecordApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new TravelRecordApp.App());
        }
    }
}
