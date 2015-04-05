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
using System.Threading.Tasks;
using Cocktail;
using Common.Actions;
using DomainModel;
using DomainModel.Projections;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.EntityModel;

// @@@ JKT controleer ook CheckReminderManagementViewModel bij wijzigingen...
// @@@ TODO: toch CheckReminderSummary maken met UpdateBookingViewModel als content?

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingTrusteeManagementViewModel :
    BaseMasterViewModel<BookingTrusteeSearchViewModel, BookingTrusteeDetailViewModel, BookingListItem, DomainModel.Booking>
  {
    private ShellViewModel _shellViewModel;
    private ExportFactory<BookingMailViewModel> _bookingMailFactory;
    private ExportFactory<CreateBookingDetailViewModel> _createBookingFactory;
    private INavigator _bookingMailNavigatorService;
    private bool _isNewBooking;
    private bool _isMailActive;

    public ToolbarViewModel CopyToolbarViewModel;
    private ExportFactory<BookingTrusteeDetailViewModel> _detailTrusteeFactory;

    [ImportingConstructor]
    public BookingTrusteeManagementViewModel(ExportFactory<BookingTrusteeSearchViewModel> searchFactory,
      ExportFactory<BookingTrusteeDetailViewModel> detailFactory,
      ExportFactory<CreateBookingDetailViewModel> createBookingFactory,
      ExportFactory<BookingMailViewModel> bookingMailFactory,
      IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
      ShellViewModel shellViewModel,
      IDialogManager dialogManager,
      ToolbarViewModel toolbar,
      ToolbarViewModel bottomToolbar)
      : base(searchFactory, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {
      _createBookingFactory = createBookingFactory;
      _bookingMailFactory = bookingMailFactory;
      _shellViewModel = shellViewModel;

      _bookingMailNavigatorService = new Navigator(this);
    }


    protected override void OnActivate()
    {
      base.OnActivate();
      _shellViewModel.BuildMenu("");
      Toolbar.Clear();
      _toolbarGroup = new ToolbarGroup(10)
      {
        new ToolbarAction(this, Resources.AccoBooking.but_EDIT, "edit.png", Edit),
      };

      Toolbar.AddGroup(_toolbarGroup);
    }

    public void StartSearch()
    {
      UseSearch = true;
      Start();
    }

    protected override async Task<DialogResult> CheckEdit()
    {
      var unitOfWork = _unitOfWorkManager.Get(_entityid);

      var booking = await unitOfWork.Bookings.WithIdFromDataSourceAsync(_entityid);
      if (booking.Status == "CANCELLED")
        return await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_BOOKING_CANCELLED, DialogButtons.Ok);
      if (booking.Status == "EXPIRED")
        return await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_BOOKING_EXPIRED, DialogButtons.Ok);

      return DialogResult.Ignore;
    }

    public async void StartDetail(int entityid)
    {
      base.OnActivate();
      _shellViewModel.BuildMenu("");
      if (Toolbar != null)
      {
        Toolbar.Clear();
        //_toolbarGroup = new ToolbarGroup(10)
        //{
        //  new ToolbarAction(this, Resources.AccoBooking.but_BACK, "cancel.png", Back),
        //};

        //Toolbar.AddGroup(_toolbarGroup);
        Toolbar.IsVisible = true;
      }
      _entityid = entityid;
      UseSearch = false;

      var detail = _detailFactory.CreateExport().Value;
      await _navigator.NavigateToAsync(detail.GetType(),
        target =>
        {
          (target as BookingTrusteeDetailViewModel).Start(entityid);
          (target as BookingTrusteeDetailViewModel).Parent = this;

        }

        );

      UpdateCommands();

    }

    public void StartDetail()
    {
      _isMailActive = false;
      StartDetail(_entityid);
    }

    public async void StartCreate(int entityid)
    {

      try
      {
        UseSearch = false;
        var detail = _createBookingFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(detail.GetType(),
          target => (target as CreateBookingDetailViewModel).Start(true, _parentid)
          );

      }
      catch (TaskCanceledException)
      {
        UpdateCommands();
      }
    }
    
    protected override IRepository<DomainModel.Booking> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.Bookings;
    }


    public override async Task OnSave()
    {
      _isNewBooking = (ActiveDetail.Entity.EntityAspect.EntityState.IsAdded());
    }


    public override async Task OnPostSave(bool isDelete)
    {
      var booking = (DomainModel.Booking) ActiveDetail.Entity;

      if (booking != null)
        using (ActiveDetail.Busy.GetTicket())
          await UpdateBookingService.ExecuteAsync(booking.BookingId);
      
      if (_isNewBooking)
      {
        using (ActiveDetail.Busy.GetTicket())
        {
          await CreateBookingService.ExecuteAsync(booking.BookingId);
          await UpdateBookingService.ExecuteAsync(booking.BookingId);

          DomainModel.Acco acco = null;
          SendMailViewModel.Context = "BOOKING";

          using (Busy.GetTicket())
          {
            var unitofWork = _unitOfWorkManager.Create();

            PreviewMailViewModel.Acco = await unitofWork.Accoes.WithIdFromDataSourceAsync(SessionManager.BookingAccoId);
            PreviewMailViewModel.Booking = await unitofWork.Bookings.WithIdFromDataSourceAsync(booking.BookingId);

            await PreviewMailViewModel.SelectMailTemplateContent("RESERVATION", booking.BookerLanguageId);

            PreviewMailViewModel.From = SessionManager.CurrentOwner.PublicEmail;
            PreviewMailViewModel.To = booking.BookerEmail;
          }
        }
        SendMail();
      }
      else if (booking != null)
        using (ActiveDetail.Busy.GetTicket())
          await UpdateBookingService.ExecuteAsync(booking.BookingId);

      if (ActiveDetail != null)
        ActiveDetail.Start(_entityid);
    }

    public async void SendMail()
    {
      _isMailActive = true;


      try
      {

        var bookingmail = _bookingMailFactory.CreateExport().Value;

        await _bookingMailNavigatorService.NavigateToAsync(
          bookingmail.GetType(),
          target =>
          {
            (target as BookingMailViewModel).Parent = this;
            (target as BookingMailViewModel).Send();
          });

      }
      catch (TaskCanceledException)
      {
        UpdateCommands();
      }
    }

  

    public async void StartMail()
    {
      _isMailActive = true;

      try
      {
        var bookingmail = _bookingMailFactory.CreateExport().Value;
        await _bookingMailNavigatorService.NavigateToAsync(
          bookingmail.GetType(),
          target =>
          {
            (target as BookingMailViewModel).Parent = this;
            (target as BookingMailViewModel).Start(_parentid);
          });

      }
      catch (TaskCanceledException)
      {
        UpdateCommands();
      }
    }

    public void Back()
    {
      Cancel();
      
    }
    
    public async override Task Cancel()
    {
      if (Parent.GetType() == typeof (UpdateBookingViewModel) || Parent.GetType() == typeof (CreateBookingViewModel))
        if (ActiveUnitOfWork.HasChanges())
        {
          var dialogresult =
            await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_CANCEL, new[] { Resources.AccoBooking.but_YES, Resources.AccoBooking.but_NO, Resources.AccoBooking.but_CANCEL });

          if (dialogresult == Resources.AccoBooking.but_CANCEL)
            return;

          if (dialogresult == Resources.AccoBooking.but_YES)
              {
                Busy.AddWatch();

                var shouldClose = ActiveEntity.EntityAspect.EntityState.IsAdded();
                ActiveUnitOfWork.Rollback();

                if (shouldClose)
                  ActiveDetail.TryClose();

              }
              CancelParent();
            }
        else CancelParent();

      else await base.Cancel();
    }

    private void CancelParent()
    {
      if (Parent.GetType() == typeof(UpdateBookingViewModel))
        (Parent as UpdateBookingViewModel).Cancel();
      else if (Parent.GetType() == typeof(CreateBookingViewModel))
        (Parent as CreateBookingViewModel).Cancel();
      TryClose();
    }

    protected override void UpdateCommands()
    {
      base.UpdateCommands();
      if (IsDetailActive)
      {
        if (Toolbar != null)
        {
          Toolbar.Clear();
          //_toolbarGroup = new ToolbarGroup(10)
          //{
          //  new ToolbarAction(this, Resources.AccoBooking.but_BACK, "cancel.png", Back),
          //};

          //Toolbar.AddGroup(_toolbarGroup);
          Toolbar.IsVisible = true;
        }
      }
      else if (Toolbar !=null && Toolbar.IsVisible)
      {
        Toolbar.Clear();
        _toolbarGroup = new ToolbarGroup(10)
        {
          new ToolbarAction(this, Resources.AccoBooking.but_EDIT, "edit.png", Edit),
        };
        Toolbar.AddGroup(_toolbarGroup);
       
      }

      if (_isMailActive)
        Toolbar.IsVisible = false;
    }
  }

}