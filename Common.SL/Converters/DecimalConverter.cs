using System;
using System.Windows.Data;

namespace Common.Converters
{
	public class DecimalConverter : IValueConverter
	{

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
      if (ReferenceEquals(null, value))
				return String.Empty;

		  var format = "N";
		  if (!ReferenceEquals(null, parameter))
		    format += parameter.ToString();

      return ((decimal)value).ToString(format);  //You may apply your format string here
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;	
		}

		#endregion
	}
}
