using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using DomainModel;
using DomainServices;
using DomainServices.Services;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class CopyAccommodationViewModel : Screen
  {
    private IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;

    [ImportingConstructor]
    public CopyAccommodationViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager, 
                                      AccoListViewModel source,
                                      AccoListViewModel dest,
                                      ToolbarViewModel toolbar)
    {
      SourceAccoList = source;
      SourceAccoList.PropertyChanged += SourceAccoList_PropertyChanged;
      DestAccoList = dest;
      DestAccoList.PropertyChanged += DestAccoList_PropertyChanged;
      _unitOfWorkManager = unitOfWorkManager;

      Busy = new BusyWatcher();

    }

    /// <summary>
    /// Busy indicator
    /// </summary>
    public IBusyWatcher Busy { get; private set; }

    void DestAccoList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CanCopy);
    }

    void SourceAccoList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CanCopy);
    }

    public AccoListViewModel DestAccoList { get; set; }

    public AccoListViewModel SourceAccoList { get; set; }

    protected override void OnActivate()
    {
      SourceAccoList.Start(SessionManager.CurrentOwner.AccoOwnerId);
      DestAccoList.Start(SessionManager.CurrentOwner.AccoOwnerId);

      NotifyOfPropertyChange(() => CanCopy);
      base.OnActivate();
    }

    public bool CanCopy
    {
      get
      {
        return SourceAccoList.ItemId != 0 && DestAccoList.ItemId != 0 && SourceAccoList.ItemId != DestAccoList.ItemId;
      }
    }

    public async void Copy()
    {
      using (Busy.GetTicket())
      {
        await CopyAccoService.ExecuteAsync(SourceAccoList.ItemId, DestAccoList.ItemId);
      }
    }
  }
}
