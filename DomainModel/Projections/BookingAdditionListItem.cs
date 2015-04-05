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
  public class BookingAdditionListItem : ListItemBase
  {
    private decimal _amount;

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Unit { get; set; }

    [DataMember]
    public string SystemCodeUnit { get; set; }

    [DataMember]
    public decimal Price { get; set; }

    [DataMember]
    public int Adults { get; set; }
    
    [DataMember]
    public int Children { get; set; }
    
    [DataMember]
    public int Pets { get; set; }

    [DataMember]
    public DateTime Arrival { get; set; }

    [DataMember]
    public DateTime Departure { get; set; }

    [DataMember]
    public decimal KWHUsage { get; set; }
    [DataMember]
    public decimal GASM3Usage { get; set; }
    [DataMember]
    public decimal WATERM3Usage { get; set; }

    public int Nights
    {
      get { return Departure.Subtract(Arrival).Days; }
    }

    [DataMember]
    public decimal Amount
    {
      set { _amount = value; }
      get
      {
        switch (SystemCodeUnit)
        {
          case "BOOKING":
            _amount = Price;
            break;

          case "NIGHT":
            _amount = Nights*Price;
            break;

          case "PERSON":
            _amount = (Adults + Children)*Price;
            break;

          case "PERSON-PER-NIGHT":
            _amount = (Adults + Children)*Nights*Price;
            break;

          case "ADULT":
            _amount = Adults*Price;
            break;

          case "ADULT-PER-NIGHT":
            _amount = Adults*Nights*Price;
            break;

          case "CHILD":
            _amount = Children*Price;
            break;

          case "CHILD-PER-NIGHT":
            _amount = Children*Nights*Price;
            break;

          case "PET":
            _amount = Pets*Price;
            break;

          case "PET-PER-NIGHT":
            _amount = Pets*Nights*Price;
            break;

          case "KWH":
            _amount = KWHUsage * Price;
            break;
          
          case "GASM3":
            _amount = GASM3Usage * Price;
            break;
          
          case "WATERM3":
            _amount = WATERM3Usage * Price;
            break;
            
          default:
            _amount = 0;
            break;
        }
        return _amount;
      }
    }

    [DataMember]
    public bool IsPaidFromDeposit { get; set; }

    [DataMember]
    public int DisplaySequence { get; set; }
  }
}