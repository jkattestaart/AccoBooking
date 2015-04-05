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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using AccoBooking.ViewModels.General;
using Cocktail;
using DomainModel;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingSummaryViewModel : BaseScreen<DomainModel.Booking>
  {
    //private IPartFactory<SendMailViewModel> _sendmailFactory;
    //private IPartFactory<PreviewMailViewModel> _previewmailFactory;
    private PreviewMailViewModel _previewMail;
    private ExportFactory<SelectBookingViewModel> _selectBookingFactory;

    [ImportingConstructor]
    public BookingSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                   ExportFactory<SelectBookingViewModel> selectBookingFactory,
                                   SystemCodeListViewModel bookingStatusList,
                                   CountryListViewModel countryList,
                                   LanguageListViewModel languageList,
                                   PreviewMailViewModel previewMail,
                                   IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      _previewMail = previewMail;
      _selectBookingFactory = selectBookingFactory;
      BookingStatusList = bookingStatusList;

      CountryList = countryList;
      CountryList.PropertyChanged += CountryListOnPropertyChanged;

      LanguageList = languageList;
      LanguageList.PropertyChanged += LanguageListOnPropertyChanged;
    }

    private void CountryListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((DomainModel.Booking)Entity).BookerCountryId = CountryList.ItemId;
        }
      }
    }

    private void LanguageListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((DomainModel.Booking)Entity).BookerLanguageId = LanguageList.ItemId;
        }
        SessionManager.BookingLanguage(LanguageList.ItemId);
      }
    }

    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(sender, e);
      UpdateButtons();
    }
    
    public CountryListViewModel CountryList { get; set; }

    public LanguageListViewModel LanguageList { get; set; }

    public SystemCodeListViewModel BookingStatusList { get; set; }

    public bool ButtonsVisible { get; set; }
    public override Entity Entity
    {
      get
      {
        return base.Entity;
      }
      set
      {
        base.Entity = value;

        if (Entity != null)
        {
          if (Entity.EntityAspect.EntityState.IsAdded())
          {
            ((DomainModel.Booking)Entity).Arrival = CreateBookingViewModel.Departure.Arrival;
            ((DomainModel.Booking)Entity).Departure = CreateBookingViewModel.Departure.Departure;
            ((DomainModel.Booking)Entity).Rent = CreateBookingViewModel.Departure.Rent;
            ((DomainModel.Booking)Entity).Deposit = SessionManager.CurrentAcco.Deposit;
            ButtonsVisible = false;
          }
          else
          {
            ButtonsVisible = true;
          }
          CountryList.ItemId = ((DomainModel.Booking)Entity).BookerCountryId;
          LanguageList.ItemId = ((DomainModel.Booking)Entity).BookerLanguageId;
          BookingStatusList.ShortName = ((DomainModel.Booking) Entity).Status;

          //set bookinglanguage
          SessionManager.BookingLanguage(((DomainModel.Booking)Entity).BookerLanguageId);
          SessionManager.BookingAccoId = ((DomainModel.Booking)Entity).AccoId;

          UpdateButtons();
        }
      }
    }

    public bool IsReadOnly
    {
      get
      {
        if (Entity == null)
          return false;

        switch (((DomainModel.Booking)Entity).Status)
        {
          case "EXPIRED":
            return true;

          case "CANCELLED":
            return true;

          case "CLOSED":
            return true;

          default:
            return false; // RESERVED or CONFIRMED

        }
      }
    }

    public async void Copy()
    {
      //dialoghost met bookingsearch 
      var selectBooking = _selectBookingFactory.CreateExport().Value;
      selectBooking.Start();
      var result = await selectBooking.ShowDialogAsync();
      if (result == DialogResult.Ok)
      {
        var unitofWork = DomainUnitOfWorkManager.Create();
        var copy = await unitofWork.Bookings.WithIdFromDataSourceAsync(selectBooking.CurrentItem.Id);
        var booking = Entity as DomainModel.Booking;


        for (int i = 0; i < copy.BookingGuests.Count; i++)
        {
          var guest = await UnitOfWork.BookingGuestFactory.CreateAsync();            //op dezelfde unit of work als booking
          guest.BookingId = booking.BookingId;
          SessionManager.Clone(copy.BookingGuests[i], guest);
        }

        booking.Booker = copy.Booker;
        booking.BookerEmail = copy.BookerEmail;
        booking.BookerPhone = copy.BookerPhone;
        booking.BookerMobile = copy.BookerMobile;
        booking.BookerAddress1 = copy.BookerAddress1;
        booking.BookerAddress2 = copy.BookerAddress2;
        booking.BookerAddress3 = copy.BookerAddress3;
        booking.BookerLanguageId = copy.BookerLanguageId;
        booking.BookerCountryId = copy.BookerCountryId;
        booking.BookerBank = copy.BookerBank;
        booking.BookerBankAccount = copy.BookerBankAccount;
        Entity = booking;

        SessionManager.BookingLanguage(LanguageList.ItemId);
        NotifyOfPropertyChange(()=>Entity);

      }
      //StaffingResource.FirstName = nameEditor.FirstName;
      //StaffingResource.MiddleName = nameEditor.MiddleName;
      //StaffingResource.LastName = nameEditor.LastName;

      //var bookingParent = (Parent as CreateBookingDetailViewModel).Parent;
      //(bookingParent as BookingManagementViewModel).Copy();
    }

    public async void Confirm()
    {
      DomainModel.Acco acco = null;

      var parentdetail = (Parent as BaseSectionViewModel<DomainModel.Booking, BookingSummaryViewModel>).Parent;
      
      object bookingParent = null;
      object checkReminderParent = null;
      
      if (parentdetail.GetType() == typeof (BookingDetailViewModel))
        bookingParent = (parentdetail as BookingDetailViewModel).Parent;
      else
        checkReminderParent = (parentdetail as CheckRemindersDetailViewModel).Parent;

      SendMailViewModel.Context = "BOOKING";
      
      var entity = (DomainModel.Booking) Entity; //(parent as BookingManagementViewModel).ActiveEntity as DomainModel.Booking;
      entity.IsConfirmed = true;

      //if (((DomainModel.Booking)Entity).Status != "PAID") 
      //  ((DomainModel.Booking)Entity).Status = "CONFIRMED";
      //((DomainModel.Booking)Entity).StatusUpdate = DateTime.Now;
      //((DomainModel.Booking)Entity).Confirmed = DateTime.Now;

      using (Busy.GetTicket())
      {
        if (checkReminderParent != null)
          await (checkReminderParent as CheckRemindersManagementViewModel).SaveAsync(false);
        else
          await (bookingParent as BookingManagementViewModel).SaveAsync(false);

        var unitofWork = DomainUnitOfWorkManager.Create();

        acco = await unitofWork.Accoes.WithIdFromDataSourceAsync(((DomainModel.Booking) Entity).AccoId);

        await PreviewMailViewModel.SelectMailTemplateContent
          (MailTemplateType.Confirmation,
            ((DomainModel.Booking)Entity).BookerLanguageId
          );
      }
      PreviewMailViewModel.Acco = acco;
      PreviewMailViewModel.Booking = (DomainModel.Booking)Entity;

      PreviewMailViewModel.From = SessionManager.CurrentOwner.PublicEmail;
      PreviewMailViewModel.To = ((DomainModel.Booking)Entity).BookerEmail;

      if (checkReminderParent != null)
        (checkReminderParent as CheckRemindersManagementViewModel).SendMail();
      else
        (bookingParent as BookingManagementViewModel).SendMail();

    }

    public bool CanConfirm
    {
      get
      {
        //return ((DomainModel.Booking)Entity).Status != "CLOSED" && !((DomainModel.Booking)Entity).IsConfirmed;
        return !((DomainModel.Booking)Entity).IsConfirmed;
      }
    }

    public async void Expire()
    {
      DomainModel.Acco acco = null;
      var parentdetail = (Parent as BaseSectionViewModel<DomainModel.Booking, BookingSummaryViewModel>).Parent;

      object bookingParent = null;
      object checkReminderParent = null;

      if (parentdetail.GetType() == typeof(BookingDetailViewModel))
        bookingParent = (parentdetail as BookingDetailViewModel).Parent;
      else
        checkReminderParent = (parentdetail as CheckRemindersDetailViewModel).Parent;

      SendMailViewModel.Context = "BOOKING";

      ((DomainModel.Booking)Entity).Status = "EXPIRED";
      ((DomainModel.Booking)Entity).StatusUpdate = DateTime.Now;

      using (Busy.GetTicket())
      {
        if (checkReminderParent != null)
          await (checkReminderParent as CheckRemindersManagementViewModel).SaveAsync(false);
        else
          await (bookingParent as BookingManagementViewModel).SaveAsync(false);

        var unitofWork = DomainUnitOfWorkManager.Create();

        acco = await unitofWork.Accoes.WithIdFromDataSourceAsync(((DomainModel.Booking) Entity).AccoId);

        await PreviewMailViewModel.SelectMailTemplateContent
          (MailTemplateType.Expiration,
            ((DomainModel.Booking)Entity).BookerLanguageId
          );
      }

      PreviewMailViewModel.Acco = acco;
      PreviewMailViewModel.Booking = (DomainModel.Booking)Entity;

      PreviewMailViewModel.From = SessionManager.CurrentAcco.AccoOwner.PublicEmail;
      PreviewMailViewModel.To = ((DomainModel.Booking)Entity).BookerEmail;

      if (checkReminderParent != null)
        (checkReminderParent as CheckRemindersManagementViewModel).StartMail();
      else
        (bookingParent as BookingManagementViewModel).StartMail();

    }

    public async void Mail()
    {
      DomainModel.Acco acco = null;
      var parentdetail = (Parent as BaseSectionViewModel<DomainModel.Booking, BookingSummaryViewModel>).Parent;

      object bookingParent = null;
      object checkReminderParent = null;

      if (parentdetail.GetType() == typeof(BookingDetailViewModel))
        bookingParent = (parentdetail as BookingDetailViewModel).Parent;
      else
        checkReminderParent = (parentdetail as CheckRemindersDetailViewModel).Parent;

      SendMailViewModel.Context = "BOOKING";

      IEnumerable<Language> languages = new Collection<Language>();

      acco = await UnitOfWork.Accoes.WithIdFromDataSourceAsync(((DomainModel.Booking) Entity).AccoId);

      languages = await UnitOfWork.Languages.FindInDataSourceAsync(
        l => l.LanguageId == ((DomainModel.Booking)Entity).BookerLanguageId, CancellationToken.None);

      PreviewMailViewModel.Acco = acco;
      PreviewMailViewModel.Booking = (DomainModel.Booking) Entity;
      //PreviewMailViewModel.TemplateId = sendMail.MailTemplateList.ItemId;
      if (languages.FirstOrDefault() != null)
        PreviewMailViewModel.LanguageId = languages.FirstOrDefault().LanguageId;

      PreviewMailViewModel.From = SessionManager.CurrentOwner.PublicEmail;
      PreviewMailViewModel.To = ((DomainModel.Booking)Entity).BookerEmail;

      if (checkReminderParent != null)
        (checkReminderParent as CheckRemindersManagementViewModel).StartMail();
      else
        (bookingParent as BookingManagementViewModel).StartMail();

      // yield return _previewmailFactory.CreatePart();

    }

    public bool CanExpire
    {
      get { return ((DomainModel.Booking) Entity).Status == BookingStatus.Reserved; }
    }

    public async void Cancel()
    {
      var dialogresult = await DialogManager.ShowMessageAsync(Resources.AccoBooking.mes_CANCEL_BOOKING, new[] { Resources.AccoBooking.but_YES, Resources.AccoBooking.but_NO });
      //DialogResult.Yes, DialogResult.No, DialogButtons.YesNo);
      if (dialogresult == Resources.AccoBooking.but_NO)
        return;

      DomainModel.Acco acco = null;

      var parentdetail = (Parent as BaseSectionViewModel<DomainModel.Booking, BookingSummaryViewModel>).Parent;

      object bookingParent = null;
      object checkReminderParent = null;

      if (parentdetail.GetType() == typeof(BookingDetailViewModel))
        bookingParent = (parentdetail as BookingDetailViewModel).Parent;
      else
        checkReminderParent = (parentdetail as CheckRemindersDetailViewModel).Parent;

      SendMailViewModel.Context = "BOOKING";

      ((DomainModel.Booking)Entity).Status = "CANCELLED";
      ((DomainModel.Booking)Entity).StatusUpdate = DateTime.Now;

      using (Busy.GetTicket())
      {
        if (checkReminderParent != null)
          await (checkReminderParent as CheckRemindersManagementViewModel).SaveAsync(false);
        else
          await (bookingParent as BookingManagementViewModel).SaveAsync(false);
        
        var unitofWork = DomainUnitOfWorkManager.Create();

        acco = await unitofWork.Accoes.WithIdFromDataSourceAsync(((DomainModel.Booking) Entity).AccoId);

       await PreviewMailViewModel.SelectMailTemplateContent
          (MailTemplateType.Cancellation,
            ((DomainModel.Booking)Entity).BookerLanguageId
          );
      }

      PreviewMailViewModel.Acco = acco;
      PreviewMailViewModel.Booking = (DomainModel.Booking)Entity;

      PreviewMailViewModel.From = SessionManager.CurrentOwner.PublicEmail;
      PreviewMailViewModel.To = ((DomainModel.Booking)Entity).BookerEmail;


      if (checkReminderParent != null)
        (checkReminderParent as CheckRemindersManagementViewModel).StartMail();
      else
        (bookingParent as BookingManagementViewModel).StartMail();

      
    }

    public bool CanCancel
    {
      get
      {
        return (((DomainModel.Booking) Entity).IsConfirmed)
               &&
               (((DomainModel.Booking) Entity).Status == BookingStatus.Confirmed ||
                 (((DomainModel.Booking) Entity).Status == BookingStatus.Paid));
      }
    }  

    void UpdateButtons()
    {
      NotifyOfPropertyChange(() => ButtonsVisible);
      NotifyOfPropertyChange(() => CanConfirm);
      NotifyOfPropertyChange(() => CanExpire);
      NotifyOfPropertyChange(() => CanCancel);
      NotifyOfPropertyChange(() => IsReadOnly);
    }

    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      BookingStatusList.Start("BOOKINGSTATUS");
      CountryList.Start(0);
      LanguageList.Start(0);
      GasUnit = SystemCodeService.SystemCodeList.FirstOrDefault(s => s.SystemGroup.Name == "UNIT" && s.Code == "GASM3").Description;
      WaterUnit = SystemCodeService.SystemCodeList.FirstOrDefault(s => s.SystemGroup.Name == "UNIT" && s.Code == "WATERM3").Description;
      ElectricityUnit = SystemCodeService.SystemCodeList.FirstOrDefault(s => s.SystemGroup.Name == "UNIT" && s.Code == "KWH").Description;
    }

    public string GasUnit { get; set; }
    public string WaterUnit { get; set; }
    public string ElectricityUnit { get; set; }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        CountryList.PropertyChanged -= CountryListOnPropertyChanged;
        LanguageList.PropertyChanged -= LanguageListOnPropertyChanged;
      }
    }

   
  }
}