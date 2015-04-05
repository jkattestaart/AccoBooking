using System;
using System.Globalization;
using System.Windows.Data;

namespace Common.Converters
{
  public class SourceUriConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new Uri((string)value, UriKind.Absolute);
      //return new Uri("./" + (string) value, UriKind.Relative);
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
