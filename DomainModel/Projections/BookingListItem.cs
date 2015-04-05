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
using System.Runtime.Serialization;

namespace DomainModel.Projections
{
  [DataContract]
  public class BookingListItem : ListItemBase
  {
    [DataMember]
    public string Accommodation { get; set; }

    [DataMember]
    public DateTime Arrival { get; set; }

    [DataMember]
    public DateTime Departure { get; set; }

    [DataMember]
    public int Adults { get; set; }

    [DataMember]
    public int Children { get; set; }

    [DataMember]
    public int Pets { get; set; }

    public int Nights 
    {
      get { return Departure.Subtract(Arrival).Days; }
    }
 
    [DataMember]
    public string GuestName { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public decimal Rent { get; set; }

    [DataMember]
    public decimal Deposit { get; set; }

    [DataMember]
    public decimal Additions { get; set; }

    public decimal SubTotal
    {
      get
      {
        return Rent + Additions;   //Additions wordt berekend in onderhouden bookingaddition
      }
    }

    public decimal Total
    {
      get { return SubTotal + Deposit; } // de totale som
    }

  }
}