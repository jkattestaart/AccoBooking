using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.Converters
{
  public class DayColorAvailabilityConverter : IValueConverter
  {

    public object Convert(object value, Type targetType,
                          object parameter, CultureInfo culture)
    {

      Day day = (value as MonthItem).Day;
      Double opacity = day.Date < DateTime.Now ? 0.5 : 1;

      if (day.HasOverlap)
      {
        //The whole day is available
        if (day.IsMorningBookable && day.IsMiddayBookable)
          return new SolidColorBrush(Color.FromArgb(255, 00, 100, 00)) {Opacity = opacity};

        //The whole day is not bookable (and not booked)
        if (!day.IsMorningBookable && !day.IsMorningBooked && !day.IsMiddayBookable && !day.IsMiddayBooked)
          return new SolidColorBrush(Colors.Transparent) {Opacity = opacity};

        Color clr1 = day.IsMorningBookable ? Color.FromArgb(255, 00, 100, 00) : Colors.Transparent;
        Color clr2 = Colors.Transparent;
        Color clr3 = day.IsMiddayBookable ? Color.FromArgb(255, 00, 100, 00) : Colors.Transparent;

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

      }
      else
      {

        //The whole day is available
        if (day.IsMorningBookable && day.IsMiddayBookable)
          return new SolidColorBrush(Colors.Green) {Opacity = opacity};

        return new SolidColorBrush();

        ////The whole day is not bookable (and not booked)
        //if (!day.IsMorningBookable && !day.IsMorningBooked && !day.IsMiddayBookable && !day.IsMiddayBooked)
        //  return new SolidColorBrush(Colors.Transparent) {Opacity = opacity};

        //Color clr1 = day.IsMorningBookable ? Colors.Green : Colors.Transparent;
        //Color clr2 = Colors.Transparent;
        //Color clr3 = day.IsMiddayBookable ? Colors.Green : Colors.Transparent;

        //var brush = new LinearGradientBrush
        //  {
        //    StartPoint = new Point(1, 0.3),
        //    EndPoint = new Point(1, 0.7),
        //    Opacity = opacity
        //  };

        //brush.GradientStops.Add(new GradientStop {Color = clr1, Offset = 0.0});
        //brush.GradientStops.Add(new GradientStop {Color = clr2, Offset = 0.4});
        //brush.GradientStops.Add(new GradientStop {Color = clr3, Offset = 0.6});

        //return brush;
      }

    }

    public object ConvertBack(object value, Type targetType,
                              object parameter, CultureInfo culture)
    {
      return null;
    }
  }

}



