using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Converters
{
  public class WeekdaysVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((value as Month).MonthNr % 3 == 1 ? Visibility.Visible : Visibility.Collapsed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  } 

}
