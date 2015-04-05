using System.ComponentModel.Composition;
using DomainModel;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels
{
  [Export(typeof(IBaseDetailSection<SystemGroup>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemGroupCodesSectionViewModel : BaseSectionViewModel<SystemGroup, SystemCodeControlViewModel>
  {
    [ImportingConstructor]
    // public SystemGroupCodesSectionViewModel(SystemCodeManagementViewModel systemCodeManagement)

    public SystemGroupCodesSectionViewModel(SystemCodeControlViewModel systemCodeManagement)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = "Codes";
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = systemCodeManagement;
    }

    #region ISystemGroupDetailSection Members

    public override int Index
    {
      get { return 20; }
    }

    #endregion

    public override BaseSectionViewModel<SystemGroup, SystemCodeControlViewModel> Start(int groupid)
    {
      ActivateItem(Section.Start(groupid));
      //ActivateItem(SystemCodeManagement.Start());
      return this;
    }
  }
}