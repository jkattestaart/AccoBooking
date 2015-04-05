using System.Threading;
using System.Windows.Controls;

namespace AccoBooking.Views.Booking
{
  public partial class SelectBookingView : UserControl
  {
    public SelectBookingView()
    {
      InitializeComponent();

      // @@@ JKT Bug in SL4 language,  not set in dialoghost
      Language = System.Windows.Markup.XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

    }
  }
}
