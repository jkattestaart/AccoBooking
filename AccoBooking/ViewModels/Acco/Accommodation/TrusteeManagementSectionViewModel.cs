using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class TrusteeManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoTrusteeManagementViewModel>
  {
    [ImportingConstructor]
    public TrusteeManagementSectionViewModel(AccoTrusteeManagementViewModel TrusteeManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_TRUSTEE;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = TrusteeManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 80; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, AccoTrusteeManagementViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return base.Start(accoid);
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoTrusteeManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(ActivityManagement.Start(acco));
      return base.Start(acco);
    }


    
  }
}