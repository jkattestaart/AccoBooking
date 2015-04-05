using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingSearchViewModel : BaseSearchViewModel<DomainModel.Booking, BookingListItem>
  {
    private bool _includeClosed;
    private bool _includeExpired;
    private string _guestName;
    private DateTime _from = DateTime.MinValue;
    private DateTime _to = DateTime.MaxValue;

    [ImportingConstructor]
    public BookingSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager )
    {

    }

    public bool IncludeClosed
    {
      get { return _includeClosed; }
      set
      {
        _includeClosed = value;
        Search(); // check or uncheck include closed results directly in search
        NotifyOfPropertyChange(() => CanSearch);
        NotifyOfPropertyChange(() => CanClear);
      }
    }

    public bool IncludeExpired
    {
      get { return _includeExpired; }
      set
      {
        _includeExpired = value;
        Search();
        NotifyOfPropertyChange(() => CanSearch);
        NotifyOfPropertyChange(() => CanClear);
      }
    }

    public DateTime From
    {
      get { return _from; }
      set
      {
        _from = value;
        NotifyOfPropertyChange(() => CanSearch);
        NotifyOfPropertyChange(() => CanClear);
      }
    }

    public DateTime To
    {
      get { return _to; }
      set
      {
        _to = value;
        NotifyOfPropertyChange(() => CanSearch);
        NotifyOfPropertyChange(() => CanClear);
      }
    }


    public string GuestName
    {
      get { return _guestName; }
      set
      {
        _guestName = value;
        NotifyOfPropertyChange(() => CanSearch);
        NotifyOfPropertyChange(() => CanClear);
      }
    }

    public override bool CanClear
    {
      get
      {
        return !string.IsNullOrWhiteSpace(GuestName) || IncludeClosed || IncludeExpired ||
               From.Date != new DateTime(1, 1, 1) || To.Date != new DateTime(1, 1, 1);
      }
    }

    public override bool CanSearch
    {
      get { return true; }
    }

    protected override Task<IEnumerable<BookingListItem>> ExecuteQuery()
    {
      return UnitOfWork.BookingSearchService.FindBookingsAsync(GuestName, IncludeClosed, IncludeExpired, From, To,CancellationToken.None);
        //TODO: de rest
    }

    public override void Clear()
    {
      IncludeClosed = false;
      IncludeExpired = false;
      GuestName = "";
      To = new DateTime(1, 1, 1);
      From = new DateTime(1, 1, 1);

      NotifyOfPropertyChange(() => IncludeClosed);
      NotifyOfPropertyChange(() => IncludeExpired);
      NotifyOfPropertyChange(() => GuestName);
      NotifyOfPropertyChange(() => To);
      NotifyOfPropertyChange(() => From);

      Search();
    }

    public async void DumpBookings(Stream stream)
    {


      var bookings =
        await
          UnitOfWork.Bookings.FindInDataSourceAsync(b => b.Acco.AccoOwnerId == SessionManager.CurrentOwner.AccoOwnerId,
            q => q.OrderBy(b => b.Arrival),
            q => q.Include(b=>b.Acco)
            );

      using (var sw = new StreamWriter(stream))
      {
        sw.Write(Resources.AccoBooking.lab_ACCO);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_ARRIVAL);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_DEPARTURE);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_GUEST);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_ADULTS);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_CHILDREN);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_PETS);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_ADDITIONS);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_RENT);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_DEPOSIT);
        sw.Write(";");
        sw.Write(Resources.AccoBooking.lab_STATUS);
        sw.WriteLine();
        foreach (var booking in bookings)
        {
          sw.Write(booking.Acco.Description);
          sw.Write(";");
          sw.Write(booking.Arrival);
          sw.Write(";");
          sw.Write(booking.Departure);
          sw.Write(";");
          sw.Write(booking.Booker);
          sw.Write(";");
          sw.Write(booking.Adults);
          sw.Write(";");
          sw.Write(booking.Children);
          sw.Write(";");
          sw.Write(booking.Pets);
          sw.Write(";");
          sw.Write(booking.Additions);
          sw.Write(";");
          sw.Write(booking.Rent);
          sw.Write(";");
          sw.Write(booking.Deposit);
          sw.Write(";");
          sw.Write(booking.Status);
          sw.WriteLine();
        }
        sw.Close();
      }
    }

  }

}
