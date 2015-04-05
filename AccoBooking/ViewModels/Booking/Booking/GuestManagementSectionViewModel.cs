using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Booking
{
  [Export(typeof(IBaseDetailSection<DomainModel.Booking>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class GuestManagementSectionViewModel : BaseSectionViewModel<DomainModel.Booking, BookingGuestManagementViewModel>
  {
    [ImportingConstructor]
    public GuestManagementSectionViewModel(BookingGuestManagementViewModel guestManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_GUEST;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = guestManagement;
    }
    
    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 15; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Booking, BookingGuestManagementViewModel> Start(int bookingId)
    {
      ActivateItem(Section.Start(bookingId));
      return base.Start(bookingId);
    }

    public override BaseSectionViewModel<DomainModel.Booking, BookingGuestManagementViewModel> Start(DomainModel.Booking booking)
    {
      //ActivateItem(ReminderManagement.Start(booking));
      return base.Start(booking);
    }


    
  }
}