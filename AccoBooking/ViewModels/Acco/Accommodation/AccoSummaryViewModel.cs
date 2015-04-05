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
using System.ComponentModel;
using System.ComponentModel.Composition;
using AccoBooking.ViewModels.General;
using Cocktail;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoSummaryViewModel : BaseScreen<DomainModel.Acco>
  {
    private ShellViewModel _shell;
    private int _zoom = 10;

    [ImportingConstructor]
    public AccoSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                CountryListViewModel countryList,
                                ShellViewModel shellViewModel,
                                IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      _shell = shellViewModel;

      CountryList = countryList;
      CountryList.PropertyChanged += CountryListOnPropertyChanged;

    }

    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(sender, e);
      NotifyOfPropertyChange(() => CanSave);
    }


    public async void Coordinates()
    {
      var address = ((DomainModel.Acco) Entity).Address1 + " " 
                  + ((DomainModel.Acco) Entity).Address2 + " " 
                  + ((DomainModel.Acco) Entity).Address3;
      address = address.Trim(' ');
      if (!string.IsNullOrEmpty(address))
      {
        var coordinates = await GeoCodingService.ExecuteAsync(address);
        if (!string.IsNullOrEmpty(coordinates))
        {
          ((DomainModel.Acco) Entity).Latitude = decimal.Parse(coordinates.Split(';')[0]);
          ((DomainModel.Acco) Entity).Longitude = decimal.Parse(coordinates.Split(';')[1]);
        }
        NotifyOfPropertyChange(() => Entity);
      }
    }

    private void CountryListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).CountryId = CountryList.ItemId;
        }
      }
    }

    public CountryListViewModel CountryList { get; set; }

    public override Entity Entity
    {
      get
      {
        return base.Entity;
      }
      set
      {
        base.Entity = value;
        if (Entity != null)
        {
          if (((DomainModel.Acco)Entity).CountryId.HasValue)
            CountryList.ItemId = ((DomainModel.Acco)Entity).CountryId.Value;

          ((DomainModel.Acco) Entity).Zoom = _zoom;

          //@@@ JKT forceer lezen talen...
          foreach (var description in ((DomainModel.Acco)Entity).AccoDescriptions)
          {
            var a = description.Language;

          }
        }
      }
    }

   
    protected override IRepository<DomainModel.Acco> Repository()
    {
      return UnitOfWork.Accoes;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      CountryList.Start(0);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        CountryList.PropertyChanged -= CountryListOnPropertyChanged;
      }
    }

    public async void AddAccommodation()
    {
      await _shell.NavigateToWorkSpace(Resources.AccoBooking.ws_ADD_ACCO);
    }

    public async void CopyAccommodation()
    {
      await _shell.NavigateToWorkSpace(Resources.AccoBooking.ws_COPY_ACCO);
    }

    public async void RemoveAccommodation()
    {
      await _shell.NavigateToWorkSpace(Resources.AccoBooking.ws_REMOVE_ACCO);
    }

    public void ZoomChanged(int value)
    {
      _zoom = value;
      if (Entity !=null)
        ((DomainModel.Acco) Entity).Zoom = _zoom;
      NotifyOfPropertyChange(()=>Entity);
    }
  }
}