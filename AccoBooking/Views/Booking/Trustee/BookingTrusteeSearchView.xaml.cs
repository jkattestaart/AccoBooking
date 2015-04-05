using System.Windows;
using System.Windows.Controls;
using AccoBooking.ViewModels.Booking;

namespace AccoBooking.Views.Booking
{
  public partial class BookingTrusteeSearchView : UserControl
  {
    private BookingTrusteeSearchViewModel _ctx;

    public BookingTrusteeSearchView()
    {
      InitializeComponent();
    }

    private void Dump_Click(object sender, RoutedEventArgs e)
    {
      _ctx = this.DataContext as BookingTrusteeSearchViewModel;

      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "CSV files|*.csv";
      saveFileDialog.ShowDialog();
      var stream = saveFileDialog.OpenFile();
      _ctx.DumpBookings(stream);
    }

  }
}
