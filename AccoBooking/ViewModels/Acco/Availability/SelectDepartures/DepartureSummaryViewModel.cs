using System.ComponentModel.Composition;
using AccoBooking.ViewModels.Booking;
using Cocktail;
using DomainModel;
using DomainServices;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class DepartureSummaryViewModel : BaseScreen<AccoRent>
  {
    private ShellViewModel _shellViewModel;
    private ExportFactory<SendMailViewModel> _sendmailFactory;
    private ExportFactory<PreviewMailViewModel> _previewmailFactory;

    [ImportingConstructor]
    public DepartureSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                               ExportFactory<PreviewMailViewModel> previewmailFactory,
                                               ExportFactory<SendMailViewModel> sendmailFactory,
                                               ShellViewModel shellViewModel,
                                               IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      _previewmailFactory = previewmailFactory; 
      _sendmailFactory = sendmailFactory;
      _shellViewModel = shellViewModel;

    }

    //public IEnumerable<IResult> Propose()
    //{
    //  SendMailViewModel.Context = "BOOKING";
    //  var sendMail = _sendmailFactory.CreatePart();
    //  yield return DialogManager.ShowDialogAsync(sendMail, DialogButtons.OkCancel);

    //  yield return UnitOfWork.Accoes.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoId, res => SessionManager.CurrentAcco = res);

    //  PreviewMailViewModel.Acco = SessionManager.CurrentAcco;
    //  //PreviewMailViewModel.Booking = (DomainModel.Booking)Entity;
    //  PreviewMailViewModel.TemplateId = sendMail.MailTemplateList.ItemId;
    //  PreviewMailViewModel.LanguageId = sendMail.LanguageList.ItemId;
    //  PreviewMailViewModel.From = SessionManager.CurrentAcco.AccoOwner.PublicEmail;
    //  //PreviewMailViewModel.To = ((DomainModel.Booking)Entity).BookingGuest.Email;
    //  yield return _previewmailFactory.CreatePart();  
    //}


    protected override IRepository<AccoRent> Repository()
    {
      return UnitOfWork.AccoRents;
    }

  }
}