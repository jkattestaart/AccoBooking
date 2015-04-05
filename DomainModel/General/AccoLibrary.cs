using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using DomainModel.Projections;
using IdeaBlade.EntityModel;

namespace DomainModel
{
  /// <summary>
  /// Library routines Acco
  /// </summary>
  public class AccoLibrary
  {
    private static Language[] _languages = new Language[3];
    private static int _fromaccoid;
    private static int _accoid;
    private static string _subscribe;
    private static int _fromyear;
    private static int _toyear;
   
    /// <summary>
    /// Clone an entity
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    public static void Clone(Entity source, Entity destination)
    {
      Clone(source, destination, new string[]{});
    }

    /// <summary>
    /// Clone an entity withe xclusions
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="properties"></param>
    public static void Clone(Entity source, Entity destination, string[] exclusions)
    {
      //clone the entity (except the primary key)
      foreach (var p in source.EntityAspect.EntityMetadata.DataProperties.Where(x => !x.IsPartOfKey))
      {
        if (exclusions.ToList().Contains(p.Name))
          continue;
        var dp = destination.EntityAspect.EntityMetadata.DataProperties.FirstOrDefault(e => e.Name == p.Name);
        if (dp.IsForeignKeyProperty) 
          continue;

        dp.SetValue(destination, p.GetValue(source, EntityVersion.Current));
      }
    }

    /// <summary>
    /// Dutch description for language of sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string LanguageDescriptionNL(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "Nederlands";

        case 20:
          return "Engels";

        case 30:
          return "Duits";
      }

