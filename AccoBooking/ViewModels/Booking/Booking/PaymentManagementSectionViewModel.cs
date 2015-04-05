using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Booking
{
  [Export(typeof(IBaseDetailSection<DomainModel.Booking>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class PaymentManagementSectionViewModel : BaseSectionViewModel<DomainModel.Booking, BookingPaymentSummaryViewModel>
  {
    [ImportingConstructor]
    public PaymentManagementSectionViewModel(BookingPaymentSummaryViewModel bookingpaymentsummary)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_PAYMENTS;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      
      Section = bookingpaymentsummary;
    }

    #region IRelationDetailSection Members

    public override int Index
    {
      get { return 40; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Booking, BookingPaymentSummaryViewModel> Start(int bookingId)
    {
      ActivateItem(Section.Start(bookingId));
      return this;
    }

    public override BaseSectionViewModel<DomainModel.Booking, BookingPaymentSummaryViewModel> Start(DomainModel.Booking booking)
    {
      //ActivateItem(Section.Start(booking));
      return this;
    }


    
  }
}