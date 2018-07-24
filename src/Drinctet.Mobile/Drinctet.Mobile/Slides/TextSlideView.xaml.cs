using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Drinctet.Mobile.Slides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextSlideView : ContentView
    {
        public static readonly BindableProperty HeaderProperty =
            BindableProperty.Create(nameof(Header), typeof(View), typeof(TextSlideView));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TextSlideView));

        public TextSlideView()
        {
            InitializeComponent();
        }

        public View Header
        {
            get => (View) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}