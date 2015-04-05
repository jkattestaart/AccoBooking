using System.ComponentModel.Composition;
using DomainModel;

#if HARNESS
using Common.SampleData;
#endif

namespace AccoBooking.ViewModels.General
{
  [Export(typeof(IBaseDetailSection<MailTemplate>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class MailTemplateSectionViewModel : BaseSectionViewModel<MailTemplate, MailTemplateSummaryViewModel>
  {
    [ImportingConstructor]
    public MailTemplateSectionViewModel(MailTemplateSummaryViewModel MailTemplateSummary)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = "Template";
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = MailTemplateSummary;
    }

    #region IMailTemplateDetailSection Members

    public override int Index
    {
      get { return 10; }
    }

    #endregion

    public override BaseSectionViewModel<MailTemplate, MailTemplateSummaryViewModel> Start(int mailtemplateid)
    {
      ActivateItem(Section.Start(mailtemplateid));
      return base.Start(mailtemplateid);
    }
  }
}