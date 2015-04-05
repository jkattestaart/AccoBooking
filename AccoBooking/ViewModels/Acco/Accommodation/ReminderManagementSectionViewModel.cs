using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class ReminderManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoReminderManagementViewModel>
  {
    [ImportingConstructor]
    public ReminderManagementSectionViewModel(AccoReminderManagementViewModel reminderManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_REMINDER;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = reminderManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 40; }
    }
    
    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, AccoReminderManagementViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return base.Start(accoid);
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoReminderManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(Section.Start(acco));
      return base.Start(acco);
    }


    
  }
}