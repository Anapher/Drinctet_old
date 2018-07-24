using Drinctet.Core;
using Drinctet.Mobile.Pages;
using Drinctet.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Drinctet.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var store = new MobilePageStore();
            store.RegisterPage<GamePage, GameViewModel>();

            var page = new MainPage();
            page.BindingContext = new MainPageViewModel {ViewInterface = new MobileInterface(store, page)};
            MainPage = new NavigationPage(page);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}