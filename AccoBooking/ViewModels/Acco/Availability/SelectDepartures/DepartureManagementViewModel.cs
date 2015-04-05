using System.ComponentModel.Composition;
using System.Threading.Tasks;
using AccoBooking.ViewModels.Booking;
using Cocktail;
using Common.Actions;
using DomainModel;
using DomainModel.Projections;
using DomainServices;

namespace AccoBooking.ViewModels.Acco
{

  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class DepartureManagementViewModel : BaseMasterViewModel<DepartureSearchViewModel, DepartureDetailViewModel, AvailableDepartureListItem, AccoRent>
                                        
  {
    private ShellViewModel _shellViewModel;
    private ExportFactory<CreateBookingViewModel> _createBookingFactory;
    private ExportFactory<ProposeViewModel> _proposeFactory;
    private readonly INavigator _navigatorBookingService;
    private readonly INavigator _navigatorProposeService;
    
    [ImportingConstructor]
    public DepartureManagementViewModel(ExportFactory<DepartureSearchViewModel> searchFactory,
                                        ExportFactory<DepartureDetailViewModel> detailFactory,  // dummy
                                        ExportFactory<ProposeViewModel> proposeFactory,
                                        ExportFactory<CreateBookingViewModel> createBookingFactory,
                                        IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                        ShellViewModel shellViewModel,
                                        IDialogManager dialogManager,
                                        ToolbarViewModel toolbar)
      : base(searchFactory, detailFactory, unitOfWorkManager, dialogManager, toolbar, null)
    {
      _createBookingFactory = createBookingFactory;
      _proposeFactory = proposeFactory;

      _shellViewModel = shellViewModel;
      _navigatorBookingService = new Navigator(this);
      _navigatorProposeService = new Navigator(this);
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      _shellViewModel.BuildMenu("");

      Toolbar.Clear();
      _toolbarGroup = new ToolbarGroup(10);
      Toolbar.AddGroup(_toolbarGroup);

      DepartureSearchViewModel.IsDaySelected = (Parent.GetType() == typeof(SelectDepartureViewModel));
    }

    protected override IRepository<AccoRent> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoRents;
    }

    public async void Propose()
    {
      IsVisible = false;
      NotifyOfPropertyChange(() => IsVisible);

      var unitOfWork = _unitOfWorkManager.Create();
      SessionManager.CurrentAcco = await unitOfWork.Accoes.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoId);

      var departure = SearchPane.CurrentItem;

      SendMailViewModel.Context = "PERIOD";
      PreviewMailViewModel.Departure = departure;
      PreviewMailViewModel.Acco = SessionManager.CurrentAcco;

      await PreviewMailViewModel.SelectMailTemplateContent(MailTemplateType.PeriodAvailable, SessionManager.CurrentAcco.AccoOwner.LanguageId);

      if (PreviewMailViewModel.TemplateId == 0)
        await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_TEMPLATE_PROPOSE, DialogButtons.Ok);

      try
      {
        var propose = _proposeFactory.CreateExport().Value;
        await _navigatorProposeService.NavigateToAsync(
            propose.GetType(),
            target =>
            {

              (target as ProposeViewModel).Parent = this;
              if (PreviewMailViewModel.TemplateId == 0)
                (target as ProposeViewModel).Start(_parentid);
              else
                (target as ProposeViewModel).Send();

            });

      }
      catch (TaskCanceledException)
      {
        UpdateCommands();
      }
    }

    public bool IsVisible { get; set; }


    public override BaseMasterViewModel<DepartureSearchViewModel, DepartureDetailViewModel, AvailableDepartureListItem, AccoRent> Start(int parentid)
    {
      IsVisible = true;
      NotifyOfPropertyChange(() => IsVisible);
      return base.Start(parentid);
    }

    public async void Book()
    {

      try
      {

        IsVisible = false;
        NotifyOfPropertyChange(() => IsVisible);

        var departure = SearchPane.CurrentItem;

        SearchPane.CurrentItem = null;

        var createbooking = _createBookingFactory.CreateExport().Value;

        await _navigatorBookingService.NavigateToAsync(createbooking.GetType(),
          target =>
          {
            CreateBookingViewModel.Departure =  departure;
            (target as CreateBookingViewModel).Parent = this;
            (target as CreateBookingViewModel).Start(_parentid);
          });
        //target => target.Start(nameEditor.Name))
      }
      catch (TaskCanceledException)
      {
        UpdateCommands();
      }

    }

    public override Task Cancel()
    {
      if (Parent.GetType() == typeof(SelectDepartureViewModel))
        (Parent as SelectDepartureViewModel).Cancel();
      else if (Parent.GetType() == typeof(NewBookingFromDepartureViewModel))
        (Parent as NewBookingFromDepartureViewModel).Cancel();
      TryClose();
      return null;
    }
  }

}