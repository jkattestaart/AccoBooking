using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using Caliburn.Micro;
using Cocktail;
using Common.Actions;
using Common.Errors;
using Common.Factories;
using DomainModel;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.EntityModel;
using Action = System.Action;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class UpdateBookingDialogViewModel : Conductor<IScreen>, IResult
  {
    private readonly IPartFactory<BookingManagementViewModel> _updateBookingFactory;
    private IWindowManager _windowManager;
    private IDomainUnitOfWorkManager<IDomainUnitOfWork> _unitOfWorkManager;
    private IErrorHandler _errorHandler;
    protected readonly NavigationService<BookingManagementViewModel> _navigationService;

    public static int BookingId;
    public static DateTime Arrival;
    public static DateTime Departure;
    public static AccoAvailablePeriod AvailablePeriod;
    private ToolbarGroup _bottomToolbarGroup;

    [ImportingConstructor]
    public UpdateBookingDialogViewModel(//IDomainUnitOfWorkManager<IDomainUnitOfWork> unitOfWorkManager,
                                        //BookingManagementViewModel updateBookingMaster,
                                        IPartFactory<BookingManagementViewModel> updateBookingFactory,
                                        IWindowManager windowManager,
                                        IErrorHandler errorHandler,
                                        ToolbarViewModel bottomToolbar
      )
    {
      _updateBookingFactory = updateBookingFactory;
      //UpdateBookingMaster = updateBookingMaster;
      //_unitOfWorkManager = unitOfWorkManager;
      _windowManager = windowManager;
      _errorHandler = errorHandler;
      _navigationService = new NavigationService<BookingManagementViewModel>(this);
      //BottomToolbar = bottomToolbar;
    }

    public BookingManagementViewModel UpdateBookingMaster { get; set; }

    //public ToolbarViewModel BottomToolbar { get; set; }

    //public bool CanSave
    //{
    //  get
    //  {
    //    return ActiveEntity != null && ActiveUnitOfWork.HasChanges() && !ActiveEntity.EntityAspect.EntityState.IsDeleted();
    //  }
    //}

    ///// <summary>
    ///// Cancel enabled like save
    ///// </summary>
    //public bool CanCancel
    //{
    //  get { return CanSave; }
    //}
    
    //public void Cancel()
    //{
    //  var shouldClose = ActiveEntity.EntityAspect.EntityState.IsAdded();
    //  ActiveUnitOfWork.Rollback();

    //  if (shouldClose)
    //    ActiveDetail.TryClose();
    //  //TryClose();  niet sluiten
    //}

    //public IEnumerable<IResult> Save()
    //{
    //  var arrival = ((DomainModel.Booking) ActiveDetail.Entity).Arrival;
    //  var departure = ((DomainModel.Booking) ActiveDetail.Entity).Departure;
    //  yield return
    //    ActiveUnitOfWork.AccoAvailablePeriods.FindInDataSourceAsync(
    //      a => a.Arrival >= arrival && a.Departure <= departure,
    //      null,
    //      "",
    //      res =>
    //        {
    //          foreach (var period in res)
    //            period.IsBooked = true;
    //        }
    //      );

    //  OperationResult<SaveResult> saveOperation;
    //  using (ActiveDetail.Busy.GetTicket())
    //  {
    //    yield return saveOperation = ActiveUnitOfWork.CommitAsync().ContinueOnError();

    //    yield return
    //      new ApplyPayPattern(((DomainModel.Booking) ActiveDetail.Entity).BookingId,
    //                          SessionManager.CurrentAcco.DefaultPayPatternId.Value);

    //    yield return new UpdateReminder(((DomainModel.Booking) ActiveDetail.Entity).BookingId);
    //  }

    //  if (saveOperation.HasError)
    //    _errorHandler.HandleError(saveOperation.Error);
      
    //  //TryClose();  niet standaard sluiten
    //}

    //private IDomainUnitOfWork ActiveUnitOfWork
    //{
    //  get
    //  {
    //    if (ActiveDetail != null)
    //      return ActiveDetail.UnitOfWork;
    //    return _unitOfWorkManager.Get((int)ActiveEntity.EntityAspect.EntityKey.Values[0]); 
    //  }
    //}

    //public BookingDetailViewModel ActiveDetail
    //{
    //  get { return (BookingDetailViewModel)ActiveItem; }
    //}

    //private Entity ActiveEntity
    //{
    //  get { return ActiveDetail != null ? ActiveDetail.Entity : null; }
    //}


    public void Execute(ActionExecutionContext context)
    {
     // _navigationService.NavigateToAsync(() => UpdateBookingMaster ?? _updateBookingFactory.CreatePart(),
     //                                    target =>
     //                                    {
     //                                      target.StartDetail(BookingId);

     //                                    });   
      _windowManager.ShowDialog(this);
      DisplayName = " ";
    }

    //void Entity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //  NotifyOfPropertyChange(() => CanCancel);
    //  NotifyOfPropertyChange(() => CanSave);

    //}

    public event EventHandler<ResultCompletionEventArgs> Completed;

    protected override void OnActivate()
    {
      base.OnActivate();

      //UpdateBookingMaster.StartDetail(BookingId);
      //if (BottomToolbar != null)
      //{
      //  BottomToolbar.IsVisible = true;
      //  if (_bottomToolbarGroup == null)
      //  {
      //    _bottomToolbarGroup = new ToolbarGroup(10);
      //    _bottomToolbarGroup.Add(new ToolbarAction(this, "Cancel", "cancel.png", (Action) Cancel));
      //    _bottomToolbarGroup.Add(new ToolbarAction(this, "Save", "ok.png", (Func<IEnumerable<IResult>>) Save));
      //  }
      //  BottomToolbar.AddGroup(_bottomToolbarGroup);
      //  BottomToolbar.HorizontalAlignment = 3; // right
      //}
    }

    public IEnumerable<IResult> Close()
    {
      //UpdateBookingMaster.TryClose();
      TryClose();
      IResult op = OperationResult.FromResult(true);
      
      yield return op;
      
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (Completed != null)
        Completed(this, new ResultCompletionEventArgs());
    }
  }

  [Export(typeof(IPartFactory<UpdateBookingDialogViewModel>))]
  public class UpdateBookingDialogViewModelFactory : PartFactoryBase<UpdateBookingDialogViewModel>
  {
  }


}
