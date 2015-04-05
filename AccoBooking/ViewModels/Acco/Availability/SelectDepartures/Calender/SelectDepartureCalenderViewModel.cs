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
using AccoBooking.ViewModels.Booking;
using Caliburn.Micro;
using Cocktail;
using Common.Messages;
using DomainModel;
using DomainServices;

namespace AccoBooking.ViewModels.Acco
{

  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SelectDepartureCalenderViewModel : Conductor<IScreen>,
                                                      IDiscoverableViewModel,
                                                      IHandle<EntityChangedMessage>
  {
    private readonly ExportFactory<SelectDepartureListViewModel> _availablePeriodListFactory;
    private readonly ExportFactory<UpdateBookingViewModel> _bookingManagementFactory;
    private readonly ExportFactory<SelectDepartureViewModel> _selectDepartureFactory;
    private readonly IDialogManager _dialogManager;
    private readonly INavigator _navigatorAvailablePeriodListService;
    private readonly INavigator _navigatorBookingService;
    private readonly INavigator _navigatorSelectAvailablePeriodService;
    private readonly IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    private int _entityid;
    private ShellViewModel _shellViewModel;

    [ImportingConstructor]
    public SelectDepartureCalenderViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                                ExportFactory<SelectDepartureListViewModel> availablePeriodListFactory,
                                                ExportFactory<UpdateBookingViewModel> bookingManagementFactory,
                                                ExportFactory<SelectDepartureViewModel> selectDepartureFactory,
                                                ShellViewModel shellViewModel,
                                                IDialogManager dialogManager)
    {
      _availablePeriodListFactory = availablePeriodListFactory;
      _bookingManagementFactory = bookingManagementFactory;
      _selectDepartureFactory = selectDepartureFactory;
      _shellViewModel = shellViewModel;
      _unitOfWorkManager = unitOfWorkManager;
      _dialogManager = dialogManager;
      _navigatorAvailablePeriodListService = new Navigator(this);
      _navigatorBookingService = new Navigator(this);
      _navigatorSelectAvailablePeriodService = new Navigator(this);
    }

    public SelectDepartureListViewModel AvailablePeriodList { get; set; }
    public UpdateBookingViewModel BookingManagement { get; set; }
    SelectDepartureViewModel SelectDeparture { get; set; }

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

    public void Back()
    {
      Start();
    }

    public async void Start()
    {
      _entityid = SessionManager.CurrentAcco.AccoId;
      _shellViewModel.BuildMenu("");

      try
      {
        var calendar = AvailablePeriodList ?? _availablePeriodListFactory.CreateExport().Value;

        await _navigatorAvailablePeriodListService.NavigateToAsync(calendar.GetType(),
          target =>
          {
            if (AvailablePeriodList != target)
              AvailablePeriodList = (target as SelectDepartureListViewModel);
            AvailablePeriodList.Parent = this; //Caliburn zet deze nu niet 
            ((IActivate) target).Activate();
            AvailablePeriodList.Start(_entityid);
          });
      }
      catch (TaskCanceledException)
      {

      }
    }

  }

}