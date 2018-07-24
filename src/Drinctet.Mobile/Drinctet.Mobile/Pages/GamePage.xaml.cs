using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Drinctet.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
		public GamePage ()
		{
			InitializeComponent ();
		}

	    protected override void OnAppearing()
	    {
	        base.OnAppearing();
            MessagingCenter.Send(this, "setLandscape");
	    }

	    protected override void OnDisappearing()
	    {
	        base.OnDisappearing();
	        MessagingCenter.Send(this, "undoLandscape");
        }
	}
}