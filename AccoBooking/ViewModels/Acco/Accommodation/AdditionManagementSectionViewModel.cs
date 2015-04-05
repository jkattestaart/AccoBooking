using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class AdditionManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoAdditionManagementViewModel>
  {
    [ImportingConstructor]
    public AdditionManagementSectionViewModel(AccoAdditionManagementViewModel additionManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_ADDITION;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section  = additionManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 30; }
    }

    #endregion


    public override BaseSectionViewModel<DomainModel.Acco, AccoAdditionManagementViewModel> Start(int accoId)
    {
      ActivateItem(Section.Start(accoId));
      return base.Start(accoId);
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoAdditionManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(Section.Start(acco)); nog niet ondersteund/nodig
      return base.Start(acco);
    }
    
  }
}