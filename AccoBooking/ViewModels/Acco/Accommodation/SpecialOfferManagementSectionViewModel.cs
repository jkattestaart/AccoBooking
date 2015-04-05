using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class SpecialOfferManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoSpecialOfferManagementViewModel>
  {
    [ImportingConstructor]
    public SpecialOfferManagementSectionViewModel(AccoSpecialOfferManagementViewModel specialOfferManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_SPECIAL_OFFER;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = specialOfferManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 90; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, AccoSpecialOfferManagementViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return base.Start(accoid);
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoSpecialOfferManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(ActivityManagement.Start(acco));
      return base.Start(acco);
    }


    
  }
}