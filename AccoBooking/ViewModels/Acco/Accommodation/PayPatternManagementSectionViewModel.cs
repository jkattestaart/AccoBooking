using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class PayPatternManagementSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoPayPatternManagementViewModel>
  {
    [ImportingConstructor]
    public PayPatternManagementSectionViewModel(AccoPayPatternManagementViewModel payPatternManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_PAY_PATTERN;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = payPatternManagement;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 70; }
    }

    #endregion

    public override  BaseSectionViewModel<DomainModel.Acco, AccoPayPatternManagementViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return base.Start(accoid); ;
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoPayPatternManagementViewModel> Start(DomainModel.Acco acco)
    {
      //ActivateItem(Section.Start(acco));
      return base.Start(acco);
    }


    
  }
}