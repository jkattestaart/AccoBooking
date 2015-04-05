using System.ComponentModel.Composition;
using DomainModel;

#if HARNESS
using Common.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<AccoPayPattern>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoPayPatternSectionViewModel : BaseSectionViewModel<AccoPayPattern, AccoPayPatternSummaryViewModel>
  {
    [ImportingConstructor]
    public AccoPayPatternSectionViewModel(AccoPayPatternSummaryViewModel accoPayPatternSummary)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_PATTERN;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = accoPayPatternSummary;
    }

    #region IAccoPayPatternDetailSection Members

    public override int Index
    {
      get { return 10; }
    }

    #endregion

    public override BaseSectionViewModel<AccoPayPattern, AccoPayPatternSummaryViewModel>  Start(int paypatternid)
    {
      ActivateItem(Section.Start(paypatternid));
      return base.Start(paypatternid);
    }
  }
}