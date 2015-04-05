// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using DomainModel;
using DomainServices;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;


namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoDescriptionListViewModel : BaseScreen<DomainModel.Acco>
  {
    private string _propertyName;

    private BindableCollection<AccoDescriptionItemViewModel> _descriptions;

    [ImportingConstructor]
    public AccoDescriptionListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                                IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
    }

    protected override IRepository<DomainModel.Acco> Repository()
    {
      return UnitOfWork.Accoes;
    }

    public override Entity Entity
    {
      get { return base.Entity; }
      set
          {
            base.Entity = value;
            if (base.Entity != null)
              ((DomainModel.Acco)base.Entity).AccoDescriptions.CollectionChanged -= AccoDescriptionsCollectionChanged;

            ClearAccoDescriptions();

            if (value != null)
            {
              Descriptions =
                new BindableCollection<AccoDescriptionItemViewModel>();
              foreach (var descripton in ((DomainModel.Acco)value).AccoDescriptions)
              {
                if (descripton.PropertyName == _propertyName)
                  Descriptions.Add(new AccoDescriptionItemViewModel(descripton));
              }
             
              ((DomainModel.Acco)value).AccoDescriptions.CollectionChanged += AccoDescriptionsCollectionChanged;
            }
          }
    }


    public BindableCollection<AccoDescriptionItemViewModel> Descriptions
    {
      get { return _descriptions; }
      set
      {
        _descriptions = value;
        NotifyOfPropertyChange(() => Descriptions);
      }
    }

    private void AccoDescriptionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (var item in
            e.OldItems.Cast<AccoDescription>().Select(a => Descriptions.First(i => i.Item == a)))
        {
          Descriptions.Remove(item);
          item.Dispose();
        }
      }

      if (e.NewItems != null)
      {
        foreach (var item in  e.NewItems.Cast<AccoDescription>())
        {
           if (item.PropertyName == _propertyName)
            Descriptions.Add(new AccoDescriptionItemViewModel(item));
        }
      }

    }

    public BaseScreen<DomainModel.Acco> Start(int entityid, string propertyName)
    {
      _propertyName = propertyName;
      LoadDataAsync(entityid);
      return this;
    }


    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);

      if (!close) return;

      ClearAccoDescriptions();
    }

    private void ClearAccoDescriptions()
    {
      if (Descriptions == null) return;

      // Clean up to avoid memory leaks
      Descriptions.ForEach(i => i.Dispose());
      Descriptions.Clear();
    }
  }
}