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
using System.Linq;
using Cocktail;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingReminderSummaryViewModel : BaseScreen<BookingReminder>
  {
    private bool _activityFlows = true;

    [ImportingConstructor]
    public BookingReminderSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                           SystemCodeListViewModel milestoneListViewModel,
                                           IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      MilestoneList = milestoneListViewModel;
      MilestoneList.PropertyChanged += MilestoneListOnPropertyChanged;
    }

    void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Offset")
      {
        ValueChangedActivityFlows(false);
      }
    }

    private void MilestoneListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((BookingReminder)Entity).Milestone = MilestoneList.ShortName;

          _activityFlows = ((BookingReminder)Entity).Milestone != "";
          ValueChangedActivityFlows(false);
          NotifyOfPropertyChange(() => ActivityFlows);

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
          if (_entity != null) Entity.PropertyChanged -= EntityPropertyChanged;
          Entity.PropertyChanged += EntityPropertyChanged;
          MilestoneList.ShortName = ((BookingReminder)Entity).Milestone;
          _activityFlows = ((BookingReminder)Entity).Milestone != "";
          ValueChangedActivityFlows(false);
          NotifyOfPropertyChange(() => ActivityFlows);
        }
      }
    }

    private void ValueChangedActivityFlows(bool changedByUser)
    {
      // Initialize depending fields
      if (!_activityFlows)
      {
        // The activity flows not with booking data and is fixed on one date
        if (changedByUser)
          MilestoneList.ShortName = "";

        // The activity flows not with booking data and is fixed on one date
        ((BookingReminder)Entity).Offset = 0;
        if (((BookingReminder)Entity).Due == null)
          ((BookingReminder)Entity).Due = DateTime.Today; 
        
      }
      else
      {
        var offset = ((BookingReminder) Entity).Offset;

        var payments = ((BookingReminder) Entity).Booking.BookingPayments;
        BookingPayment payment;

        // The activity flows with booking data
        switch (MilestoneList.ShortName)
        {
          case "BOOKING":
            ((BookingReminder) Entity).IsDue = true;
            ((BookingReminder) Entity).Due = ((BookingReminder) Entity).Booking.Booked.AddDays(offset);
            break;

          case "FIRST-PAYMENT":
            // check if booking has a non scheduled payment by the guest. 
            // If there is such a payment, the activity is due. If there is not, the activity isn't due
            if (payments != null)
            {
              payment = payments.FirstOrDefault(p => !p.IsScheduledPayment && p.IsPaymentByGuest);
              if (payment != null)
              {
                ((BookingReminder) Entity).IsDue = true;
                ((BookingReminder) Entity).Due = payment.Due;
              }
              else
              {
                ((BookingReminder) Entity).IsDue = false;
                ((BookingReminder) Entity).Due = null;
              }
            }
            else
            {
              ((BookingReminder) Entity).IsDue = false;
              ((BookingReminder) Entity).Due = null;
            }

            break;

          case "LAST-PAYMENT":
            // check if booking has status PAID. If true, the activity is due. If not true, the activity isn't due
            if (((BookingReminder) Entity).Booking.Status == "PAID")
            {
              ((BookingReminder) Entity).IsDue = true;
              // find last not scheduled payment by guest to calculate due date
              //TODO
              if (payments != null)
              {
                payment =
                  payments.Where(p => !p.IsScheduledPayment && p.IsPaymentByGuest)
                    .OrderByDescending(p => p.Due)
                    .FirstOrDefault();
                ((BookingReminder)Entity).IsDue = true;
                ((BookingReminder) Entity).Due = payment.Due;
              }
              else
              {
                ((BookingReminder) Entity).IsDue = false;
                ((BookingReminder) Entity).Due = null;
              }
            }
            break;

          case "ARRIVAL":
            ((BookingReminder)Entity).IsDue = true;
            ((BookingReminder)Entity).Due = ((BookingReminder)Entity).Booking.Arrival.AddDays(offset);
            break;

          case "DEPARTURE":
            ((BookingReminder)Entity).IsDue = true;
            ((BookingReminder)Entity).Due = ((BookingReminder)Entity).Booking.Departure.AddDays(offset);
            break;
        }

        NotifyOfPropertyChange(() => Entity);
      }

    }

    public bool ActivityFlows
    {
      get { return _activityFlows; }
      set
      {
        _activityFlows = value;

        // Refresh depending fields
        ValueChangedActivityFlows(true);
        
        NotifyOfPropertyChange(() => ActivityFlows);
        
      }
    }

    // Done kan alleen true worden bij wijzigen activity en IsDue = true
    // in alle andere gevallen checkbox niet laten zien
    // 
    protected override IRepository<BookingReminder> Repository()
    {
      return UnitOfWork.BookingReminders;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      MilestoneList.Start("MILESTONE");
    }

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