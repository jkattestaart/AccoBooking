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

using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Repositories
{
  public class SequenceRepository : Repository<Sequence>
  {
    public SequenceRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.Sequences.Take(1);
    }
  }

  public class AccoRepository : Repository<Acco>
  {
    public AccoRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.OverwriteChanges);
      return EntityManager.Accoes
                          .Where(c => c.AccoId == (int) keyValues[0])
                          .Include(c => c.AccoDescriptions)
                          .Include(c => c.AccoOwner)
                          .Include(c => c.AccoReminders)
                          .Include(c => c.AccoAdditions)
                          .Include(c => c.AccoCancelConditions)
                          .Include(c => c.AccoPayPatterns)
                          .Include(c => c.AccoPayPattern)
                          .Include(c => c.AccoRents)
                          .Include(c => c.AccoRent)
                          .Include(c => c.AccoSeasons)
                          .Include(c => c.Country)
                          .Include(c => c.AccoOwner.Country)
                          .Include(c => c.AccoOwner.Language)
                          .Include(c => c.Bookings)
                          .Include(c => c.AccoSpecialOffers);                          
    }
  }


  public class AccoReminderRepository : Repository<AccoReminder>
  {
    public AccoReminderRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoReminders
                          .Where(c => c.AccoReminderId == (int) keyValues[0])
                          .Include(c => c.Acco);
    }
  }


  public class AccoAdditionRepository : Repository<AccoAddition>
  {
    public AccoAdditionRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoAdditions
        .Where(c => c.AccoAdditionId == (int) keyValues[0])
        .Include(c => c.Acco)
        .Include(c => c.AccoAdditionDescriptions);
    }
  }


  public class AccoAdditionDescriptionRepository : Repository<AccoAdditionDescription>
  {
    public AccoAdditionDescriptionRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoAdditionDescriptions
                          .Where(c => c.AccoAdditionDescriptionId == (int) keyValues[0])
                          .Include(c => c.Language);
    }
  }

  public class AccoCancelConditionRepository : Repository<AccoCancelCondition>
  {
    public AccoCancelConditionRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.AccoCancelConditions
                          .Where(c => c.AccoCancelConditionId == (int) keyValues[0]);
    }
  }




  public class AccoOwnerRepository : Repository<AccoOwner>
  {
    public AccoOwnerRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoOwners
                          .Where(c => c.AccoOwnerId == (int) keyValues[0])
                          .Include(c => c.Country)
                          .Include(c => c.Language)
                          .Include(c => c.AccoTrustees);
    }
  }


  public class AccoPayPatternRepository : Repository<AccoPayPattern>
  {
    public AccoPayPatternRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.AccoPayPatterns
                          .Where(c => c.AccoPayPatternId == (int) keyValues[0]);
    }
  }


  public class AccoPayPatternPaymentRepository : Repository<AccoPayPatternPayment>
  {
    public AccoPayPatternPaymentRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoPayPatternPayments
                          .Where(c => c.AccoPayPatternPaymentId == (int) keyValues[0])
                          .Include(c => c.AccoPayPattern.Acco);
    }
  }

  public class AccoRentRepository : Repository<AccoRent>
  {
    public AccoRentRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoRents.Where(c => c.AccoRentId == (int) keyValues[0])
                                    .Include(c=>c.AccoSeasons);
    }
  }

  public class AccoSeasonRepository : Repository<AccoSeason>
  {
    public AccoSeasonRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoSeasons.Where(c => c.AccoSeasonId == (int)keyValues[0])
                                      .Include(c => c.AccoRent);
    }
  }

  public class AccoSpecialOfferRepository : Repository<AccoSpecialOffer>
  {
    public AccoSpecialOfferRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoSpecialOffers.Where(c => c.AccoSpecialOfferId == (int) keyValues[0])
        .Include(c => c.Acco)
        .Include(c => c.Language);
    }
  }

  public class AccoTrusteeRepository : Repository<AccoTrustee>
  {
    public AccoTrusteeRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoTrustees.Where(c => c.AccoTrusteeId == (int) keyValues[0])
        .Include(c => c.AccoOwner);
    }
  }

  public class AccoNotificationRepository : Repository<AccoNotification>
  {
    public AccoNotificationRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.AccoNotifications.Where(c => c.AccoNotificationId == (int)keyValues[0])
        .Include(c => c.AccoOwner);
    }
  }

}

