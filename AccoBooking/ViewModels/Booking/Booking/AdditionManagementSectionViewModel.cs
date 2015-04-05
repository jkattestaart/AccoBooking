using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Booking
{
  [Export(typeof (IBaseDetailSection<DomainModel.Booking>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class AdditionManagementSectionViewModel : BaseSectionViewModel<DomainModel.Booking, BookingAdditionManagementViewModel>
  {
    [ImportingConstructor]
    public AdditionManagementSectionViewModel(BookingAdditionManagementViewModel additionManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_ADDITION;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = additionManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 20; }
    }

    #endregion
    
    public override BaseSectionViewModel<DomainModel.Booking, BookingAdditionManagementViewModel> Start(int bookingId)
    {
      ActivateItem((Section).Start(bookingId));
      return base.Start(bookingId);
    }


    public override BaseSectionViewModel<DomainModel.Booking, BookingAdditionManagementViewModel> Start(DomainModel.Booking booking)
    {
      //ActivateItem(AdditionManagement.Start(booking)); nog niet ondersteund/nodig
      return base.Start(booking);
    }
  }
}
