using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class RentManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoRentManagementViewModel>
  {
    [ImportingConstructor]
    public RentManagementSectionViewModel(AccoRentManagementViewModel rentManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_RENT;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = rentManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 50; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, AccoRentManagementViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return base.Start(accoid);
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoRentManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(Section.Start(acco));
      return base.Start(acco);
    }


    
  }
}