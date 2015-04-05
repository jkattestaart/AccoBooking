using System;
using System.ComponentModel.Composition;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AccoBooking.ViewModels.Acco;
using AccoBooking.ViewModels.General;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Helper;
using DomainModel;
using DomainModel.Projections;
using DomainServices;
using DomainServices.Services;
using Security;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class PreviewMailViewModel : Screen
  {
    private IWindowManager _windowManager;
    private IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    private IErrorHandler _errorHandler;
    private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
    private decimal _totalRefund;
    private decimal _cancelRent;
    private string _subject;
    private string _contentBody;
    private string _currencySign;

    //variables for interaction between programs
    //public static string Body;
    public static string From;
    public static string To;
    //public static string Title;
    public static int TemplateId;
    public static int TemplateContentId;
    public static int LanguageId;

    public static DomainModel.Booking Booking;
    public static DomainModel.Acco Acco;
    public static AvailableDepartureListItem Departure;
    private IDialogManager _dialogManager;
    private ExportFactory<MailSettingsViewModel> _mailSettingsFactory;
    

    [ImportingConstructor]
    public PreviewMailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
      ExportFactory<MailSettingsViewModel> mailSettingsFactory,
      LanguageListViewModel languagelist,
      IWindowManager windowManager,
      IDialogManager dialogManager,
      IErrorHandler errorHandler
      )
    {
      _mailSettingsFactory = mailSettingsFactory;
      LanguageList = languagelist;
      // MailContentList.Start(TemplateId);
      LanguageList.PropertyChanged += LanguageListPropertyChanged;
      _unitOfWorkManager = unitOfWorkManager;
      _windowManager = windowManager;
      _dialogManager = dialogManager;
      _errorHandler = errorHandler;
    }

    private async void LanguageListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        LanguageId = LanguageList.ItemId;
        await SetContentLanguage(LanguageList.ItemId);
      }
    }

    public LanguageListViewModel LanguageList { get; set; }

    public bool ResetEmail { get; set; }

    public string Sender
    {
      get { return From; }
      set
      {
        From = value;
        UpdateTriggers();
      }
    }

    public string Addressee
    {
      get { return To; }
      set
      {
        To = value;
        UpdateTriggers();

      }
    }

    public string Subject
    {
      get { return _subject; }
      set
      {
        _subject = value;
        UpdateTriggers();
      }
    }

    //{
    //  get { return Title; }
    //  set
    //  {
    //    Title = value;
    //  }
    //}

    public string ContentBody
    {
      get { return _contentBody; }
      set
      {
        _contentBody = value;
        UpdateTriggers();
      }
    }

    //{
    //  get { return Body; }
    //  set
    //  {
    //    Body = value;
    //  }
    //}

    public bool CanSend
    {
      get
      {
        //return true;
        var r = new Regex(@"^[a-z0-9][\w\.-]*[a-z0-9]@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);

        return !String.IsNullOrEmpty(Sender) &&
        !String.IsNullOrEmpty(Addressee) &&
        !String.IsNullOrEmpty(Subject) &&
        !String.IsNullOrEmpty(ContentBody) &&
        r.Match(Addressee).Success;
      }
    }

    private void UpdateTriggers()
    {
      NotifyOfPropertyChange(() => Sender);
      NotifyOfPropertyChange(() => Addressee);
      NotifyOfPropertyChange(() => Subject);
      NotifyOfPropertyChange(() => ContentBody);
      NotifyOfPropertyChange(() => CanSend);
    }

    public async void Send()
    {
      await SendMail();
      Cancel();
    }

    public void Cancel()
    {
      if (Parent.GetType() == typeof(SendMailWizardViewModel))
        (Parent as SendMailWizardViewModel).Cancel();
      TryClose();
    }

    public async Task SendMail()
    {
      string user = "";
      string password = "";
      string provider = "";

      if (ResetEmail)
      {
        appSettings.Remove("User");
        appSettings.Remove("Password");
        appSettings.Remove("Provider");
      }
      else
      {
        appSettings.TryGetValue("User", out user);
        appSettings.TryGetValue("Password", out password);
        appSettings.TryGetValue("Provider", out provider);
        if (string.IsNullOrEmpty(user))
          provider = "GMAIL";       //default provider
      }

      if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
      {
        var mailSettings = _mailSettingsFactory.CreateExport().Value;
        var dialogresult = await mailSettings.ShowDialogAsync();

        user = mailSettings.Email.Encrypt();
        password = mailSettings.Password.Encrypt();
        provider = mailSettings.Providers.ShortName;

        if (mailSettings.RememberSettings)
        {
          appSettings.Remove("User");
          appSettings.Remove("Password");
          appSettings.Remove("Provider");

          appSettings.Add("User", user);
          appSettings.Add("Password", password);
          appSettings.Add("Provider", provider);
          appSettings.Save();
        }

        if (dialogresult != DialogResult.Ok)
          return;
      }

      var attach = "";
      if (!string.IsNullOrEmpty(Attachment))
        attach = SessionManager.CurrentAcco.AccoId + "_" + Attachment;

      await SendEmailService.ExecuteAsync(Addressee, Sender, Subject, ContentBody, attach, user.Decrypt(), password.Decrypt(), provider, SessionManager.CurrentOwner.PublicName);

    }

    public string Attachment { get; set; }

    protected override async void OnActivate()
    {
      base.OnActivate();

      LanguageList.Start(TemplateId);

      var unitofwork = _unitOfWorkManager.Create();

      //LanguageId = SessionManager.CurrentOwner.LanguageId;
      LanguageList.ItemId = LanguageId;
      
      var currencies = await unitofwork.Currencies.FindInDataSourceAsync(c => c.CurrencyCode == Acco.Currency);
      _currencySign = currencies.FirstOrDefault().CurrencySign;

      var template = await unitofwork.MailTemplates.WithIdFromDataSourceAsync(TemplateId);
      var htmlFile = new Uri(TemplateFile(template.Description, LanguageId), UriKind.Relative);

      ContentBody = ReplaceTags(ResourceReader.Content(htmlFile));
      //Subject = Subject = GetHtmlTitle(ContentBody);     //template.Description; //HtmlParser.ReadElement(ContentBody, "Title");
      Subject = HtmlParser.ReadTitle(ContentBody);

      UpdateTriggers();
    }

    //TODO evt veld opnemen in template
    private string TemplateFile(string htmlfile, int languageId)
    {
      //var htmlFile = SystemCodeService.Description(templateType, SystemGroupName.MailTemplate);

      return string.Format("/AccoBooking;component/Assets/MailTemplates/{0}/{1}.html",
        languageShortcode(languageId), htmlfile);
    }

    //Veld opnemen in language
    private string languageShortcode(int languageId)
    {
      switch (languageId)
      {
        case 1:
          return "nl";
        case 2:
          return "en";
        case 3:
          return "de";
        default:
          return "nl";
      }
    }

    private string Currency(string currency)
    {
      switch (currency.ToUpper())
      {
        case "POUND":
        case "GBP":
          return "£";
        case "DOLLAR":
        case "USD":
          return "$";
        default:
          return "€";
      }
    }

    private async Task SetContentLanguage(int languageid)
    {
      var unitofwork = _unitOfWorkManager.Create();

      var template = await unitofwork.MailTemplates.WithIdFromDataSourceAsync(TemplateId);
      LanguageId = languageid;
      //LanguageList.ItemId = LanguageId;

      var currencies = await unitofwork.Currencies.FindInDataSourceAsync(c => c.CurrencyCode == Acco.Currency);
      _currencySign = currencies.FirstOrDefault().CurrencySign;

      if (Booking != null)
      {
        _cancelRent = await CancelRentService.ExecuteAsync(Booking.BookingId);
        _totalRefund = await CancellationRefundService.ExecuteAsync(Booking.BookingId);
      }

      var htmlFile = new Uri(TemplateFile(template.Description, languageid), UriKind.Relative);

      ContentBody = ReplaceTags(ResourceReader.Content(htmlFile));
      //Subject = GetHtmlTitle(ContentBody); //template.Description; //HtmlParser.ReadElement(ContentBody, "Title");
      Subject = HtmlParser.ReadTitle(ContentBody);

      NotifyOfPropertyChange(() => Subject);
      NotifyOfPropertyChange(() => ContentBody);

    }

    //private string GetHtmlTitle(string html)
    //{
    //  string[] str = html.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
    //  bool append = false;
    //  string title = "";
    //  for (int i = 0; i < str.Length; i++)
    //  {
    //    if (!append)
    //    {
    //      if (str[i].Contains("<title>"))
    //        append = true;
    //      else 
    //        continue;
    //    }
        
        
    //    title += str[i].Replace("<title>", "").Replace("</title>", "");

    //    if (str[i].Contains("</title"))
    //      return title.Trim();
    //  }
    //  return "";
    //}

    private string Tag(string code)
    {
      return "%" + code.ToLower() + "%";
    }


    private string ReplaceTags(string text)
    {
      if (Acco != null)
        foreach (var code in SystemCodeService.SystemCodeList.Where(c => c.SystemGroup.Name == SystemGroupName.Acco))
        {
          switch (code.Code)
          {

            case "ACCO-DESCRIPTION":
              text = text.Replace(Tag(code.Description), Acco.Description);
              break;
            case "ACCO-TYPE-DESCRIPTION":
              text = text.Replace(Tag(code.Description), Acco.TypeDescription(LanguageId));
              break;
            case "ACCO-LOCATION":
              text = text.Replace(Tag(code.Description), Acco.Location);
              break;
            case "ACCO-ADDRESS-1":
              text = text.Replace(Tag(code.Description), Acco.Address1);
              break;
            case "ACCO-ADDRESS-2":
              text = text.Replace(Tag(code.Description), Acco.Address2);
              break;
            case "ACCO-ADDRESS-3":
              text = text.Replace(Tag(code.Description), Acco.Address3);
              break;
            case "ACCO-COUNTRY":
              text = text.Replace(Tag(code.Description), Acco.CountryDescription);
              break;
            case "ACCO-LATITUDE":
              text = text.Replace(Tag(code.Description), Acco.Latitude.ToString());
              break;
            case "ACCO-LONGITUDE":
              text = text.Replace(Tag(code.Description), Acco.Longitude.ToString());
              break;
            case "ACCO-MAP":
              text = text.Replace(Tag(code.Description), string.Format(
                        "<img src=\"https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&amp;zoom=10&amp;size=300x300&amp;markers=color:red%7Clabel:Acco%7C{0},{1}\">",
                        Acco.Latitude, Acco.Longitude));
              break;
            case "ACCO-MAP-LOCATION":
              text = text.Replace(Tag(code.Description), Acco.MapLocation);
              break;
            case "CURRENCY":
              text = text.Replace(Tag(code.Description), Acco.Currency);
              break;
            case "ARRIVE-AFTER":
              text = text.Replace(Tag(code.Description), Acco.ArriveAfterDescription(LanguageId));
              break;
            case "DEPARTURE-BEFORE":
              text = text.Replace(Tag(code.Description), Acco.DepartureBeforeDescription(LanguageId));
              break;
            case "OWNER-NAME":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.Name);
              break;
            case "OWNER-EMAIL":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.Email);
              break;
            case "OWN-WEBSITE":
              text = text.Replace(Tag(code.Description), Acco.OwnWebsite);
              break;
            case "OWNER-PHONE":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.Phone);
              break;
            case "OWNER-ADDRESS-1":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.PublicAddress1);
              break;
            case "OWNER-ADDRESS-2":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.PublicAddress2);
              break;
            case "OWNER-ADDRESS-3":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.PublicAddress3);
              break;
            case "OWNER-COUNTRY":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.CountryDescription);
              break;
            case "OWNER-BANK":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.PublicBank);
              break;
            case "OWNER-BANK-ACCOUNT":
              text = text.Replace(Tag(code.Description), Acco.AccoOwner.PublicBankAccount);
              break;

          }
        }


      if (Departure != null)
        foreach (var code in SystemCodeService.SystemCodeList.Where(c => c.SystemGroup.Name == SystemGroupName.Period))
        {
          switch (code.Code)
          {
            case "ARRIVAL":
              text = text.Replace(Tag(code.Description), Departure.Arrival.ToShortDateString());
              break;
            case "DEPARTURE":
              text = text.Replace(Tag(code.Description), Departure.Departure.ToShortDateString());
              break;
            case "RENT":
              text = text.Replace(Tag(code.Description), Departure.Rent.ToString());
              break;
            case "NIGHTS":
              text = text.Replace(Tag(code.Description), Departure.Nights.ToString());
              break;
          }
        }


      if (Booking != null)
        foreach (var code in SystemCodeService.SystemCodeList.Where(c => c.SystemGroup.Name == SystemGroupName.Booking))
        {
          switch (code.Code)
          {

            case "BOOKED":
              text = text.Replace(Tag(code.Description), Booking.Booked.ToShortDateString());
              break;

            case "CONFIRMED":
              if (Booking.Confirmed.HasValue)
                text = text.Replace(Tag(code.Description), Booking.Confirmed.Value.ToShortDateString());
              else
                text = text.Replace(Tag(code.Description), "?");
              break;

            case "ARRIVAL":
              text = text.Replace(Tag(code.Description), Booking.Arrival.ToShortDateString());
              break;

            case "DEPARTURE":
              text = text.Replace(Tag(code.Description), Booking.Departure.ToShortDateString());
              break;

            case "ADULTS":
              text = text.Replace(Tag(code.Description), Booking.Adults.ToString());
              break;

            case "CHILDREN":
              text = text.Replace(Tag(code.Description), Booking.Children.ToString());
              break;

            case "PETS":
              text = text.Replace(Tag(code.Description), Booking.Pets.ToString());
              break;

            case "RENT":
              text = text.Replace(Tag(code.Description), string.Format("{0} {1:0.00}", _currencySign, Booking.Rent));
              break;

            case "DEPOSIT":
              text = text.Replace(Tag(code.Description), string.Format("{0} {1:0.00}", _currencySign, Booking.Deposit));
              break;

            case "CANCEL-ADMINISTRATION-COSTS":
              text = text.Replace(Tag(code.Description), string.Format("{0} {1:0.00}", _currencySign, Booking.CancelAdministrationCosts));
              break;

            case "GUEST-NAME":
              text = text.Replace(Tag(code.Description), Booking.Booker);
              break;

            case "GUEST-EMAIL":
              text = text.Replace(Tag(code.Description), Booking.BookerEmail);
              break;

            case "GUEST-PHONE":
              text = text.Replace(Tag(code.Description), Booking.BookerPhone);
              break;

            case "GUEST-ADDRESS-1":
              text = text.Replace(Tag(code.Description), Booking.BookerAddress1);
              break;

            case "GUEST-ADDRESS-2":
              text = text.Replace(Tag(code.Description), Booking.BookerAddress2);
              break;

            case "GUEST-ADDRESS-3":
              text = text.Replace(Tag(code.Description), Booking.BookerAddress3);
              break;

            case "GUEST-COUNTRY":
              text = text.Replace(Tag(code.Description), Booking.CountryDescription);
              break;

            case "GUEST-BANK":
              text = text.Replace(Tag(code.Description), Booking.BookerBank);
              break;

            case "GUEST-BANK-ACCOUNT":
              text = text.Replace(Tag(code.Description), Booking.BookerBankAccount);
              break;

            case "ADDITION-DESCRIPTION":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => !a.AccoAddition.IsPaidFromDeposit))
                  if (!addition.AccoAddition.IsPaidFromDeposit)
                    text = text.Replace(Tag(code.Description + " " + i++), addition.Description);

                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "ADDITION-UNIT":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => !a.AccoAddition.IsPaidFromDeposit))
                  if (!addition.AccoAddition.IsPaidFromDeposit)
                  {
                    var unit = SystemCodeService.Description(addition.Unit, SystemGroupName.Unit);
                    text = text.Replace(Tag(code.Description + " " + i++), unit);
                  }

                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "ADDITION-QUANTITY":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => !a.AccoAddition.IsPaidFromDeposit))
                  if (!addition.AccoAddition.IsPaidFromDeposit)
                    text = text.Replace(Tag(code.Description + " " + i++), "1");

                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "ADDITION-PRICE":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => !a.AccoAddition.IsPaidFromDeposit))
                  if (!addition.AccoAddition.IsPaidFromDeposit)
                    text = text.Replace(Tag(code.Description + " " + i++), string.Format("{0} {1:0.00}", _currencySign, addition.Price));

                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "ADDITION-AMOUNT":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => !a.AccoAddition.IsPaidFromDeposit))
                  if (!addition.AccoAddition.IsPaidFromDeposit)
                    text = text.Replace(Tag(code.Description + " " + i++.ToString()), string.Format("{0} {1:0.00}", _currencySign, addition.Amount));

                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j.ToString()), "");
                break;
              }

            case "DEPOSIT-ADDITION-DESCRIPTION":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => a.AccoAddition.IsPaidFromDeposit))
                  if (addition.AccoAddition.IsPaidFromDeposit)
                    text = text.Replace(Tag(code.Description + " " + i++), addition.Description);

                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "DEPOSIT-ADDITION-PRICE":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => a.AccoAddition.IsPaidFromDeposit))
                  if (addition.AccoAddition.IsPaidFromDeposit)
                    text = text.Replace(Tag(code.Description + " " + i++), string.Format("{0} {1:0.00}", _currencySign, addition.Price));

                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "DEPOSIT-ADDITION-UNIT":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => a.AccoAddition.IsPaidFromDeposit))
                  if (addition.AccoAddition.IsPaidFromDeposit)
                  {
                    var unit = SystemCodeService.Description(addition.Unit, SystemGroupName.Unit);
                    text = text.Replace(Tag(code.Description + " " + i++), unit);
                  }
                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "DEPOSIT-ADDITION-AMOUNT":
              {
                int i = 1;
                foreach (var addition in Booking.BookingAdditions.Where(a => a.AccoAddition.IsPaidFromDeposit))
                {
                  if (addition.AccoAddition.IsPaidFromDeposit)
                    text = text.Replace(Tag(code.Description + " " + i++.ToString()), string.Format("{0} {1:0.00}", _currencySign, addition.Amount));
                }
                for (int j = i; j < 6; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-DUE-SCHEDULED-GUEST":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => p.IsScheduledPayment && p.IsPaymentByGuest))
                  text = text.Replace(Tag(code.Description + " " + i++), payment.Due.ToShortDateString());
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-AMOUNT-SCHEDULED-GUEST":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => p.IsScheduledPayment && p.IsPaymentByGuest))
                  text = text.Replace(Tag(code.Description + " " + i++), string.Format("{0} {1:0.00}", _currencySign, payment.Amount));
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-DUE-PAID-GUEST":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => !p.IsScheduledPayment && p.IsPaymentByGuest))
                  text = text.Replace(Tag(code.Description + " " + i++), payment.Due.ToShortDateString());
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-AMOUNT-PAID-GUEST":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => !p.IsScheduledPayment && p.IsPaymentByGuest))
                  text = text.Replace(Tag(code.Description + " " + i++), string.Format("{0} {1:0.00}", _currencySign, payment.Amount));
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-DUE-SCHEDULED-OWNER":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => p.IsScheduledPayment && !p.IsPaymentByGuest))
                  text = text.Replace(Tag(code.Description + " " + i++), payment.Due.ToShortDateString());
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-AMOUNT-SCHEDULED-OWNER":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => p.IsScheduledPayment && !p.IsPaymentByGuest)
                  )
                  text = text.Replace(Tag(code.Description + " " + i++), string.Format("{0} {1:0.00}", _currencySign, payment.Amount));
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-DUE-PAID-OWNER":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => !p.IsScheduledPayment && p.IsPaymentByGuest))
                  text = text.Replace(Tag(code.Description + " " + i++), payment.Due.ToShortDateString());
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "PAY-AMOUNT-PAID-OWNER":
              {
                int i = 1;
                foreach (var payment in Booking.BookingPayments.Where(p => !p.IsScheduledPayment && !p.IsPaymentByGuest)
                  )
                  text = text.Replace(Tag(code.Description + " " + i++), string.Format("{0} {1:0.00}", _currencySign, payment.Amount));
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "CANCEL-DAYS-BEFORE-ARRIVAL":
              {
                int i = 1;
                foreach (var cancelcondition in Booking.BookingCancelConditions)
                  text = text.Replace(Tag(code.Description + " " + i++),
                    cancelcondition.DaysBeforArrival.ToString());
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "CANCEL-PERCENTAGE-TO-PAY":
              {
                int i = 1;
                foreach (var cancelcondition in Booking.BookingCancelConditions)
                  text = text.Replace(Tag(code.Description + " " + i++),
                    cancelcondition.RentPercentageToPay.ToString());
                for (int j = i; j < 4; j++)
                  text = text.Replace(Tag(code.Description + " " + j), "");
                break;
              }

            case "CANCEL-RENT-AMOUNT":
              {
                text = text.Replace(Tag(code.Description), string.Format("{0} {1:0.00}", _currencySign, _cancelRent));
                break;
              }

            case "TOTAL-REFUND":
              {
                text = text.Replace(Tag(code.Description), string.Format("{0} {1:0.00}", _currencySign, _totalRefund));
                break;
              }

          }
        }

      return text;
    }

    //public event EventHandler<ResultCompletionEventArgs> Completed;

    public async static Task SelectLanguage(string language)
    {
      var unitOfWorkManager = new AccoBookingUnitOfWorkManager();
      var unitofwork = unitOfWorkManager.Create();

      // taal van de guest zoeken, lijkt me nog een beetje lastig die stringwaarde voor language
      var languages = await unitofwork.Languages.FindInDataSourceAsync(l => l.Description == language, CancellationToken.None);

      if (languages.Count() > 0)
        LanguageId = languages.FirstOrDefault().LanguageId;
      else
        LanguageId = SessionManager.CurrentOwner.LanguageId;

    }

    public async static Task SelectMailTemplate(string templateType)
    {
      var unitOfWorkManager = new AccoBookingUnitOfWorkManager();
      var unitofwork = unitOfWorkManager.Create();

      // template zoeken 
      var templates = await unitofwork.MailTemplates.FindInDataSourceAsync(l => l.TemplateType == templateType, CancellationToken.None);

      var template = templates.FirstOrDefault();

      TemplateId = template != null ? template.MailTemplateId : 0;
    }

    public static async Task SelectMailTemplateContent(string templateType, int languageid)
    {
      LanguageId = languageid;
      await SelectMailTemplate(templateType);
      //await SelectMailTemplateContent();
    }

    public static async Task SelectMailTemplateContent(string templateType, string language)
    {
      await SelectMailTemplate(templateType);
      await SelectLanguage(language);
      //await SelectMailTemplateContent();
    }

    public static async Task SelectMailTemplateContent(int templateid, string language)
    {

      TemplateId = templateid;
      await SelectLanguage(language);
      //await SelectMailTemplateContent();
    }

    public static async Task SelectMailTemplateContent(int templateid, int languageid)
    {
      LanguageId = languageid;
      TemplateId = templateid;
      //await SelectMailTemplateContent();

    }

    //public static async Task SelectMailTemplateContent()
    //{
    //  var unitOfWorkManager = new AccoBookingUnitOfWorkManager();
    //  var unitofwork = unitOfWorkManager.Create();


    //  MailTemplate mailtemplate = null;

    //  if (TemplateId != 0)
    //    mailtemplate = await unitofwork.MailTemplates.WithIdFromDataSourceAsync(TemplateId);

    //  //selecteer template
    //  //1. van taal guest
    //  //2. van taal owner
    //  //3. de eerste die gevonden wordt
    //  MailTemplateContent content = null;

    //  var contents = await unitofwork.MailTemplateContents.FindInDataSourceAsync(c => c.MailTemplateId == TemplateId, CancellationToken.None);

    //      if (LanguageId > 0)
    //        content = contents.FirstOrDefault(t => t.LanguageId == LanguageId);
    //      if (content == null && LanguageId != SessionManager.CurrentOwner.LanguageId)
    //        content = contents.FirstOrDefault(t => t.LanguageId == SessionManager.CurrentOwner.LanguageId);
    //      if (content == null && contents.Count() > 0)
    //        content = contents.FirstOrDefault();
    //      if (content != null)
    //        TemplateContentId = content.MailTemplateContentId;


    //      if (content == null && TemplateId != 0)
    //  {
    //    var dialogManager = new DialogManager();
    //    await dialogManager.ShowMessageAsync(string.Format(Resources.AccoBooking.mes_NO_TEMPLATE, mailtemplate.Description), DialogButtons.Ok);
    //  }
    //}

  }

}
