using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.Converters
{
  public class DayColorConverter : IValueConverter
  {

    public object Convert(object value, Type targetType,
      object parameter, CultureInfo culture)
    {

      Day day = (value as MonthItem).Day;
      Double opacity = day.Date < DateTime.Now ? 0.5 : 1;

      //days has same status?
      //if (day.ArrivalColor == day.DepartureColor)
      //  return new SolidColorBrush(day.ArrivalBrush) {Opacity = opacity};

      Color clr1 = day.DepartureBrush;
      Color clr2 = day.RentBrush;
      Color clr3 = day.ArrivalBrush;

      var brush = new LinearGradientBrush
      {
        StartPoint = new Point(1, 0.3),
        EndPoint = new Point(1, 0.7),
        Opacity = opacity
      };

      brush.GradientStops.Add(new GradientStop {Color = clr1, Offset = 0.0});
      brush.GradientStops.Add(new GradientStop {Color = clr2, Offset = 0.4});
      brush.GradientStops.Add(new GradientStop {Color = clr3, Offset = 0.6});

      return brush;
      
      //Uri imageUri;
      //BitmapImage image = null;

      //if (Uri.TryCreate("/AccoBooking;Component/Assets/Images/" + clr1 + clr2 + clr3 + ".png", UriKind.RelativeOrAbsolute, out imageUri))
      //{
      //  image = new BitmapImage(imageUri);
      //}

      //var brush = new ImageBrush();
      //brush.ImageSource = image;  //new BitmapImage(new Uri("Assets/Images/Booked-Available.png", UriKind.RelativeOrAbsolute));
      //brush.Opacity = opacity;

      //return brush;
      
    }
    
    public object ConvertBack(object value, Type targetType,
                              object parameter, CultureInfo culture)
    {
      return null;
    }
  }

}



