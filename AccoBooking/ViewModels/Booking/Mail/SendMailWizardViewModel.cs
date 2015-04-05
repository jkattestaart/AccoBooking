//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Common.Messages;
using DomainServices;

namespace AccoBooking.ViewModels.Booking
{

  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SendMailWizardViewModel : Conductor<IScreen>,
                                         IDiscoverableViewModel,
                                         IHandle<EntityChangedMessage>
  {
    private readonly ExportFactory<PreviewMailViewModel> _previewModelFactory;
    private readonly ExportFactory<SendMailViewModel> _sendMailFactory;
    private readonly INavigator _navigatorPreviewMailService;
    private readonly INavigator _navigatorSendMailService;

    [ImportingConstructor]
    public SendMailWizardViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                   ExportFactory<PreviewMailViewModel> previewMailFactory,
                                   ExportFactory<SendMailViewModel> sendMailFactory,
                                   IDialogManager dialogManager)
    {
      _previewModelFactory = previewMailFactory;
      _sendMailFactory = sendMailFactory;
     _navigatorPreviewMailService = new Navigator(this);
      _navigatorSendMailService = new Navigator(this);
    }

    public PreviewMailViewModel PreviewMail { get; set; }
    public SendMailViewModel SendMail { get; set; }

    #region IHandle<EntityChangedMessage> Members

    /// <summary>
    /// If there are no messsages update the comamnd buttons
    /// </summary>
    /// <param name="message">melding die ontstaan is</param>
    public void Handle(EntityChangedMessage message)
    {
      //if (ActiveEntity == null || !ActiveUnitOfWork.HasEntity(message.Entity))
      //  return;
      //UpdateCommands();
    }

    #endregion

    protected override void OnActivate()
    {
      base.OnActivate();

      Start();
    }

    public void Cancel()
    {
      if (Parent.GetType() == typeof(ProposeViewModel))
        (Parent as ProposeViewModel).Cancel();
      if (Parent.GetType() == typeof(BookingMailViewModel))
        (Parent as BookingMailViewModel).Cancel();
      TryClose();
    }

    public void Send()
    {
      var preview = PreviewMail ?? _previewModelFactory.CreateExport().Value;

      _navigatorPreviewMailService.NavigateToAsync(preview.GetType(),
                                               target =>
                                               {
                                                 if (PreviewMail != target)
                                                   PreviewMail = target as PreviewMailViewModel;
                                                 PreviewMail.Parent = this;
                                                 //PreviewMailViewModel.Acco = SessionManager.CurrentAcco;
                                                 ////PreviewMailViewModel.Booking = (DomainModel.Booking)Entity;
                                                 //PreviewMailViewModel.TemplateId = SendMail.MailTemplateList.ItemId;
                                                 //PreviewMailViewModel.LanguageId = SendMail.LanguageList.ItemId;
                                                 //PreviewMailViewModel.From = SessionManager.CurrentAcco.AccoOwner.PublicEmail;
                                                 ////PreviewMailViewModel.To = ((DomainModel.Booking)Entity).BookingGuest.Email;
                                                 ////yield return _previewmailFactory.CreatePart();
                                                 ((IActivate)target).Activate();
                                               }
        );
    }

    public void Start()
    {
      var sendmail = SendMail ?? _sendMailFactory.CreateExport().Value;

      _navigatorSendMailService.NavigateToAsync(sendmail.GetType(),
                                               target =>
                                               {
                                                 if (SendMail != target)
                                                   SendMail = target as SendMailViewModel;
                                                 SendMail.Parent = this;
                                                 ((IActivate)target).Activate();
                                               }
        );
    }

    //void BookingSelected(object sender, EventArgs e)
    //{
    //  AvailablePeriodList.AvailablePeriodSelected -= AvailablePeriodSelected;
    //  AvailablePeriodList.BookingSelected -= BookingSelected;

    //  _navigationBookingService.NavigateToAsync(() => BookingManagement ?? _bookingManagementFactory.CreatePart(),
    //                                           target =>
    //                                           {
    //                                             BookingManagement = target;
    //                                             target.Parent = this;  //Caliburn zet deze nu niet 
    //                                             ((IActivate)target).Activate();
    //                                             target.Start(AccoAvailablePeriodListViewModel.BookingId);
    //                                             //target.Completed += BookingManagementCompleted;
    //                                           }
    //    );

    //}


  }

}