using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Drinctet.Mobile.Pages;
using Xamarin.Forms;

namespace Drinctet.Mobile.Droid
{
    [Activity(Label = "Drinctet.Mobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            MessagingCenter.Subscribe<GamePage>(this, "setLandscape",
                page => { RequestedOrientation = ScreenOrientation.Landscape; });

            MessagingCenter.Subscribe<GamePage>(this, "undoLandscape",
                page => { RequestedOrientation = ScreenOrientation.Unspecified; });

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

