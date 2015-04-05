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

using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MailTagListViewModel : BaseListViewModel<MailTag>
  {
    private string _context;

    [ImportingConstructor]
    public MailTagListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
    }

    public new void Start(string context)
    {
      _context = context;
      Start(0);
    }

    public override BaseListViewModel<MailTag> Start(int accoid)
    {
      //var orderBySelector = new SortSelector("Name");

      //_unitOfWork.PeriodUnits.FindInDataSourceAsync(c => c.AccoId == accoid,
      //                                q => q.OrderBy(c => c.Description),
      //                                "",
      //                                result => Items = new BindableCollection<PeriodUnit>(result),
      //                                _errorHandler.HandleError);
      int i = 0;
      Items = new BindableCollection<MailTag>();

      foreach (var code in SystemCodeService.SystemCodeList.Where(c => c.SystemGroup.Name == "ACCO"))
        Items.Add(new MailTag()
        {
          Id = ++i,
          Context = "Acco",
          ShortName = code.Description.ToLower(),
          Description = code.Description + "(" + code.SystemGroup.Name + ")"
        });

      if (_context == "PERIOD")
      {
        foreach (var code in SystemCodeService.SystemCodeList.Where(c => c.SystemGroup.Name == "PERIOD"))
          Items.Add(new MailTag()
          {
            Id = ++i,
            Context = "Period",
            ShortName = code.Description.ToLower(),
            Description = code.Description + "(" + code.SystemGroup.Name + ")"
          });
      }
      if (_context == "BOOKING")
        foreach (var code in SystemCodeService.SystemCodeList.Where(c => c.SystemGroup.Name == "BOOKING"))
          Items.Add(new MailTag()
          {
            Id = ++i,
            Context = "Booking",
            ShortName = code.Description.ToLower(),
            Description = code.Description + "(" + code.SystemGroup.Name + ")"
          });

      return this;
    }

  }

  public class MailTag : Entity
  {
    [Key]
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Context { get; set; }

    [DataMember]
    public string ShortName { get; set; }

    [DataMember]
    public string Description { get; set; }

  }

}
