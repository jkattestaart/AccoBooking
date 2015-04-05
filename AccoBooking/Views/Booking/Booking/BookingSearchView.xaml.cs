using System.Windows;
using System.Windows.Controls;
using AccoBooking.ViewModels.Booking;

namespace AccoBooking.Views.Booking
{
  public partial class BookingSearchView : UserControl
  {
    private BookingSearchViewModel _ctx;

    public BookingSearchView()
    {
      InitializeComponent();
    }

    private void Dump_Click(object sender, RoutedEventArgs e)
    {
      _ctx = this.DataContext as BookingSearchViewModel;

      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "CSV files|*.csv";
      saveFileDialog.ShowDialog();
      var stream = saveFileDialog.OpenFile();
      _ctx.DumpBookings(stream);
    }

  }
}
