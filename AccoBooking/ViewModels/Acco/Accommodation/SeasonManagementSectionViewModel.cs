using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class SeasonManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoSeasonManagementViewModel>
  {
    [ImportingConstructor]
    public SeasonManagementSectionViewModel(AccoSeasonManagementViewModel rentManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_SEASON;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = rentManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 60; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, AccoSeasonManagementViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return base.Start(accoid);
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoSeasonManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(Section.Start(acco));
      return base.Start(acco);
    }


    
  }
}