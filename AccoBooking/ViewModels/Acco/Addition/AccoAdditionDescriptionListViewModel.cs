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
    public class AccoAdditionDescriptionListViewModel : BaseScreen<AccoAddition>
    {
        private BindableCollection<AccoAdditionDescriptionItemViewModel> _descriptions;

        [ImportingConstructor]
        public AccoAdditionDescriptionListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                                    IDialogManager dialogManager)
          : base(unitOfWorkManager, dialogManager)
        {
        }

        protected override IRepository<AccoAddition> Repository()
        {
          return UnitOfWork.AccoAdditions;
        }

        public override Entity Entity
        {
          get { return base.Entity; }
          set
          {
            base.Entity = value;
            if (base.Entity != null)
              ((AccoAddition)base.Entity).AccoAdditionDescriptions.CollectionChanged -= AccoAdditionDescriptionsCollectionChanged;

            ClearAccoAdditionDescriptions();

            if (value != null)
            {
              AccoAdditionDescriptions =
                new BindableCollection<AccoAdditionDescriptionItemViewModel>(
                  ((AccoAddition)value).AccoAdditionDescriptions.ToList()
                                       .Select(a => new AccoAdditionDescriptionItemViewModel(a)));
              ((AccoAddition)value).AccoAdditionDescriptions.CollectionChanged += AccoAdditionDescriptionsCollectionChanged;
            }
          }
        }


        public BindableCollection<AccoAdditionDescriptionItemViewModel> AccoAdditionDescriptions
        {
          get { return _descriptions; }
            set
            {
              _descriptions = value;
                NotifyOfPropertyChange(() => AccoAdditionDescriptions);
            }
        }

        private void AccoAdditionDescriptionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in
                    e.OldItems.Cast<AccoAdditionDescription>().Select(a => AccoAdditionDescriptions.First(i => i.Item == a)))
                {
                  AccoAdditionDescriptions.Remove(item);
                    item.Dispose();
                }
            }

            if (e.NewItems != null)
              e.NewItems.Cast<AccoAdditionDescription>()
                    .ForEach(a => AccoAdditionDescriptions.Add(new AccoAdditionDescriptionItemViewModel(a)));

        }


        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (!close) return;

            ClearAccoAdditionDescriptions();
        }

        private void ClearAccoAdditionDescriptions()
        {
          if (AccoAdditionDescriptions == null) return;

            // Clean up to avoid memory leaks
          AccoAdditionDescriptions.ForEach(i => i.Dispose());
          AccoAdditionDescriptions.Clear();
        }
    }
}