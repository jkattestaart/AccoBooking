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
  public class BookingGuestListItem : ListItemBase
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Email { get; set; }

    [DataMember]
    public string Phone { get; set; }

    [DataMember]
    public string Address1 { get; set; }

    [DataMember]
    public string Address2 { get; set; }

    [DataMember]
    public string Address3 { get; set; }

    [DataMember]
    public string Country { get; set; }

    [DataMember]
    public string Language { get; set; }

    [DataMember]
    public string Bank { get; set; }

    [DataMember]
    public string BankAccount { get; set; }

    [DataMember]
    public string Gender { get; set; }

    [DataMember]
    public DateTime? BirthDate { get; set; }

    [DataMember]
    public string IdentityDocument { get; set; }
  }
}