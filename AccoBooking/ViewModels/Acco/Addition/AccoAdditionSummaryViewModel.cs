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
using Cocktail;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoAdditionSummaryViewModel : BaseScreen<AccoAddition>
  {
    [ImportingConstructor]
    public AccoAdditionSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                        AccoAdditionDescriptionListViewModel descriptions,
                                        SystemCodeListViewModel unitListViewModel,
                                        IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      Descriptions = descriptions;
      UnitList = unitListViewModel;
      UnitList.PropertyChanged += UnitListOnPropertyChanged;

    }

    public AccoAdditionDescriptionListViewModel Descriptions { get; set; }

    private void UnitListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((AccoAddition)Entity).Unit = UnitList.ShortName;
        }
      }
    }

    public SystemCodeListViewModel UnitList { get; set; }

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
          UnitList.ShortName = ((AccoAddition)Entity).Unit;
          NotifyOfPropertyChange(() => Descriptions);

          //@@@ JKT forceer lezen talen...
          foreach (var description in ((AccoAddition) Entity).AccoAdditionDescriptions)
          {
            var a = description.Language;
            
          }
          
        }
      }
    }

    
    protected override IRepository<AccoAddition> Repository()
    {
      return UnitOfWork.AccoAdditions;
    }

    public override BaseScreen<AccoAddition> Start(int entityid)
    {
      Descriptions.Start(entityid);
      return base.Start(entityid);
    }


    protected override void OnActivate()
    {
      base.OnActivate();
      UnitList.Start("UNIT");
      
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        UnitList.PropertyChanged -= UnitListOnPropertyChanged;
      }
    }


  }
}