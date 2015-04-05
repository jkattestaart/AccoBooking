using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel.Projections;
using DomainServices;

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SelectBookingViewModel : BaseSearchViewModel<DomainModel.Booking, BookingListItem>
  {
    private bool _includeClosed;
    private bool _includeExpired;
    private string _guestName;
    private DateTime _from = DateTime.MinValue;
    private DateTime _to = DateTime.MaxValue;
    private IDialogUICommand<DialogResult> _okCommand;
    private IDialogManager _dialogManager;

    [ImportingConstructor]
    public SelectBookingViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager, IDialogManager dialogManager)
      : base(unitOfWorkManager )
    {
      _dialogManager = dialogManager;
      
    }

    public Task<DialogResult> ShowDialogAsync()
    {
      _okCommand = new DialogUICommand<DialogResult>("Selecteer", DialogResult.Ok, true) { Enabled = true};
      var cancelCommand = new DialogUICommand<DialogResult>(Resources.AccoBooking.but_CANCEL, DialogResult.Cancel, false, true);

      return _dialogManager.ShowDialogAsync(new[] { _okCommand, cancelCommand }, this);
    }

    public void DoubleClicked()
    {
      DialogHost.GetCurrent(this).TryClose(DialogResult.Ok);   //simuleer de selecteer knop
    }

    //public bool IncludeClosed
    //{
    //  get { return _includeClosed; }
    //  set
    //  {
    //    _includeClosed = value;
    //    Search(); // check or uncheck include closed results directly in search
    //    NotifyOfPropertyChange(() => CanSearch);
    //    NotifyOfPropertyChange(() => CanClear);
    //  }
    //}

    //public bool IncludeExpired
    //{
    //  get { return _includeExpired; }
    //  set
    //  {
    //    _includeExpired = value;
    //    Search();
    //    NotifyOfPropertyChange(() => CanSearch);
    //    NotifyOfPropertyChange(() => CanClear);
    //  }
    //}

    //public DateTime From
    //{
    //  get { return _from; }
    //  set
    //  {
    //    _from = value;
    //    NotifyOfPropertyChange(() => CanSearch);
    //    NotifyOfPropertyChange(() => CanClear);
    //  }
    //}

    //public DateTime To
    //{
    //  get { return _to; }
    //  set
    //  {
    //    _to = value;
    //    NotifyOfPropertyChange(() => CanSearch);
    //    NotifyOfPropertyChange(() => CanClear);
    //  }
    //}


    //public string GuestName
    //{
    //  get { return _guestName; }
    //  set
    //  {
    //    _guestName = value;
    //    NotifyOfPropertyChange(() => CanSearch);
    //    NotifyOfPropertyChange(() => CanClear);
    //  }
    //}

    //public override bool CanClear
    //{
    //  get
    //  {
    //    return !string.IsNullOrWhiteSpace(GuestName) || IncludeClosed || IncludeExpired ||
    //           From.Date != new DateTime(1, 1, 1) || To.Date != new DateTime(1, 1, 1);
    //  }
    //}

    //public override bool CanSearch
    //{
    //  get { return true; }
    //}

    protected override Task<IEnumerable<BookingListItem>> ExecuteQuery()
    {
      return UnitOfWork.BookingSearchService.FindBookingsAsync(CancellationToken.None);
        //TODO: de rest
    }

    //public override void Clear()
    //{
    //  IncludeClosed = false;
    //  IncludeExpired = false;
    //  GuestName = "";
    //  To = new DateTime(1, 1, 1);
    //  From = new DateTime(1, 1, 1);

    //  NotifyOfPropertyChange(() => IncludeClosed);
    //  NotifyOfPropertyChange(() => IncludeExpired);
    //  NotifyOfPropertyChange(() => GuestName);
    //  NotifyOfPropertyChange(() => To);
    //  NotifyOfPropertyChange(() => From);

    //  Search();
    //}


  }

}
