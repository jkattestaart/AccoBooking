using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using DomainModel;
using DomainServices;
using DomainServices.Services;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class CopyAccoRentViewModel : Screen
  {
    private IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    private int _sourceYear;
    private int _destYear;

    [ImportingConstructor]
    public CopyAccoRentViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager, 
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

    public int SourceYear
    {
      get { return _sourceYear; }
      set
      {
        _sourceYear = value;
        NotifyOfPropertyChange(() => CanCopy);
      }
    }

    public int DestYear
    {
      get { return _destYear; }
      set
      {
        _destYear = value;
        NotifyOfPropertyChange(() => CanCopy);
      }
    }

    public AccoListViewModel DestAccoList { get; set; }

    public AccoListViewModel SourceAccoList { get; set; }

    protected override void OnActivate()
    {
      SourceYear = DateTime.Now.Year;
      DestYear = DateTime.Now.Year;
      SourceAccoList.Start(SessionManager.CurrentOwner.AccoOwnerId);
      DestAccoList.Start(SessionManager.CurrentOwner.AccoOwnerId);

      NotifyOfPropertyChange(() => CanCopy);
      base.OnActivate();
    }

    public bool CanCopy
    {
      get
      {
        return SourceAccoList.ItemId != 0 && DestAccoList.ItemId != 0 && (SourceAccoList.ItemId != DestAccoList.ItemId || SourceYear != DestYear);
      }
    }

    public async void Copy()
    {
      using (Busy.GetTicket())
      {
        await CopyAccoRentService.ExecuteAsync(SourceAccoList.ItemId, SourceYear, DestAccoList.ItemId, DestYear);
      }
    }
  }

}
