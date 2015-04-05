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
using System.Linq;
using System.Threading.Tasks;
using AccoBooking.ViewModels.Acco;
using Cocktail;
using DomainModel;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingPaymentSummaryViewModel : BaseScreen<DomainModel.Booking>
  {
    private bool _applyPattern;

    [ImportingConstructor]
    public BookingPaymentSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                          BookingPaymentListViewModel scheduledpaymentguestList,
                                          BookingPaymentListViewModel scheduledpaymentownerList,
                                          BookingPaymentListViewModel paymentguestList,
                                          BookingPaymentListViewModel paymentownerList,
                                          AccoPayPatternListViewModel accoPayPatternList,

                                          IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      ScheduledPaymentGuestList = scheduledpaymentguestList;
      ScheduledPaymentGuestList.IsPaymentByGuest = true;
      ScheduledPaymentGuestList.IsScheduledPayment = true;
      ScheduledPaymentGuestList.PropertyChanged += PaymentListPropertyChanged;

      ScheduledPaymentOwnerList = scheduledpaymentownerList;
      ScheduledPaymentOwnerList.IsPaymentByGuest = false;
      ScheduledPaymentOwnerList.IsScheduledPayment = true;
      ScheduledPaymentOwnerList.PropertyChanged += PaymentListPropertyChanged;

      PaymentGuestList = paymentguestList;
      PaymentGuestList.IsPaymentByGuest = true;
      PaymentGuestList.IsScheduledPayment = false;
      PaymentGuestList.PropertyChanged += PaymentListPropertyChanged;

      PaymentOwnerList = paymentownerList;
      PaymentOwnerList.IsPaymentByGuest = false;
      PaymentOwnerList.IsScheduledPayment = false;
      PaymentOwnerList.PropertyChanged += PaymentListPropertyChanged;

      AccoPayPatternList = accoPayPatternList;
      AccoPayPatternList.PropertyChanged += AccoPayPatternListPropertyChanged;
    }

    void AccoPayPatternListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CanApplyPattern);
      NotifyOfPropertyChange(() => PaymentRemark);
    }
    
    void PaymentListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CanSave);
      NotifyOfPropertyChange(() => PaymentRemark);
    }

    public BookingPaymentListViewModel ScheduledPaymentGuestList { get; set; }
    public BookingPaymentListViewModel ScheduledPaymentOwnerList { get; set; }
    public BookingPaymentListViewModel PaymentGuestList { get; set; }
    public BookingPaymentListViewModel PaymentOwnerList { get; set; }

    public AccoPayPatternListViewModel AccoPayPatternList { get; set; }
  
    public bool IsReadOnly
    {
      get
      {
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

    public string PaymentRemark
    {
      get
      {
        decimal total = 0;
        foreach (var payment in ((DomainModel.Booking) Entity).BookingPayments.Where(p => p.IsPaymentByGuest && p.IsScheduledPayment))
        {
          total = total + payment.Amount;
        }
        if (((DomainModel.Booking)Entity).Total - ((DomainModel.Booking)Entity).Usage != total) 
          return Resources.AccoBooking.mes_APPLY_PAY_PATTERN;

        total = 0;
        foreach (var payment in ((DomainModel.Booking)Entity).BookingPayments.Where(p => !p.IsPaymentByGuest && p.IsScheduledPayment))
        {
          total = total + payment.Amount;
        }
        if (((DomainModel.Booking)Entity).Deposit - ((DomainModel.Booking)Entity).Usage != total)
          return Resources.AccoBooking.mes_APPLY_PAY_PATTERN;
        
        return "";
      }
    }

    public override Entity Entity
    {
      get { return base.Entity; }
      set
      {
        if (value != null)
        {
          base.Entity = value;
          if (((DomainModel.Booking) Entity).Acco.DefaultPayPatternId.HasValue)
          {
            AccoPayPatternList.ItemId = ((DomainModel.Booking) Entity).Acco.DefaultPayPatternId.Value;
            NotifyOfPropertyChange(()=>PaymentRemark);
          }

        }
      }
    }

    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    public override BaseScreen<DomainModel.Booking> Start(int entityid)
    {
      ScheduledPaymentGuestList.Start(entityid);
      ScheduledPaymentOwnerList.Start(entityid);
      PaymentGuestList.Start(entityid);
      PaymentOwnerList.Start(entityid);
      
      AccoPayPatternList.Start(SessionManager.BookingAccoId);
 
      NotifyOfPropertyChange(() => IsReadOnly);

      return base.Start(entityid);
    }

    public bool CanApplyPattern
    {
      get { return !string.IsNullOrEmpty(AccoPayPatternList.Description); }
    }
 

    public async void ApplyPattern()
    {
      //implicit save toepassen
      _applyPattern = true;
      await Save();

      using (Busy.GetTicket())
      {
        if (_entityid > 0)
          await ApplyPayPatternService.ExecuteAsync(_entityid, AccoPayPatternList.ItemId);

        Entity = await Repository().WithIdFromDataSourceAsync(_entityid);
      }

      ScheduledPaymentGuestList.Start(_entityid);
      ScheduledPaymentOwnerList.Start(_entityid);
      NotifyOfPropertyChange(()=>PaymentRemark);
    }

    public override Task OnPostSave(bool isDelete)
    {
      if (!_applyPattern)
      {
        (((Parent as PayPatternManagementSectionViewModel)
          .Parent as BookingDetailViewModel)
          .Parent as BookingManagementViewModel).Cancel();
      }
      _applyPattern = false;
      return base.OnPostSave(isDelete);
    }

    protected async override void OnActivate()
    {
      Entity = await Repository().WithIdFromDataSourceAsync(_entityid);
      
      base.OnActivate();
    }
  }
}