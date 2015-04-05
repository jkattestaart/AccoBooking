using System.ComponentModel.Composition;
using DomainModel;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<AccoPayPattern>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoPayPatternPaymentSectionViewModel : BaseSectionViewModel<AccoPayPattern, AccoPayPatternPaymentManagementViewModel>
  {
    [ImportingConstructor]
    // public SystemGroupCodesSectionViewModel(SystemCodeManagementViewModel systemCodeManagement)

    public AccoPayPatternPaymentSectionViewModel(AccoPayPatternPaymentManagementViewModel accoPayPatternPaymentManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_PAYMENTS;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = accoPayPatternPaymentManagement;
    }

    #region IAccoPayPatternPaymentDetailSection Members

    public override int Index
    {
      get { return 20; }
    }
    #endregion

    public override BaseSectionViewModel<AccoPayPattern, AccoPayPatternPaymentManagementViewModel> Start(int paypatternid)
    {
      ActivateItem(Section.Start(paypatternid));
      return base.Start(paypatternid);
    }
  }
}