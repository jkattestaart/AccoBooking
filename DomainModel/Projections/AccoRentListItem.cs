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

using System.Runtime.Serialization;

namespace DomainModel.Projections
{
  [DataContract]
  public class AccoRentListItem : ListItemBase
  {
    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Period { get; set; }

    [DataMember]
    public int RentYear { get; set; }
    
    [DataMember]
    public decimal Rent { get; set; }

    [DataMember]
    public int MinimalStay { get; set; }

    [DataMember]
    public string WeekExchangeDay1 { get; set; }

    [DataMember]
    public string WeekExchangeDay2 { get; set; }

    [DataMember]
    public string Color { get; set; }

    
    [DataMember]
    public decimal RentPerNight { get; set; }
    
    [DataMember]
    public decimal RentPerWeekend { get; set; }
    
    [DataMember]
    public decimal RentPerMidweek { get; set; }

    [DataMember]
    public decimal RentPerWeek { get; set; }

    [DataMember]
    public bool IsActive { get; set; }

  }
}