      return "";
    }

    /// <summary>
    /// English description for language of sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string LanguageDescriptionEN(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "English";

        case 20:
          return "German";

        case 30:
          return "Dutch";
      }

      return "";
    }

    /// <summary>
    /// German description for language of sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string LanguageDescriptionDE(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "Deutsch";

        case 20:
          return "Niederländisch";

        case 30:
          return "Englisch";
      }

      return "";
    }

    /// <summary>
    /// Description for language of sequence in language of subscribe
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string LanguageDescription(int displaySequence)
    {
      switch (_subscribe)
      {
        case "NL":
          return LanguageDescriptionNL(displaySequence);
        case "EN":
          return LanguageDescriptionEN(displaySequence);
        case "DE":
          return LanguageDescriptionDE(displaySequence);
      }

      return "";
    }

    /// <summary>
    /// Create a language
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static Language CreateLanguage(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.LanguageId);

      var language = new Language();
      em.AddEntity(language);
      language.LanguageId = sequence;

      return language;
    }

    /// <summary>
    /// Create a language for sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Language CreateLanguage(AccoBookingEntities em, int displaySequence)
    {
      var language = CreateLanguage(em);
      language.Description = LanguageDescription(displaySequence);
      language.DisplaySequence = displaySequence;

      return language;
    }

    /// <summary>
    /// Find a language for a Dutch sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Language FindLanguageNL(AccoBookingEntities em, int displaySequence)
    {
      switch (displaySequence)
      {
        case 10:
          return em.Languages.FirstOrDefault(l => l.Description == "Dutch");
        case 20:
          return em.Languages.FirstOrDefault(l => l.Description == "English");
        case 30:
          return em.Languages.FirstOrDefault(l => l.Description == "German");
        case 40:
          return em.Languages.FirstOrDefault(l => l.Description == "French");
      }
      return null;
    }

    /// <summary>
    /// Find a language for a English sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Language FindLanguageEN(AccoBookingEntities em, int displaySequence)
    {
      switch (displaySequence)
      {
        case 10:
          return em.Languages.FirstOrDefault(l => l.Description == "English");
        case 20:
          return em.Languages.FirstOrDefault(l => l.Description == "German");
        case 30:
          return em.Languages.FirstOrDefault(l => l.Description == "Dutch");
        case 40:
          return em.Languages.FirstOrDefault(l => l.Description == "French");
      }
      return null;
    }

    /// <summary>
    /// Find a language for a German sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Language FindLanguageDE(AccoBookingEntities em, int displaySequence)
    {
      switch (displaySequence)
      {
        case 10:
          return em.Languages.FirstOrDefault(l => l.Description == "German");
        case 20:
          return em.Languages.FirstOrDefault(l => l.Description == "Dutch");
        case 30:
          return em.Languages.FirstOrDefault(l => l.Description == "English");
        case 40:
          return em.Languages.FirstOrDefault(l => l.Description == "French");
      }
      return null;
    }

    /// <summary>
    /// Find a language for the sequence in the language of the subscribe
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Language FindLanguage(AccoBookingEntities em, int displaySequence)
    {
      switch (_subscribe)
      {
        case "NL":
          return FindLanguageNL(em, displaySequence);
        case "EN":
          return FindLanguageEN(em, displaySequence);
        case "DE":
          return FindLanguageDE(em, displaySequence);
      }
      return null;
    }

    /// <summary>
    /// Dutch description for country of sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string CountryDescriptionNL(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "Nederland";

        case 20:
          return "Duitsland";

        case 30:
          return "België";

        case 40:
          return "Luxemburg";

        case 50:
          return "Engeland";
      }

      return "";
    }

    /// <summary>
    /// English description for country of sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string CountryDescriptionEN(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "England";

        case 20:
          return "Scotland";

        case 30:
          return "Wales";

        case 40:
          return "Ireland";

        case 50:
          return "France";

        case 60:
          return "Netherlands";

        case 70:
          return "Belgium";

      }

      return "";
    }

    /// <summary>
    /// German description for country of sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string CountryDescriptionDE(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "Deutschland";

        case 20:
          return "Niederlände";

        case 30:
          return "Belgiën";

        case 40:
          return "Luxemburg";

        case 50:
          return "England";
      }

      return "";
    }

    /// <summary>
    /// Description for country of sequence in language of subscribe
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string CountryDescription(int displaySequence)
    {
      switch (_subscribe)
      {
        case "NL":
          return CountryDescriptionNL(displaySequence);
        case "EN":
          return CountryDescriptionEN(displaySequence);
        case "DE":
          return CountryDescriptionDE(displaySequence);
      }
      return "";
    }

    /// <summary>
    /// Create a country
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static Country CreateCountry(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.CountryId);

      var country = new Country();
      em.AddEntity(country);
      country.CountryId = sequence;

      return country;
    }

    /// <summary>
    /// Create a country for sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Country CreateCountry(AccoBookingEntities em, int displaySequence)
    {
      var country = CreateCountry(em);
      country.Description = CountryDescription(displaySequence);
      country.DisplaySequence = displaySequence;

      return country;
    }

    /// <summary>
    /// Find a country for a Dutch sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Country FindCountryNL(AccoBookingEntities em, int displaySequence)
    {
      switch (displaySequence)
      {
        case 10:
          return em.Countries.FirstOrDefault(c => c.Description == "Netherlands");
        case 20:
          return em.Countries.FirstOrDefault(c => c.Description == "Germany");
        case 30:
          return em.Countries.FirstOrDefault(c => c.Description == "Belgium");
        case 40:
          return em.Countries.FirstOrDefault(c => c.Description == "Luxembourg");
        case 50:
          return em.Countries.FirstOrDefault(c => c.Description == "England");
      }
      return null;
    }

    /// <summary>
    /// Find a country for an English sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Country FindCountryEN(AccoBookingEntities em, int displaySequence)
    {
      switch (displaySequence)
      {
        case 10:
          return em.Countries.FirstOrDefault(c => c.Description == "England");
        case 20:
          return em.Countries.FirstOrDefault(c => c.Description == "Scotland");
        case 30:
          return em.Countries.FirstOrDefault(c => c.Description == "Wales");
        case 40:
          return em.Countries.FirstOrDefault(c => c.Description == "Ireland");
        case 50:
          return em.Countries.FirstOrDefault(c => c.Description == "France");
        case 60:
          return em.Countries.FirstOrDefault(c => c.Description == "Netherlands");
        case 70:
          return em.Countries.FirstOrDefault(c => c.Description == "Belgium");
      }
      return null;
    }

    /// <summary>
    /// Find a country for a German sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Country FindCountryDE(AccoBookingEntities em, int displaySequence)
    {
      switch (displaySequence)
      {
        case 10:
          return em.Countries.FirstOrDefault(c => c.Description == "Germany");
        case 20:
          return em.Countries.FirstOrDefault(c => c.Description == "Netherland");
        case 30:
          return em.Countries.FirstOrDefault(c => c.Description == "Belgium");
        case 40:
          return em.Countries.FirstOrDefault(c => c.Description == "Luxembourg");
        case 50:
          return em.Countries.FirstOrDefault(c => c.Description == "England");
      }
      return null;
    }

    /// <summary>
    /// Find a country for the sequence in the language of the subscribe
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static Country FindCountry(AccoBookingEntities em, int displaySequence)
    {
      switch (_subscribe)
      {
        case "NL":
          return FindCountryNL(em, displaySequence);
        case "EN":
          return FindCountryEN(em, displaySequence);
        case "DE":
          return FindCountryDE(em, displaySequence);
      }
      return null;
    }

    /// <summary>
    /// Create an acco description
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoDescription CreateAccoDescription(AccoBookingEntities  em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoDescriptionId); 
      var description = new AccoDescription();
      em.AddEntity(description);
      description.AccoDescriptionId = sequence;

      return description;
    }
    
    private static string AccoDescriptionNL(string name)
    {
      switch (name)
      {
        case "ArriveAfter":
          return "na 3 uur in de middag";
        case "DepartureBefore":
          return "voor 11 uur 's morgens";
        case "AccoType":
          return "vakantiehuis";
        default:
          return string.Format("Geef {0} omschrijving", name);
      }
    }

    /// <summary>
    /// Create an acco description for a Dutch sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static AccoDescription CreateAccoDescriptionNL(AccoBookingEntities  em, int displaySequence, string name)
    {
      var description = CreateAccoDescription(em);
      description.PropertyName = name;
      
      switch (displaySequence)
      {
        case 10:
          description.LanguageId = _languages[0].LanguageId;
          description.Description = AccoDescriptionNL(name);
          break;

        case 20:
          description.LanguageId = _languages[1].LanguageId;
          description.Description = AccoDescriptionEN(name);
          break;

        case 30:
          description.LanguageId = _languages[2].LanguageId;
          description.Description = AccoDescriptionDE(name);
          break;

      }
      return description;
    }

    private static string AccoDescriptionEN(string name)
    {
      switch (name)
      {
        case "ArriveAfter":
          return "after 3 o'clock in the afternoon";
        case "DepartureBefore":
          return "before 11 o'clock in the morning";
        case "AccoType":
          return "holiday cottage";
        default:
          return string.Format("Give {0} description", name);
      }
    }


    /// <summary>
    /// Create an acco description for an English sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static AccoDescription CreateAccoDescriptionEN(AccoBookingEntities em, int displaySequence, string name)
    {
      var description = CreateAccoDescription(em);
        
       
      description.PropertyName = name;

      switch (displaySequence)
      {
        case 10:
          description.LanguageId = _languages[0].LanguageId;
          description.Description = AccoDescriptionEN(name);
          break;

        case 20:
          description.LanguageId = _languages[1].LanguageId;
          description.Description = AccoDescriptionDE(name);
          break;

        case 30:
          description.LanguageId = _languages[2].LanguageId;
          description.Description = AccoDescriptionNL(name);
          break;

      }
      return description;
    }

    private static string AccoDescriptionDE(string name)
    {
      switch (name)
      {
        case "ArriveAfter":
          return "nach 3 Uhr am Nachmittag";
        case "DepartureBefore":
          return "vor 11 Uhr in der Fruh";
        case "AccoType":
          return "Ferien wohnung";
        default:
          return string.Format("Gib {0} beschreibung", name);
      }
    }
    
    /// <summary>
    /// Create an acco description for a German sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static AccoDescription CreateAccoDescriptionDE(AccoBookingEntities em, int displaySequence, string name)
    {
      var description = CreateAccoDescription(em);
      description.PropertyName = name;

      switch (displaySequence)
      {
        case 10:
          description.LanguageId = _languages[0].LanguageId;
          description.Description = AccoDescriptionDE(name);

          break;

        case 20:
          description.LanguageId = _languages[1].LanguageId;
          description.Description = AccoDescriptionEN(name);
          break;

        case 30:
          description.LanguageId = _languages[2].LanguageId;
          description.Description = AccoDescriptionNL(name);
          break;

      }
      return description;
    }

    /// <summary>
    /// Create an acco description for a sequence in the language of subscribe
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static AccoDescription CreateAccoDescription(AccoBookingEntities em, int displaySequence, string name)
    {
      switch (_subscribe)
      {
        case "NL":
          return CreateAccoDescriptionNL(em, displaySequence, name);
        case "EN":
          return CreateAccoDescriptionEN(em, displaySequence, name);
        case "DE":
          return CreateAccoDescriptionDE(em, displaySequence, name);
      }
      return null;      
    }

    /// <summary>
    /// Create reminder description for Dutch sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string ReminderDescriptionNL(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "Wens een prettige vakantie";

        case 20:
          return "Stuur sleutel";

        case 30:
          return "Is sleutel aangekomen";

        case 40:
          return "Leuke vakantie gehad?";

      }

      return "";
    }

    /// <summary>
    /// Create reminder description for English sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string ReminderDescriptionEN(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "Wish happy holidays";

        case 20:
          return "Send key";

        case 30:
          return "Has key arrived?";

        case 40:
          return "Had a nice holiday?";

      }

      return "";
    }

    /// <summary>
    /// Create reminder description for German sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string ReminderDescriptionDE(int displaySequence)
    {
      switch (displaySequence)
      {

        case 10:
          return "Schönen Urlaub wünschen";

        case 20:
          return "Schlüssel senden";

        case 30:
          return "Hat Schlüssel angekommmen";

        case 40:
          return "hatten einen schönen Urlaub";
      }

      return "";
    }

    /// <summary>
    /// Create reminder description for sequence in language for subscribe
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string ReminderDescription(int displaySequence)
    {
      switch (_subscribe)
      {
        case "NL":
          return ReminderDescriptionNL(displaySequence);
        case "EN":
          return ReminderDescriptionEN(displaySequence);
        case "DE":
          return ReminderDescriptionDE(displaySequence);
      }
      return "";
    }

    /// <summary>
    /// Create a reminder
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoReminder CreateReminder(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, "AccoReminderId");

      var reminder = new AccoReminder();
      em.AddEntity(reminder);
      reminder.AccoReminderId = sequence;

      return reminder;
    }

    /// <summary>
    /// Create a reminder for a sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static AccoReminder CreateReminder(AccoBookingEntities em, int displaySequence)
    {
      var reminder = CreateReminder(em);

      reminder.Description = ReminderDescription(displaySequence);
      reminder.DisplaySequence = displaySequence;
      switch (displaySequence)
      {
        case 10:
          reminder.Milestone = MileStone.Arrival;
          reminder.Offset = -7;
          break;
        case 20:
          reminder.Milestone = MileStone.LastPayment;
          reminder.Offset = 1;
          break;
        case 30:
          reminder.Milestone = MileStone.LastPayment;
          reminder.Offset = 8;
          break;
        case 40:
          reminder.Milestone = MileStone.Departure;
          reminder.Offset = 7;
          break;
      }

      return reminder;
    }

    /// <summary>
    ///  Addition description for a Dutch sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string AdditionDescriptionNL(int displaySequence)
    {

      switch (displaySequence)
      {

        case 10:
          return "Eindschoonmaak";

        case 20:
          return "Toeristenbelasting";

        case 30:
          return "Water";

        case 40:
          return "Linnengoed";

        case 50:
          return "Extra schoonmaakt i.v.m. huisdieren";
      }

      return "";
    }

    /// <summary>
    ///  Addition description for a German sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string AdditionDescriptionDE(int displaySequence)
    {

      switch (displaySequence)
      {

        case 10:
          return "Endreinigung";

        case 20:
          return "Kurtaxe";

        case 30:
          return "Wasser";

        case 40:
          return "Bettwasche";

        case 50:
          return "Extra reinigung für Haustiere";
      }

      return "";
    }

    /// <summary>
    ///  Addition description for a English sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string AdditionDescriptionEN(int displaySequence)
    {

      switch (displaySequence)
      {

        case 10:
          return "End cleaning";

        case 20:
          return "Tourist tax";

        case 30:
          return "Water";

        case 40:
          return "Linnengoed";

        case 50:
          return "Bed sheets";
      }

      return "";
    }

    /// <summary>
    ///  Addition description for a Dutch sequence in the language sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string AdditionDescriptionNL(int displaySequence, int index)
    {
      switch (index)
      {
        case 1:
          return AdditionDescriptionNL(displaySequence);
        case 2:
          return AdditionDescriptionEN(displaySequence);
        case 3:
          return AdditionDescriptionDE(displaySequence);
      }
      return "";
    }

    /// <summary>
    ///  Addition description for a Englisg sequence in the language sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string AdditionDescriptionEN(int displaySequence, int index)
    {
      switch (index)
      {
        case 1:
          return AdditionDescriptionEN(displaySequence);
        case 2:
          return AdditionDescriptionDE(displaySequence);
        case 3:
          return AdditionDescriptionNL(displaySequence);
      }
      return "";
    }

    /// <summary>
    ///  Addition description for a German sequence in the language sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string AdditionDescriptionDE(int displaySequence, int index)
    {
      switch (index)
      {
        case 1:
          return AdditionDescriptionDE(displaySequence);
        case 2:
          return AdditionDescriptionNL(displaySequence);
        case 3:
          return AdditionDescriptionEN(displaySequence);
      }
      return "";
    }

    /// <summary>
    ///  Addition description for a sequence in the language sequence of the subscribe
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string AdditionDescription(int displaySequence, int index)
    {
      switch (_subscribe)
      {
        case "NL":
          return AdditionDescriptionNL(displaySequence, index);
        case "EN":
          return AdditionDescriptionEN(displaySequence, index);
        case "DE":
          return AdditionDescriptionDE(displaySequence, index);
      }
      return "";
    }

    /// <summary>
    /// Create an addition description
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoAdditionDescription CreateAdditionDescription(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoAdditionDescriptionId);

      var description = new AccoAdditionDescription();

      em.AddEntity(description);
      description.AccoAdditionDescriptionId = sequence;

      return description;
    }

    /// <summary>
    /// Create an addition description for a sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static AccoAdditionDescription CreateAdditionDescription(AccoBookingEntities em, int displaySequence,
      int index)
    {
      var description = CreateAdditionDescription(em);

      description.Language = _languages[index - 1];
      description.Description = AdditionDescription(displaySequence, index);

      return description;
    }

    /// <summary>
    /// Creante an addition
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoAddition CreateAddition(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoAdditionId);

      var addition = new AccoAddition();
      em.AddEntity(addition);
      addition.AccoAdditionId = sequence;

      return addition;
    }

    /// <summary>
    /// Create an addition for a sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static AccoAddition CreateAddition(AccoBookingEntities em, int displaySequence)
    {
      var description1 = CreateAdditionDescription(em, displaySequence, 1);
      var description2 = CreateAdditionDescription(em, displaySequence, 2);
      var description3 = CreateAdditionDescription(em, displaySequence, 3);

      var addition = CreateAddition(em);
      addition.DisplaySequence = displaySequence;

      switch (displaySequence)
      {
        case 10:
          addition.Price = 50;
          addition.Unit = UnitName.Booking;
          addition.IsDefaultBooked = true;
          addition.IsPaidFromDeposit = false;
          break;
        case 20:
          addition.Price = 1;
          addition.Unit = UnitName.PersonPerNight;
          addition.IsDefaultBooked = true;
          addition.IsPaidFromDeposit = false;
          break;
        case 30:
          addition.Price = 35;
          addition.Unit = UnitName.WaterM3;
          addition.IsDefaultBooked = true;
          addition.IsPaidFromDeposit = true;
          break;
        case 40:
          addition.Price = 10;
          addition.Unit = UnitName.Night;
          addition.IsDefaultBooked = false;
          addition.IsPaidFromDeposit = false;
          break;
        case 50:
          addition.Price = 5;
          addition.Unit = UnitName.Pet;
          addition.IsDefaultBooked = false;
          addition.IsPaidFromDeposit = false;
          break;
      }

      addition.AccoAdditionDescriptions.Add(description1);
      addition.AccoAdditionDescriptions.Add(description2);
      addition.AccoAdditionDescriptions.Add(description3);

      return addition;
    }

    /// <summary>
    /// Create a paypattern payment
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoPayPatternPayment CreatePayPatternPayment(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoPayPatternPaymentId);

      var payment = new AccoPayPatternPayment();
      em.AddEntity(payment);
      payment.AccoPayPatternPaymentId = sequence;

      return payment;
    }

    /// <summary>
    /// Create a paypattern payment for an sequence and a pattern index
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static AccoPayPatternPayment CreatePayPatternPayment(AccoBookingEntities em, int displaySequence, int index)
    {
      var payment = CreatePayPatternPayment(em);

      switch (displaySequence)
      {
        case 10:
          switch (index)
          {
            case 1:
              payment.DaysToPayAfterBooking = 7;
              payment.DaysToPayBeforeArrival = 0;
              payment.PaymentAmount = 0;
              payment.PaymentPercentage = 50;
              payment.DisplaySequence = 10;
              break;
            case 2:
              payment.DaysToPayAfterBooking = 0;
              payment.DaysToPayBeforeArrival = 42;
              payment.PaymentAmount = 0;
              payment.PaymentPercentage = 50;
              payment.DisplaySequence = 20;
              break;
          }
          break;

        case 20:
          switch (index)
          {
            case 1:
              payment.DaysToPayAfterBooking = 7;
              payment.DaysToPayBeforeArrival = 0;
              payment.PaymentAmount = 100;
              payment.PaymentPercentage = 0;
              payment.DisplaySequence = 10;
              break;
            case 2:
              payment.DaysToPayAfterBooking = 0;
              payment.DaysToPayBeforeArrival = 84;
              payment.PaymentAmount = 0;
              payment.PaymentPercentage = 50;
              payment.DisplaySequence = 20;
              break;
            case 3:
              payment.DaysToPayAfterBooking = 0;
              payment.DaysToPayBeforeArrival = 42;
              payment.PaymentAmount = 0;
              payment.PaymentPercentage = 50;
              payment.DisplaySequence = 30;
              break;
          }
          break;

        case 30:
          payment.DaysToPayAfterBooking = 3;
          payment.DaysToPayBeforeArrival = 0;
          payment.PaymentAmount = 0;
          payment.PaymentPercentage = 100;
          payment.DisplaySequence = 10;
          break;

      }
      return payment;
    }

    /// <summary>
    /// Create a pattern description for a Dutch sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string PatternDescriptionNL(int displaySequence)
    {

      switch (displaySequence)
      {

        case 10:
          return "Normaal";

        case 20:
          return "Vroege boeking";

        case 30:
          return "Last minute boeking";

      }

      return "";
    }

    /// <summary>
    /// Create a pattern description for an English sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string PatternDescriptionEN(int displaySequence)
    {

      switch (displaySequence)
      {

        case 10:
          return "Normal";

        case 20:
          return "Early booking";

        case 30:
          return "Last minute booking";

      }

      return "";
    }

    /// <summary>
    /// Create a pattern description for a German sequence
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string PatternDescriptionDE(int displaySequence)
    {

      switch (displaySequence)
      {

        case 10:
          return "Normal";

        case 20:
          return "Früh Buchung";

        case 30:
          return "Last Minute buchung";

      }

      return "";
    }

    /// <summary>
    /// Create a pattern description for a sequence in the language for subscribe
    /// </summary>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static string PatternDescription(int displaySequence)
    {
      switch (_subscribe)
      {
        case "NL":
          return PatternDescriptionNL(displaySequence);
        case "EN":
          return PatternDescriptionEN(displaySequence);
        case "DE":
          return PatternDescriptionDE(displaySequence);
      }
      return "";
    }

    /// <summary>
    /// Create a paypattern
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoPayPattern CreatePayPattern(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoPayPatternId);

      var pattern = new AccoPayPattern();
      em.AddEntity(pattern);
      pattern.AccoPayPatternId = sequence;

      return pattern;
    }

    /// <summary>
    /// Create a paypattern for a sequence
    /// </summary>
    /// <param name="em"></param>
    /// <param name="displaySequence"></param>
    /// <returns></returns>
    private static AccoPayPattern CreatePayPattern(AccoBookingEntities em, int displaySequence)
    {
      var pattern = CreatePayPattern(em);

      pattern.DisplaySequence = displaySequence;
      pattern.Description = PatternDescription(displaySequence);

      AccoPayPatternPayment payment1;
      AccoPayPatternPayment payment2;
      AccoPayPatternPayment payment3;

      switch (displaySequence)
      {
        case 10:
          pattern.IsAdditionInLastPayment = true;
          pattern.IsDepositInLastPayment = true;
          pattern.DaysBeforeArrival = 200;
          payment1 = CreatePayPatternPayment(em, displaySequence, 1);
          payment2 = CreatePayPatternPayment(em, displaySequence, 2);
          pattern.AccoPayPatternPayments.Add(payment1);
          pattern.AccoPayPatternPayments.Add(payment2);
          break;
        case 20:
          pattern.IsAdditionInLastPayment = true;
          pattern.IsDepositInLastPayment = true;
          pattern.DaysBeforeArrival = 999;
          payment1 = CreatePayPatternPayment(em, displaySequence, 1);
          payment2 = CreatePayPatternPayment(em, displaySequence, 2);
          payment3 = CreatePayPatternPayment(em, displaySequence, 3);
          pattern.AccoPayPatternPayments.Add(payment1);
          pattern.AccoPayPatternPayments.Add(payment2);
          pattern.AccoPayPatternPayments.Add(payment3);
          break;
        case 30:
          pattern.IsAdditionInLastPayment = false;
          pattern.IsDepositInLastPayment = false;
          pattern.DaysBeforeArrival = 21;
          payment1 = CreatePayPatternPayment(em, displaySequence, 1);
          pattern.AccoPayPatternPayments.Add(payment1);
          break;
      }

      return pattern;
    }

    /// <summary>
    /// Create a cancelcondition
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoCancelCondition CreateCancelCondition(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoCancelConditionId);

      var condition = new AccoCancelCondition();
      em.AddEntity(condition);
      condition.AccoCancelConditionId = sequence;

      return condition;
    }

    /// <summary>
    /// Create a cancelcondition for an index
    /// </summary>
    /// <param name="em"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static AccoCancelCondition CreateCancelCondition(AccoBookingEntities em, int index)
    {
      var condition = CreateCancelCondition(em);

      switch (index)
      {
        case 1:
          condition.DaysBeforeArrival = 7;
          condition.RentPercentageToPay = 100;
          break;
        case 2:
          condition.DaysBeforeArrival = 14;
          condition.RentPercentageToPay = 75;
          break;
        case 3:
          condition.DaysBeforeArrival = 21;
          condition.RentPercentageToPay = 50;
          break;
        case 4:
          condition.DaysBeforeArrival = 28;
          condition.RentPercentageToPay = 35;
          break;
        case 5:
          condition.DaysBeforeArrival = 42;
          condition.RentPercentageToPay = 25;
          break;
        case 6:
          condition.DaysBeforeArrival = 70;
          condition.RentPercentageToPay = 0;
          break;

      }
      return condition;
    }

    /// <summary>
    /// Create a rent
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoRent CreateRent(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoRentId);

      var rent = new AccoRent();
      em.AddEntity(rent);
      rent.AccoRentId = sequence;

      return rent;
    }

    /// <summary>
    /// Create a season
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoSeason CreateSeason(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoSeasonId);

      var season = new AccoSeason();
      em.AddEntity(season);
      season.AccoSeasonId = sequence;

      return season;
    }

    /// <summary>
    /// Subscribe to accobooking with credentials for a selected language
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string Subscribe(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      try
      {
        _accoid = (int) args[0];
        _subscribe = args[1] as string;
        var em = (AccoBookingEntities) entitymanager;

        var acco = em.Accoes.FirstOrDefault(a => a.AccoId == _accoid);
        var owner = em.AccoOwners.FirstOrDefault(a => a.AccoOwnerId == acco.AccoOwnerId);

        var language1 = FindLanguage(em, 10);
        var language2 = FindLanguage(em, 20);
        var language3 = FindLanguage(em, 30);

        _languages[0] = language1;
        _languages[1] = language2;
        _languages[2] = language3;

        var country1 = FindCountry(em, 10);

        var accodescription1 = CreateAccoDescription(em, 10, "AccoType");
        var accodescription2 = CreateAccoDescription(em, 20, "AccoType");
        var accodescription3 = CreateAccoDescription(em, 30, "AccoType");

        var accodescription4 = CreateAccoDescription(em, 10, "ArriveAfter");
        var accodescription5 = CreateAccoDescription(em, 20, "ArriveAfter");
        var accodescription6 = CreateAccoDescription(em, 30, "ArriveAfter");

        var accodescription7 = CreateAccoDescription(em, 10, "DepartureBefore");
        var accodescription8 = CreateAccoDescription(em, 20, "DepartureBefore");
        var accodescription9 = CreateAccoDescription(em, 30, "DepartureBefore");
        
        var reminder1 = CreateReminder(em, 10);
        var reminder2 = CreateReminder(em, 20);
        var reminder3 = CreateReminder(em, 30);
        var reminder4 = CreateReminder(em, 40);

        var addition1 = CreateAddition(em, 10);
        var addition2 = CreateAddition(em, 20);
        var addition3 = CreateAddition(em, 30);
        var addition4 = CreateAddition(em, 40);
        var addition5 = CreateAddition(em, 50);

        var pattern1 = CreatePayPattern(em, 10);
        var pattern2 = CreatePayPattern(em, 20);
        var pattern3 = CreatePayPattern(em, 30);

        var condition1 = CreateCancelCondition(em, 1);
        var condition2 = CreateCancelCondition(em, 2);
        var condition3 = CreateCancelCondition(em, 3);
        var condition4 = CreateCancelCondition(em, 4);
        var condition5 = CreateCancelCondition(em, 5);
        var condition6 = CreateCancelCondition(em, 6);

        accodescription1.Acco = acco;
        accodescription2.Acco = acco;
        accodescription3.Acco = acco;

        accodescription4.Acco = acco;
        accodescription5.Acco = acco;
        accodescription6.Acco = acco;

        accodescription7.Acco = acco;
        accodescription8.Acco = acco;
        accodescription9.Acco = acco;

        reminder1.Acco = acco;
        reminder2.Acco = acco;
        reminder3.Acco = acco;
        reminder4.Acco = acco;
        
        addition1.Acco = acco;
        addition2.Acco = acco;
        addition3.Acco = acco;
        addition4.Acco = acco;
        addition5.Acco = acco;

        pattern1.Acco = acco;
        pattern2.Acco = acco;
        pattern3.Acco = acco;

        condition1.Acco = acco;
        condition2.Acco = acco;
        condition3.Acco = acco;
        condition4.Acco = acco;
        condition5.Acco = acco;
        condition6.Acco = acco;

        em.SaveChanges(); //Fingers crossed!

        //lukt niet in een keer
        acco.AccoPayPattern = acco.AccoPayPatterns[0];
        acco.Country = country1;
        owner.Language = language1;
        owner.Country = country1;
        owner.PublicCountryId = country1.CountryId;

        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Copy all data from one acco to another acco (predeleting all the data)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string CopyAcco(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      try
      {
        _fromaccoid = (int)args[0];
        _accoid = (int)args[1];
        var em = (AccoBookingEntities)entitymanager;

        var fromacco = em.Accoes.FirstOrDefault(a => a.AccoId == _fromaccoid);
        var acco = em.Accoes.FirstOrDefault(a => a.AccoId == _accoid);

        Clone(fromacco, acco);
        acco.Description = fromacco.Description + " - copy";
        acco.AccoOwnerId = fromacco.AccoOwnerId;
        acco.CountryId = fromacco.CountryId;

        foreach (var description in em.AccoDescriptions.Where(c => c.AccoId == _accoid))
          description.EntityAspect.Delete();

        var descriptions = em.AccoDescriptions.Where(c => c.AccoId == _fromaccoid);
        foreach (var description in descriptions)
        {
          var copy = CreateAccoDescription(em);
          Clone(description, copy);
          copy.AccoId = _accoid;
        }

        foreach (var reminder in em.AccoReminders.Where(c => c.AccoId == _accoid))
          reminder.EntityAspect.Delete();

        var reminders = em.AccoReminders.Where(c => c.AccoId == _fromaccoid);
        foreach (var reminder in reminders)
        {
          var copy = CreateReminder(em);
          Clone(reminder, copy);
          copy.AccoId = _accoid;
        }

        foreach (var addition in em.AccoAdditions.Where(c => c.AccoId == _accoid))
          addition.EntityAspect.Delete();

        foreach (var addition in em.AccoAdditions.Where(c => c.AccoId == _fromaccoid))
        {
          var copy = CreateAddition(em);
          Clone(addition, copy);
          copy.AccoId = _accoid;

          foreach (var description in addition.AccoAdditionDescriptions)
          {
            var copyDescription = CreateAdditionDescription(em);
            Clone(description, copyDescription);
            copyDescription.AccoAdditionId = copy.AccoAdditionId;
            copyDescription.LanguageId = description.LanguageId;
          }
        }

        foreach (var rent in em.AccoRents.Where(c => c.AccoId == _accoid))
          rent.EntityAspect.Delete();

        var rentTo = new Dictionary<int, int>();

        foreach (var rent in em.AccoRents.Where(c => c.AccoId == _fromaccoid && c.IsActive))
        {
          var copy = CreateRent(em);

          Clone(rent, copy);
          if (rent.AccoRentId == fromacco.BaseRentId)
            acco.BaseRentId = rent.AccoRentId;
          copy.AccoId = _accoid;

          rentTo.Add(rent.AccoRentId, copy.AccoRentId);
        }
        
        foreach (var season in em.AccoSeasons.Where(c => c.AccoId == _accoid))
          season.EntityAspect.Delete();

        foreach (var season in em.AccoSeasons.Where(c => c.AccoId == _fromaccoid && c.SeasonEnd >= DateTime.Now))
        {
          var copy = CreateSeason(em);
          Clone(season, copy);
          copy.AccoId = _accoid;
          copy.AccoRentId = rentTo[season.AccoRentId];
        }
        
        foreach (var pattern in em.AccoPayPatterns.Where(c => c.AccoId == _accoid))
          pattern.EntityAspect.Delete();

        foreach (var pattern in em.AccoPayPatterns.Where(c => c.AccoId == _fromaccoid))
        {
          var copy = CreatePayPattern(em);
          Clone(pattern, copy);
          if (pattern.AccoPayPatternId == fromacco.DefaultPayPatternId)
            acco.DefaultPayPatternId = pattern.AccoPayPatternId;

          copy.AccoId = _accoid;

          foreach (var payment in pattern.AccoPayPatternPayments)
          {
            var copyPayment = CreatePayPatternPayment(em);
            Clone(payment, copyPayment);
            copyPayment.AccoPayPatternId = copy.AccoPayPatternId;
          }
        }
        
        foreach (var condition in em.AccoCancelConditions.Where(c => c.AccoId == _accoid))
          condition.EntityAspect.Delete();

        foreach (var condition in em.AccoCancelConditions.Where(c => c.AccoId == _fromaccoid))
        {
          var copy = CreateCancelCondition(em);
          Clone(condition, copy);
          copy.AccoId = _accoid;
        }

        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Copy rent/season definition from one acco to another
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string CopyRent(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      try
      {
        _fromaccoid = (int) args[0];
        _fromyear = (int) args[1];
        _accoid = (int) args[2];
        _toyear = (int) args[3];
        var em = (AccoBookingEntities) entitymanager;

        foreach (var rent in em.AccoRents.Where(c => c.AccoId == _fromaccoid && c.RentYear == _fromyear))
        {
          var copy = CreateRent(em);
          Clone(rent, copy);
          copy.AccoId = _accoid;
          copy.RentYear = _toyear;

          //seizoenen copieren van deze huur
          foreach (var season in rent.AccoSeasons)
          {
            var copySeason = CreateSeason(em);
            Clone(season, copySeason);
            copySeason.AccoId = _accoid;
            copySeason.SeasonYear = _toyear;
            copySeason.AccoRentId = copy.AccoRentId;
            copySeason.SeasonStart = season.SeasonStart.AddYears(_toyear - _fromyear);
            copySeason.SeasonEnd = season.SeasonEnd.AddYears(_toyear - _fromyear);
          }
        }

        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Duplicate data from one acoo to another
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string DuplicateAcco(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      try
      {
        _fromaccoid = (int) args[0];
        _accoid = (int) args[1];
        _subscribe = args[1] as string;
        var em = (AccoBookingEntities) entitymanager;

        var fromacco = em.Accoes.FirstOrDefault(a => a.AccoId == _fromaccoid);
        var acco = em.Accoes.FirstOrDefault(a => a.AccoId == _accoid);
        var owner = em.AccoOwners.FirstOrDefault(a => a.AccoOwnerId == acco.AccoOwnerId);
        var accoes = em.Accoes.Where(a => a.AccoOwnerId == owner.AccoOwnerId);

        acco.DisplaySequence = accoes.Count()*10;
        acco.Location = fromacco.Location;
        acco.OwnWebsite = fromacco.OwnWebsite;
        acco.Currency = fromacco.Currency;
        acco.ArriveAfter = fromacco.ArriveAfter;
        acco.DepartureBefore = fromacco.DepartureBefore;
        acco.DaysToPayDepositBackAfterDeparture = fromacco.DaysToPayDepositBackAfterDeparture;
        acco.DaysToExpire = fromacco.DaysToExpire;
        acco.StartWeekdayCalendar = fromacco.StartWeekdayCalendar;
        acco.Deposit = fromacco.Deposit;
        acco.CancelAdministrationCosts = fromacco.CancelAdministrationCosts;
        acco.IsActive = true;
        acco.LicenceExpiration = DateTime.Now.AddDays(31);
        acco.ColorOwner = fromacco.ColorOwner;
        acco.ColorOnline = fromacco.ColorOnline;
        acco.ColorBlock = fromacco.ColorBlock;
        acco.SendWeeklyReminders = fromacco.SendWeeklyReminders;

        var descriptions = em.AccoDescriptions.Where(c => c.AccoId == _fromaccoid);

        foreach (var d in descriptions)
        {
          var description = CreateAccoDescription(em);
          Clone(d, description);
          description.LanguageId = d.LanguageId;
          description.Acco = acco;
        }

        var reminders = em.AccoReminders.Where(c => c.AccoId == _fromaccoid);

        foreach (var r in reminders)
        {
          var reminder = CreateReminder(em);
          Clone(r, reminder);
          reminder.Acco = acco;
        }

        var additions = em.AccoAdditions.Where(c => c.AccoId == _fromaccoid);

        foreach (var a in additions)
        {
          var addition = CreateAddition(em);
          Clone(a, addition);
          addition.Acco = acco;

          foreach (var d in a.AccoAdditionDescriptions)
          {
            var description = CreateAdditionDescription(em);
           
            Clone(d, description);
            description.LanguageId = d.LanguageId;
            description.AccoAddition = addition;
          }
        }

        var rentTo = new Dictionary<int, int>();

        var rents = em.AccoRents.Where(c => c.AccoId == _fromaccoid);

        foreach (var r in rents)
        {
          var rent = CreateRent(em);
          Clone(r, rent);
          rent.Acco = acco;
          if (r.AccoRentId == fromacco.BaseRentId)
            acco.BaseRentId = rent.AccoRentId;

          rentTo.Add(r.AccoRentId, rent.AccoRentId);  //vertaal tabel van rente naar rente
        }

        var seasons = em.AccoSeasons.Where(c => c.AccoId == _fromaccoid);

        foreach (var s in seasons)
        {
          var season = CreateSeason(em);
          Clone(s, season);
          season.Acco = acco;
          season.AccoRentId = rentTo[s.AccoRentId];
        }

        var paypatterns = em.AccoPayPatterns.Where(c => c.AccoId == _fromaccoid);

        foreach (var t in paypatterns)
        {
          var pattern = CreatePayPattern(em);
          Clone(t, pattern);
          pattern.Acco = acco;

          foreach (var p in t.AccoPayPatternPayments)
          {
            var payment = CreatePayPatternPayment(em);
            Clone(p, payment);
            payment.AccoPayPattern = pattern;
          }
        }

        var cancelconditions = em.AccoCancelConditions.Where(c => c.AccoId == _fromaccoid);

        foreach (var c in cancelconditions)
        {
          var condition = CreateCancelCondition(em);
          Clone(c, condition);
          condition.Acco = acco;
        }

        // zetten defaults
        acco.DefaultPayPatternId = acco.AccoPayPatterns[0].AccoPayPatternId;
        acco.CountryId = fromacco.CountryId;

        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Build list for available departures for an acco on a arrival date
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static IEnumerable<AvailableDepartureListItem> AvailableDepartures(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      var departures = new List<AvailableDepartureListItem>();
      try
      {
        _accoid = (int) args[0];
        var arrivalOn = (DateTime) args[1];

        if (arrivalOn < DateTime.Now.Date)
          return departures;      //empty

        var em = (AccoBookingEntities) entitymanager;

        var acco = em.Accoes.FirstOrDefault(a => a.AccoId == _accoid);
        var season = Season(em, _accoid, arrivalOn);
        var rent = season == null ? acco.AccoRent : season.AccoRent;

        decimal amount;
        var date = arrivalOn.Date;


        if (rent.IsAvailablePerWeek)
        {
          amount = 0;
          var from = rent.MinimalWeeks;
          if (from > 0) from--;
          if (rent.MinimalWeeks > 1)
            amount = (rent.MinimalWeeks - 1)*rent.RentPerWeek;

          if (date.DayOfWeek.ToString().ToUpper() == rent.WeekExchangeDay ||
              date.DayOfWeek.ToString().ToUpper() == rent.OptionalWeekExchangeDay)
          {
            //aaneensluitend
            for (int i = from; i < 3; i++)
            {
              var departureOn = date.AddDays((i + 1)*7);
              var ok = true;

              for (int j = 0; j < 7; j++)
              {
                departureOn = date.AddDays(i*7 + j);
                var departureSeason = Season(em, _accoid, departureOn);
                var departureRent = departureSeason == null ? acco.AccoRent : departureSeason.AccoRent;
                amount += departureRent.RentPerWeek/7;

                ok = ok && (date.DayOfWeek.ToString().ToUpper() == departureRent.WeekExchangeDay ||
                            date.DayOfWeek.ToString().ToUpper() == departureRent.OptionalWeekExchangeDay)
                        && departureRent.IsAvailablePerWeek;
                if (!ok) break;
              }

              if (!ok) break;

              if (departures.All(d => d.Departure != departureOn.AddDays(1)))
              {
                var departure = new AvailableDepartureListItem()
                {
                  Arrival = date,
                  Departure = departureOn.AddDays(1),
                  PeriodUnit = PeriodUnitName.Week,
                  Nights = (i + 1)*7,
                  Rent = Math.Round(amount, 2, MidpointRounding.AwayFromZero)
                };
                if (CheckDeparture(em, _accoid, departure))
                  departures.Add(departure);
              }
            }
          }
        }

        if (rent.IsAvailablePerWeekend)
        {
          if (date.DayOfWeek == DayOfWeek.Friday)
          {
            var departureOn = date.AddDays(3);
            var ok = true;
            amount = 0;

            //aaneensluitend
            for (int i = 0; i < 4; i++)
            {
              departureOn = date.AddDays(i);
              var departureSeason = Season(em, _accoid, departureOn);
              var departureRent = departureSeason == null ? acco.AccoRent : departureSeason.AccoRent;

              ok = ok && departureRent.IsAvailablePerWeekend;
              if (!ok) break;

              amount += departureRent.RentPerWeekend / 4;
            }

            if (ok)
            {
              if (departures.All(d => d.Departure != departureOn))
              {
                var departure = new AvailableDepartureListItem()
                {
                  Arrival = date,
                  Departure = departureOn,
                  Nights = 3,
                  PeriodUnit = PeriodUnitName.Weekend,
                  Rent = Math.Round(amount, 2, MidpointRounding.AwayFromZero)
                };
                if (CheckDeparture(em, _accoid, departure))
                  departures.Add(departure);
              }
            }
          }
        }

        if (date.DayOfWeek == DayOfWeek.Monday)
        {
          var departureOn = date.AddDays(4);
          var ok = true;
          amount = 0;

          //aaneensluitend
          for (int i = 0; i < 5; i++)
          {
            departureOn = date.AddDays(i);
            var departureSeason = Season(em, _accoid, departureOn);
            var departureRent = departureSeason == null ? acco.AccoRent : departureSeason.AccoRent;

            ok = ok && departureRent.IsAvailablePerWeekend;
            if (!ok) break;

            amount += departureRent.RentPerMidweek / 5;

          }

          if (ok)
          {
            if (departures.All(d => d.Departure != departureOn))
            {
              var departure = new AvailableDepartureListItem()
              {
                Arrival = date,
                Departure = departureOn,
                Nights = 4,
                PeriodUnit = PeriodUnitName.MidWeek,
                Rent = Math.Round(amount, 2, MidpointRounding.AwayFromZero)
              };
              if (CheckDeparture(em, _accoid, departure))
                departures.Add(departure);
            }
          }
        }
        
        
        if (rent.IsAvailablePerNight)
        {
          amount = 0;
          var from = rent.MinimalNights;
          if (from > 0) from--;
          if (rent.MinimalNights > 1)
            amount = (rent.MinimalNights - 1)*rent.RentPerNight;

          for (int i = from; i < 14; i++)
          {
            var departureOn = date.AddDays(i + 1);
            var departureSeason = Season(em, _accoid, departureOn);
            var departureRent = departureSeason == null ? acco.AccoRent : departureSeason.AccoRent;

            //aaneensluitend
            if (!departureRent.IsAvailablePerNight) break;
           
            amount += departureRent.RentPerNight; //TODO rekening houden met midwweek, weekend, week etc.

            if (departures.All(d => d.Departure != departureOn))
            {
              var departure = new AvailableDepartureListItem()
              {
                Arrival = date,
                Departure = departureOn,
                Nights = i + 1,
                PeriodUnit = PeriodUnitName.Night,
                Rent = Math.Round(amount, 2, MidpointRounding.AwayFromZero)
              };
              if (CheckDeparture(em, _accoid, departure))
                departures.Add(departure);
            }
          }
        }
        
      }
        
      catch
        (Exception)
      {

        throw;
      }


      return departures;
    }

    /// <summary>
    /// Check if departure is already in list
    /// A departures for 1 week makes the departure for 7 nights not relevant
    /// </summary>
    /// <param name="em"></param>
    /// <param name="accoid"></param>
    /// <param name="departure"></param>
    /// <returns></returns>
    private static bool CheckDeparture(AccoBookingEntities em, int accoid, AvailableDepartureListItem departure)
    {
      var booking = em.Bookings.FirstOrDefault(b => b.AccoId == accoid && 
                                                   ((b.Arrival >= departure.Arrival && b.Arrival < departure.Departure) ||
                                                    (b.Departure > departure.Arrival && b.Departure <= departure.Departure)) 
                                                    && b.Status != BookingStatus.Cancelled 
                                                    && b.Status != BookingStatus.Expired);
      return booking == null;
    }

    private static AccoSeason Season(AccoBookingEntities em, int accoid, DateTime arrival)
    {
      return em.AccoSeasons.FirstOrDefault(s => s.AccoId == accoid && s.SeasonStart <= arrival && s.SeasonEnd >= arrival);
    }
  }
}


