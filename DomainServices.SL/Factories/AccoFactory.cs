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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cocktail;
using DomainModel;
using DomainServices.Services;

namespace DomainServices.Factories
{

  public class SequenceFactory : Factory<Sequence>
  {
    public SequenceFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<Sequence> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = new Sequence();
      EntityManager.AddEntity(sequence);
      return sequence;

      //return Coroutine.Start(CreateAsyncCore, op => op.OnComplete(onSuccess, onFail))
      //                .AsTask<Sequence>();
    }

    //protected IEnumerable<INotifyCompleted> CreateAsyncCore()
    //{
    //  var sequence = new Sequence();
    //  EntityManager.AddEntity(sequence);

    //  yield return Coroutine.Return(sequence);
    //}
  }


  public class AccoFactory : Factory<Acco>
  {
    public AccoFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<Acco> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      
       //Accomodation
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoId, cancellationToken);

      var acco = new Acco();
      EntityManager.AddEntity(acco);
      acco.AccoId = sequence.CurrentId;
      acco.ColorOwner = Colors.Red.ToString();
      acco.ColorBlock = Colors.Gray.ToString();
      acco.ColorOnline = Colors.Blue.ToString();
      acco.SendWeeklyReminders = false;

      return acco;
    }
  }

  public class AccoReminderFactory : Factory<AccoReminder>
  {
    private readonly IRepository<AccoReminder> _accoReminders;

    public AccoReminderFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
      _accoReminders = new Repository<AccoReminder>(entityManagerProvider);
    }

    public override async Task<AccoReminder> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoReminderId, cancellationToken);
      var reminders =
        await
          _accoReminders.FindInDataSourceAsync(t => t.AccoId == SessionManager.CurrentAcco.AccoId, cancellationToken);
      var reminder = reminders.OrderBy(x => x.DisplaySequence).LastOrDefault();

      var accoReminder = new AccoReminder();
      EntityManager.AddEntity(accoReminder);
      accoReminder.AccoReminderId = sequence.CurrentId;
      accoReminder.DisplaySequence = 0;
      if (reminder != null)
        accoReminder.DisplaySequence = reminder.DisplaySequence + 10;
      else
        accoReminder.DisplaySequence = 10;

      return accoReminder;
    }
  }

  
  public class AccoAdditionFactory : Factory<AccoAddition>
  {
    private readonly IRepository<Language> _accoLanguages;

    public AccoAdditionFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
      _accoLanguages = new Repository<Language>(entityManagerProvider);
    }

    public override async Task<AccoAddition> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoAdditionId, cancellationToken);

      var accoAddition = new AccoAddition();
      EntityManager.AddEntity(accoAddition);
      accoAddition.AccoAdditionId = sequence.CurrentId;
      accoAddition.IsDefaultBooked = false;
      accoAddition.DisplaySequence = 10;

      var languages = await _accoLanguages.AllInDataSourceAsync(cancellationToken);

      foreach (Language language in languages)
      {
        sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoAdditionDescriptionId, cancellationToken);

        var descr = accoAddition.AddDescription(language);
        EntityManager.AddEntity(descr);
        descr.AccoAdditionDescriptionId = sequence.CurrentId;
        descr.AccoAddition = accoAddition;
        descr.AccoAdditionId = accoAddition.AccoAdditionId;
        descr.Description = "Give description";
      }
        
      return accoAddition;
    }
  }

  public class AccoAdditionDescriptionFactory : Factory<AccoAdditionDescription>
  {
    public AccoAdditionDescriptionFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoAdditionDescription> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoAdditionDescriptionId, cancellationToken);

      var accoAdditionDescription = new AccoAdditionDescription();
      EntityManager.AddEntity(accoAdditionDescription);
      accoAdditionDescription.AccoAdditionDescriptionId = sequence.CurrentId;

      return accoAdditionDescription;
    }
  }


  public class AccoCancelConditionFactory : Factory<AccoCancelCondition>
  {
    public AccoCancelConditionFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoCancelCondition> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoCancelConditionId, cancellationToken);

      var accoCancelCondition = new AccoCancelCondition();
      EntityManager.AddEntity(accoCancelCondition);
      accoCancelCondition.AccoCancelConditionId = sequence.CurrentId;

      return accoCancelCondition;
    }
  }

  
  public class AccoOwnerFactory : Factory<AccoOwner>
  {
    public AccoOwnerFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoOwner> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoOwnerId, cancellationToken);

      var accoOwner = new AccoOwner();
      EntityManager.AddEntity(accoOwner);
      accoOwner.AccoOwnerId = sequence.CurrentId;
      //accoOwner.AccoLanguage = accoLanguage1;

      return accoOwner;
    }
  }


  public class AccoPayPatternFactory : Factory<AccoPayPattern>
  {
    public AccoPayPatternFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoPayPattern> CreateAsync(CancellationToken cancellationToken)
    {
     var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoPayPatternId, cancellationToken);

      var accoPayPattern = new AccoPayPattern();
      EntityManager.AddEntity(accoPayPattern);
      accoPayPattern.AccoPayPatternId = sequence.CurrentId;
      accoPayPattern.IsAdditionInLastPayment = false;
      accoPayPattern.IsDepositInLastPayment = false;

      return accoPayPattern;
    }
  }



  public class AccoPayPatternPaymentFactory : Factory<AccoPayPatternPayment>
  {
    public AccoPayPatternPaymentFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoPayPatternPayment> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoPayPatternPaymentId, cancellationToken);

      var accoPayPatternPayment = new AccoPayPatternPayment();
      EntityManager.AddEntity(accoPayPatternPayment);
      accoPayPatternPayment.AccoPayPatternPaymentId = sequence.CurrentId;

      return accoPayPatternPayment;
    }
  }


  public class AccoRentFactory : Factory<AccoRent>
  {
    public AccoRentFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoRent> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoRentId, cancellationToken);

      var rent = new AccoRent();
      EntityManager.AddEntity(rent);
      rent.AccoRentId = sequence.CurrentId;
      rent.IsActive = true;

      return rent;
    }
  }

  public class AccoSeasonFactory : Factory<AccoSeason>
  {
    public AccoSeasonFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoSeason> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync("AccoSeasonId", cancellationToken);

      var season = new AccoSeason();
      EntityManager.AddEntity(season);
      season.AccoSeasonId = sequence.CurrentId;

      return season;
    }
  }

  public class AccoSpecialOfferFactory : Factory<AccoSpecialOffer>
  {
    public AccoSpecialOfferFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoSpecialOffer> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoSpecialOfferId, cancellationToken);

      var specialOffer = new AccoSpecialOffer();
      EntityManager.AddEntity(specialOffer);
      specialOffer.AccoSpecialOfferId = sequence.CurrentId;

      return specialOffer;
    }
  }

  public class AccoTrusteeFactory : Factory<AccoTrustee>
  {
    public AccoTrusteeFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoTrustee> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoTrusteeId, cancellationToken);

      var trustee = new AccoTrustee();
      EntityManager.AddEntity(trustee);
      trustee.AccoTrusteeId = sequence.CurrentId;

      return trustee;
    }
  }

  public class AccoNotificationFactory : Factory<AccoNotification>
  {
    public AccoNotificationFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<AccoNotification> CreateAsync(CancellationToken cancellationToken)
    {
      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.AccoTrusteeId, cancellationToken);

      var notification = new AccoNotification();
      EntityManager.AddEntity(notification);
      notification.AccoNotificationId = sequence.CurrentId;

      return notification;
    }
  }


}