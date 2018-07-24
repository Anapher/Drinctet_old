using System;
using System.Globalization;
using Xamarin.Forms;

namespace Drinctet.Mobile.Converter
{
    public class GetTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.GetType().Name;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}