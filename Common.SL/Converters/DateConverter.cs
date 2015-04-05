using System;
using System.Windows.Data;

namespace Common.Converters
{
	public class DateConverter : IValueConverter
	{

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo 

culture)
		{
      if (ReferenceEquals(null, value))
				return String.Empty;
      
      return ((DateTime)value).ToShortDateString();  //You may apply your format string here
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo 

culture)
		{
			return value;	
		}

		#endregion
	}
}
