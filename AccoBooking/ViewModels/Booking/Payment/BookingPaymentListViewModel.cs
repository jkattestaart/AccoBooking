using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using Cocktail;
using DomainModel;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingPaymentListViewModel : BaseScreen<DomainModel.Booking>
  {
    private readonly IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    private BindableCollection<BookingPaymentItemViewModel> _payments;
    private decimal _total;

    [ImportingConstructor]
    public BookingPaymentListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                       IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      _unitOfWorkManager = unitOfWorkManager;
    }

    public bool IsScheduledPayment { get; set; }

    public bool IsPaymentByGuest { get; set; }

    public bool TotalVisible 
    { 
      get { return (Payments.Count > 1); } 
    }

    public string PaidStatus {
      get
      {
        if (!IsScheduledPayment)
        return "";

        decimal totalpaid = 0;
        var p = Payments.FirstOrDefault();
        if (p != null)
        {
          foreach (
            var payment in
              p.Item.Booking.BookingPayments.Where(
                x => x.IsPaymentByGuest == p.Item.IsPaymentByGuest && !x.IsScheduledPayment))
            totalpaid = totalpaid + payment.Amount;
          if (Total == totalpaid && Total != 0)
            return Resources.AccoBooking.lab_IS_COMPLETE;
        }
        return "";
      }
    }

    public decimal Total
    {
      get
      {
        //MessageBox.Show(string.Format("Bereken total"));
        _total = 0;
        foreach (var p in Payments)
          _total += p.Item.Amount;

        return _total;
      }
    }

    public override Entity Entity
    {
      get { return base.Entity; }
      set
      {
        {
          if (base.Entity != null)
            ((DomainModel.Booking) base.Entity).BookingPayments.CollectionChanged -= PaymentsCollectionChanged;

          ClearPayments();

          if (value != null)
          {
            Payments = new BindableCollection<BookingPaymentItemViewModel>(
              ((DomainModel.Booking) value).BookingPayments.ToList()
                                           .Where(a => a.IsPaymentByGuest==IsPaymentByGuest && a.IsScheduledPayment == IsScheduledPayment)
                                           .Select(a => new BookingPaymentItemViewModel(a)));
            ((DomainModel.Booking)value).BookingPayments.CollectionChanged += PaymentsCollectionChanged;
          }
          base.Entity = value;
        }
      }
    }

    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    public BindableCollection<BookingPaymentItemViewModel> Payments
    {
      get { return _payments; }
      set
      {
        _payments = value;
        foreach (var p in Payments)
          p.Item.EntityAspect.EntityPropertyChanged += ItemPropertyChanged;

        NotifyOfPropertyChange(() => Payments);
        NotifyOfPropertyChange(() => TotalVisible);
        NotifyOfPropertyChange(() => Total);
        NotifyOfPropertyChange(() => PaidStatus);
      }
    }

    private void PaymentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (var item in
          e.OldItems.Cast<BookingPayment>()
           .Where(a => a.IsPaymentByGuest == IsPaymentByGuest && a.IsScheduledPayment == IsScheduledPayment)
           .Select(a => Payments.FirstOrDefault(i => i.Item == a)))
        {
          item.Item.EntityAspect.EntityPropertyChanged -= ItemPropertyChanged;
          Payments.Remove(item);
          item.Dispose();
        }
      }

      if (e.NewItems != null)
        e.NewItems.Cast<BookingPayment>()
         .ForEach(a =>
           {
             if (a.IsPaymentByGuest == IsPaymentByGuest && a.IsScheduledPayment == IsScheduledPayment)
             {
               a.EntityAspect.EntityPropertyChanged += ItemPropertyChanged;
               Payments.Add(new BookingPaymentItemViewModel(a));
             }
           });

      EnsureDelete();
    }

    void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() =>  Payments);
      NotifyOfPropertyChange(() => TotalVisible);
      NotifyOfPropertyChange(() => Total);
      NotifyOfPropertyChange(() => PaidStatus);
    }

    private void EnsureDelete()
    {
      Payments.ForEach(i => i.NotifyOfPropertyChange(() => i.CanDelete));
      NotifyOfPropertyChange(() => TotalVisible);
      NotifyOfPropertyChange(() => Total);
    }

    public async void Add()
    {
      using (Busy.GetTicket())
      {
        var acco = await _unitOfWork.Accoes.WithIdFromDataSourceAsync(SessionManager.BookingAccoId);
        var sequence = await SequenceKeyService.NextValueAsync("BookingPaymentId", CancellationToken.None);

        int paymentid = sequence.CurrentId;
        var payment = ((DomainModel.Booking) Entity).AddBookingPayment(paymentid, IsPaymentByGuest, IsScheduledPayment);
        if (!IsPaymentByGuest && IsScheduledPayment)
          payment.Due = ((DomainModel.Booking) Entity).Departure.AddDays(acco.DaysToPayDepositBackAfterDeparture);
        NotifyOfPropertyChange(() => Entity);
      }
      EnsureDelete();
    }

    public void Delete(BookingPaymentItemViewModel paymentItem)
    {

      using (Busy.GetTicket())
      {
        ((DomainModel.Booking) Entity).DeleteBookingPayment(paymentItem.Item);
      }
      EnsureDelete();
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);

      if (!close) return;

      ClearPayments();
    }

    private void ClearPayments()
    {
      if (Payments != null)
      {

        // Clean up to avoid memory leaks
        Payments.ForEach(i =>
          {
            i.PropertyChanged -= ItemPropertyChanged;
            i.Dispose();
          });
        Payments.Clear();
      }

    }
  }
}
