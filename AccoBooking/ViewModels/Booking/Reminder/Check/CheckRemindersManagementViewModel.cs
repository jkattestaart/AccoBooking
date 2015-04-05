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
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using Cocktail;
using Common.Actions;
using DomainModel.Projections;
using DomainServices;

// @@@ JKT is eigenlijk copie van BookingManagementViewModel
// @@@ TODO: toch CheckReminderSummary maken met UpdateBookingViewModel als content?
using DomainServices.Services;

namespace AccoBooking.ViewModels.Booking
{
  [Export]
  public class CheckRemindersManagementViewModel :
    BaseMasterViewModel<CheckRemindersSearchViewModel, CheckRemindersDetailViewModel, AccoNotificationListItem, DomainModel.Booking>
  {
    private ShellViewModel _shellViewModel;
    private ExportFactory<BookingMailViewModel> _bookingMailFactory;
    private INavigator _bookingMailNavigatorService;
    private bool _isNewBooking;
    private bool _isMailActive;

    public ToolbarViewModel CopyToolbarViewModel;

    [ImportingConstructor]
    public CheckRemindersManagementViewModel(ExportFactory<CheckRemindersSearchViewModel> searchFactory,
                                              ExportFactory<CheckRemindersDetailViewModel> detailFactory,
                                              ExportFactory<BookingMailViewModel> bookingMailFactory,
                                              IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                              ShellViewModel shellViewModel,
                                              IDialogManager dialogManager,
                                              ToolbarViewModel toolbar,
                                              ToolbarViewModel bottomToolbar)
      : base(searchFactory, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {
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
          (target as CheckRemindersDetailViewModel).Start(entityid);
          (target as CheckRemindersDetailViewModel).Parent = this;

        }

        );

      UpdateCommands();

    }

    public void StartDetail()
    {
      _isMailActive = false;
      StartDetail(_entityid);
    }
    protected override IRepository<DomainModel.Booking> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.Bookings;
    }


    public override async Task OnSave()
    {
      _isNewBooking = false;
    }

    public override async Task OnPostSave(bool isDelete)
    {
      var booking = (DomainModel.Booking) ActiveDetail.Entity;
      if (booking != null)
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
      try
      {
        if (ActiveEntity != null && ActiveUnitOfWork != null)
          await base.Cancel();
        else
        {
          Start();
        }
      }
      catch (Exception)
      {
        return;
      }
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
          new ToolbarAction(this, Resources.AccoBooking.but_UPDATE_BOOKING, "edit.png", Edit),
        };
        Toolbar.AddGroup(_toolbarGroup);
       
      }

      if (_isMailActive)
        Toolbar.IsVisible = false;
    }
  }

}