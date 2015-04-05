using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class CancelConditionManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoCancelConditionManagementViewModel>
  {
    [ImportingConstructor]
    public CancelConditionManagementSectionViewModel(AccoCancelConditionManagementViewModel cancelConditionManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_CANCEL_CONDITION;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = cancelConditionManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 100; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, AccoCancelConditionManagementViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return base.Start(accoid);
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoCancelConditionManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(ActivityManagement.Start(acco));
      return base.Start(acco);
    }


    
  }
}