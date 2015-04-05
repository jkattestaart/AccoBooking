using System;
using System.Runtime.Serialization;

namespace DomainModel.Projections
{
  [DataContract]
  public class BookingToDoListItem: ListItemBase
  {
    [DataMember]
    public string Context { get; set; }

    [DataMember]
    public string Accommodation { get; set; }

    [DataMember]
    public string Description { get; set; }
   
    [DataMember]
    public string Guest { get; set; }

    public DateTime Due
    {
      get { return ExpirationDate.AddDays(DaysToExpire); }
    }

    [DataMember]
    public int DaysToExpire { get; set; }

    [DataMember]
    public DateTime ExpirationDate { get; set; }

  }
}

