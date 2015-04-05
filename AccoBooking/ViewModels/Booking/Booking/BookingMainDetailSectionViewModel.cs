using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Booking
{
  [Export(typeof(IBaseDetailSection<DomainModel.Booking>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingMainDetailSectionViewModel : BaseSectionViewModel<DomainModel.Booking, BookingSummaryViewModel>
  {
      // meer commentaar

    [ImportingConstructor]
    public BookingMainDetailSectionViewModel(BookingSummaryViewModel bookingSummary)
    {
 
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_BOOKING;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = bookingSummary;
      Section.Parent = this;
    }

    #region IRelationDetailSection Members

    public override int Index
    {
      get { return 10; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Booking, BookingSummaryViewModel> Start(int bookingId)
    {
      ActivateItem(Section.Start(bookingId));
      return this;
    }

    public override BaseSectionViewModel<DomainModel.Booking, BookingSummaryViewModel> Start(DomainModel.Booking booking)
    {
      ActivateItem(Section.Start(booking));
      return this;
    }

  }
}
