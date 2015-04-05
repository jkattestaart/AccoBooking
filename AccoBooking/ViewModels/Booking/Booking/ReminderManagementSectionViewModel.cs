using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Booking
{
  [Export(typeof(IBaseDetailSection<DomainModel.Booking>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class ReminderManagementSectionViewModel : BaseSectionViewModel<DomainModel.Booking, BookingReminderManagementViewModel>
  {
    [ImportingConstructor]
    public ReminderManagementSectionViewModel(BookingReminderManagementViewModel reminderManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_REMINDER;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = reminderManagement;
    }
    
    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 30; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Booking, BookingReminderManagementViewModel> Start(int bookingId)
    {
      ActivateItem(Section.Start(bookingId));
      return base.Start(bookingId);
    }

    public override BaseSectionViewModel<DomainModel.Booking, BookingReminderManagementViewModel> Start(DomainModel.Booking booking)
    {
      //ActivateItem(ReminderManagement.Start(booking));
      return base.Start(booking);
    }


    
  }
}