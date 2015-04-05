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
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoSpecialOfferSummaryViewModel : BaseScreen<AccoSpecialOffer>
  {
    [ImportingConstructor]
    public AccoSpecialOfferSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                            LanguageListViewModel languageList,
                                            IDialogManager dialogManager)
      : base(unitOfWorkManager,  dialogManager)
    {
      LanguageList = languageList;
      LanguageList.PropertyChanged += LanguageListPropertyChanged;
    }

    void LanguageListPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((AccoSpecialOffer)Entity).LanguageId = LanguageList.ItemId;
        }
      }
    }

    public LanguageListViewModel LanguageList { get; set; }

    protected override IRepository<AccoSpecialOffer> Repository()
    {
      return UnitOfWork.AccoSpecialOffers;
    }

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
          LanguageList.ItemId = ((AccoSpecialOffer)Entity).LanguageId;
        }
      }
    }
    protected override void OnActivate()
    {
      base.OnActivate();
      LanguageList.Start(0);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        LanguageList.PropertyChanged -= LanguageListPropertyChanged;
      }
    }
  }
}