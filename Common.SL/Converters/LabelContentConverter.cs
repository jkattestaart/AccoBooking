using System;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Data;

namespace Common.Converters
{
  public class LabelContentConverter : IValueConverter
  {
    public static ResourceManager LabelResource { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
        return LabelResource.GetString((value as FrameworkElement).Name);
      return (value as FrameworkElement).Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
