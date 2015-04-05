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
  public class SystemGroupRepository : Repository<SystemGroup>
  {
    public SystemGroupRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.SystemGroups
                          .Where(c => c.GroupId == (int) keyValues[0]);
    }
  }

  public class SystemCodeRepository : Repository<SystemCode>
  {
    public SystemCodeRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.OverwriteChanges);
      return EntityManager.SystemCodes
                          .Where(c => c.CodeId == (int)keyValues[0])
                          .Include(c=>c.SystemGroup);
    }
  }

  public class CountryRepository : Repository<Country>
  {
    public CountryRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.Countries
        .Where(c => c.CountryId == (int)keyValues[0]);
    }
  }


  public class CurrencyRepository : Repository<Currency>
  {
    public CurrencyRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.Currencies
        .Where(c => c.CurrencyId == (int)keyValues[0]);
    }
  }

  
  public class LanguageRepository : Repository<Language>
  {
    public LanguageRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.Languages
                          .Where(c => c.LanguageId == (int)keyValues[0]);
    }
  }

  public class MailTemplateRepository : Repository<MailTemplate>
  {
    public MailTemplateRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities)base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      return EntityManager.MailTemplates
                          .Where(c => c.MailTemplateId == (int)keyValues[0]);
    }
  }

}