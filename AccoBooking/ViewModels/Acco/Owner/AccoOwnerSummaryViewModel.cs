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

using System.ComponentModel;
using System.ComponentModel.Composition;
using AccoBooking.ViewModels.General;
using Cocktail;
using Common.Actions;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoOwnerSummaryViewModel : BaseScreen<DomainModel.Acco>

  {
    protected ToolbarGroup _toolbarGroup;

    [ImportingConstructor]
    public AccoOwnerSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                     CountryListViewModel countryList,
                                     LanguageListViewModel languageList,
                                     CountryListViewModel publicCountryList,
                                     IDialogManager dialogManager
      )
      : base(unitOfWorkManager, dialogManager)
    {
      CountryList = countryList;
      CountryList.PropertyChanged += CountryListOnPropertyChanged;

      LanguageList = languageList;
      LanguageList.PropertyChanged += LanguageListOnPropertyChanged;

      PublicCountryList = publicCountryList;
      PublicCountryList.PropertyChanged += PublicCountryListOnPropertyChanged;

    }


    //public IEnumerable<IResult> Save()
    //{
    //  //todo username bewaren in Security.User
    //}


    void AccoOwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Name")
        ((DomainModel.Acco)Entity).AccoOwner.PublicName = ((DomainModel.Acco)Entity).AccoOwner.Name;
      if (e.PropertyName == "Email")
        ((DomainModel.Acco)Entity).AccoOwner.PublicEmail = ((DomainModel.Acco)Entity).AccoOwner.Email;
      if (e.PropertyName == "Phone")
        ((DomainModel.Acco)Entity).AccoOwner.PublicPhone = ((DomainModel.Acco)Entity).AccoOwner.Phone;
      NotifyOfPropertyChange(() => Entity);
      NotifyOfPropertyChange(() => CanSave);
    }

    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(sender, e);
      NotifyOfPropertyChange(() => CanSave);
    }
    
    private void CountryListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).AccoOwner.CountryId = CountryList.ItemId;
          ((DomainModel.Acco)Entity).AccoOwner.PublicCountryId = CountryList.ItemId;
           PublicCountryList.ItemId = CountryList.ItemId;
        }
      }
    }

    private void LanguageListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).AccoOwner.LanguageId = LanguageList.ItemId;
        }
      }
    }

    private void PublicCountryListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).AccoOwner.PublicCountryId = PublicCountryList.ItemId;
        }
      }
    }

    public CountryListViewModel CountryList { get; set; }
    public LanguageListViewModel LanguageList { get; set; }
    public CountryListViewModel PublicCountryList { get; set; }


    public override Entity Entity
    {
      get
      {
        return base.Entity;
      }
      set
      {
        if (_entity != null)
          ((DomainModel.Acco)Entity).AccoOwner.PropertyChanged -= AccoOwnerPropertyChanged;

        base.Entity = value;
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).AccoOwner.PropertyChanged += AccoOwnerPropertyChanged;

          if (((DomainModel.Acco)Entity).AccoOwner.CountryId.HasValue)
            CountryList.ItemId = ((DomainModel.Acco)Entity).AccoOwner.CountryId.Value;
          LanguageList.ItemId = ((DomainModel.Acco)Entity).AccoOwner.LanguageId;
          if (((DomainModel.Acco)Entity).AccoOwner.PublicCountryId.HasValue)
            PublicCountryList.ItemId = ((DomainModel.Acco)Entity).AccoOwner.PublicCountryId.Value;

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
      CountryList.Start(((DomainModel.Acco)Entity).AccoId);
      LanguageList.Start(((DomainModel.Acco)Entity).AccoId);
      PublicCountryList.Start(((DomainModel.Acco)Entity).AccoId);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        CountryList.PropertyChanged -= CountryListOnPropertyChanged;
        LanguageList.PropertyChanged -= LanguageListOnPropertyChanged;
        PublicCountryList.PropertyChanged -= PublicCountryListOnPropertyChanged;
      }
    }

  }
}