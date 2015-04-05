using System.ComponentModel.Composition;
using AccoBooking.ViewModels.General;
using Cocktail;
using DomainModel;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SendMailViewModel : BaseScreen<DomainModel.Acco>
  {
    private ExportFactory<PreviewMailViewModel> _previewmailFactory;
    private int _templateid;
    private int _languageid;

    public static string Context;
    private ShellViewModel _shellViewModel;

    [ImportingConstructor]
    public SendMailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                             ExportFactory<PreviewMailViewModel> previewmailFactory,
                             SystemCodeListViewModel mailcontextlist,
                             MailTemplateListViewModel mailtemplatelist,
                             LanguageListViewModel languagelist,
                             ShellViewModel shellViewModel,
                             IDialogManager dialogManager
      )
      : base(unitOfWorkManager, dialogManager)
    {
      _shellViewModel = shellViewModel;
      _previewmailFactory = previewmailFactory;

      MailContextList = mailcontextlist;
      MailContextList.ShortName = Context;
      MailContextList.PropertyChanged += MailContextListPropertyChanged;

      MailTemplateList = mailtemplatelist;
      MailTemplateList.PropertyChanged += MailTemplateListPropertyChanged;
      
      LanguageList = languagelist;
      LanguageList.PropertyChanged += LanguageListPropertyChanged;

      MailTemplateList.ItemId = PreviewMailViewModel.TemplateId;
      LanguageList.ItemId = PreviewMailViewModel.LanguageId;
    }

    void LanguageListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        _languageid = MailTemplateList.ItemId;
        MailTemplateList.LanguageId = _languageid;
      }
    }

    void MailTemplateListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
         _templateid = MailTemplateList.ItemId;
      }
    }

    private void MailContextListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      MailTemplateList.MailContext = MailContextList.ShortName;
    }

    public SystemCodeListViewModel MailContextList { get; set; }
    public MailTemplateListViewModel MailTemplateList { get; set; }
    public LanguageListViewModel LanguageList { get; set; }

    protected override IRepository<DomainModel.Acco> Repository()
    {
      return UnitOfWork.Accoes;
    }

    
    protected override void OnActivate()
    {
      base.OnActivate();
      _shellViewModel.BuildMenu("");
      MailContextList.Start(SystemGroupName.MailContext);
      MailContextList.IsEnabled = false;
      MailTemplateList.Start(0);
      LanguageList.Start(0);

      //trigger the lazy load of AccoOwner for later use
      PreviewMailViewModel.From = SessionManager.CurrentOwner.PublicEmail;
    }

    public async void Send() 
    {
      if (MailTemplateList.ItemId == 0)
        await DialogManager.ShowMessageAsync(Resources.AccoBooking.mes_NO_TEMPLATE_SELECTED, DialogButtons.Ok);
      else if (LanguageList.ItemId == 0)
        await DialogManager.ShowMessageAsync(Resources.AccoBooking.mes_NO_LANGUAGE_SELECTED, DialogButtons.Ok);
      else
      {

        PreviewMailViewModel.From = SessionManager.CurrentOwner.PublicEmail;

        PreviewMailViewModel.TemplateId = MailTemplateList.ItemId;
        PreviewMailViewModel.LanguageId = LanguageList.ItemId;

        SessionManager.CurrentAcco = await UnitOfWork.Accoes.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoId);

        await PreviewMailViewModel.SelectMailTemplateContent(MailTemplateList.ItemId, LanguageList.ItemId);

        (Parent as SendMailWizardViewModel).Send();
        TryClose();
      }

    }

    public void Cancel()
    {
      (Parent as SendMailWizardViewModel).Cancel();
      TryClose();
    }

  }

}
