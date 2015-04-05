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
  public class AccoReminderSummaryViewModel : BaseScreen<AccoReminder>
  {
    [ImportingConstructor]
    public AccoReminderSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                        SystemCodeListViewModel milestoneListViewModel,
                                        IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      MilestoneList = milestoneListViewModel;
      MilestoneList.PropertyChanged += MilestoneListOnPropertyChanged;

    }

    private void MilestoneListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((AccoReminder)Entity).Milestone = MilestoneList.ShortName;
        }
      }
    }

    public SystemCodeListViewModel MilestoneList { get; set; }

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
          MilestoneList.ShortName = ((AccoReminder)Entity).Milestone;
        }
      }
    }

    public override BaseScreen<AccoReminder> Start(Entity entity)
    {
      base.Start(entity);
      MilestoneList.Start("MILESTONE");
      return this;
    }


    public override BaseScreen<AccoReminder> Start(int entityid)
    {
      base.Start(entityid);
      MilestoneList.Start("MILESTONE");
      return this;
    }


    protected override IRepository<AccoReminder> Repository()
    {
      return UnitOfWork.AccoReminders;
    }

    //protected override void OnActivate()
    //{
    //  base.OnActivate();
    //  MilestoneList.Start("MILESTONE");
    //}

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        MilestoneList.PropertyChanged -= MilestoneListOnPropertyChanged;
      }
    }


  }
}