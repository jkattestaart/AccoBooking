using System;
using System.Globalization;
using System.Windows.Data;

namespace Common.Converters
{
  public class SystemCodeConverter : IValueConverter
  {

    public object Convert(object value, Type targetType,
                          object parameter, CultureInfo culture)
    {
      if (parameter != null)
      {
        string group = parameter.ToString();
        string code = value.ToString();

        if (!string.IsNullOrEmpty(group))
        {
          if (group=="GENDER")
          {
            if (code == "F")
              return "Female";
            if (code == "M")
              return "Male";
            return "Unknown";
          }
          return code;    // systemcode(group, value.ToString())
        }
      }
      return value;
    }

    public object ConvertBack
      (object value, Type targetType,
       object parameter, CultureInfo culture)
    {
      return value;
    }
  }
}
