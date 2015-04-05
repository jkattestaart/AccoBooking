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
  public class BookingReminderListItem : ListItemBase
  {
    [DataMember]
    public string Accommodation { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Milestone { get; set; }

    [DataMember]
    public int Offset { get; set; }

    [DataMember]
    public int DisplaySequence { get; set; }

    [DataMember]
    public bool IsDue { get; set; }

    [DataMember]
    public DateTime? Due { get; set; }

    [DataMember]
    public bool IsDone { get; set; }

    [DataMember]
    public DateTime? Done { get; set; }
  }
}