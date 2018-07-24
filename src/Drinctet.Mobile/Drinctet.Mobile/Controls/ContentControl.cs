using Xamarin.Forms;

namespace Drinctet.Mobile.Controls
{
    public class ContentControl : ContentView
    {
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate),
            typeof(DataTemplate), typeof(ContentControl), null, BindingMode.OneWay, null, OnItemTemplateChanged);

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data),
            typeof(object), typeof(ContentControl), null, BindingMode.OneWay, null, OnDataChanged);

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        private static void OnItemTemplateChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var cp = (ContentControl) bindable;

            var template = (DataTemplate) newvalue;
            if (template != null)
            {
                var content = (View) template.CreateContent();
                content.BindingContext = cp.Data;
                cp.Content = content;
            }
            else cp.Content = null;
        }

        private static void OnDataChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var cp = (ContentControl)bindable;
            if (cp.Content != null)
            {
                var content = (View) cp.ItemTemplate.CreateContent();
                content.BindingContext = newvalue;
                cp.Content = content;
            }
        }
    }
}