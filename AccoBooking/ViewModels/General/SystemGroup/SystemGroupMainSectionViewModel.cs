using System.ComponentModel.Composition;
using DomainModel;

#if HARNESS
using Common.SampleData;
#endif

namespace AccoBooking.ViewModels
{
  [Export(typeof(IBaseDetailSection<SystemGroup>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemGroupMainSectionViewModel : BaseSectionViewModel<SystemGroup, SystemGroupSummaryViewModel>
  {
    [ImportingConstructor]
    public SystemGroupMainSectionViewModel(SystemGroupSummaryViewModel systemGroupSummary)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = "Group";
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = systemGroupSummary;
    }

    #region ISystemGroupDetailSection Members

    public override int Index
    {
      get { return 10; }
    }

    #endregion

    public override BaseSectionViewModel<SystemGroup, SystemGroupSummaryViewModel> Start(int groupid)
    {
      ActivateItem(Section.Start(groupid));
      return this;
    }
  }
}