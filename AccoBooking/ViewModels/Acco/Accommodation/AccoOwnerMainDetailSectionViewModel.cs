using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof (IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoOwnerMainDetailSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoOwnerSummaryViewModel>
  {
    [ImportingConstructor]
    public AccoOwnerMainDetailSectionViewModel(AccoOwnerSummaryViewModel accoOwnerSummary)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_OWNER;
      // ReSharper restore DoNotCallOverridableOwnerMethodsInConstructor
      Section = accoOwnerSummary;

    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 0; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, AccoOwnerSummaryViewModel> Start(int accoId)
    {
      ActivateItem(Section.Start(accoId));
      return this;
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoOwnerSummaryViewModel> Start(DomainModel.Acco acco)
    {
      ActivateItem(Section.Start(acco));
      return this;
    }

    
  }
}