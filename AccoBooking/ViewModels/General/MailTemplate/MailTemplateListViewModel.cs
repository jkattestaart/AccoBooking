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

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using DomainModel;
using DomainServices;
using DomainServices.Services;

namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MailTemplateListViewModel : BaseListViewModel<MailTemplate>
  {
    private int _accoid;
    private string _mailContext;
    private int _languageId;

    [ImportingConstructor]
    public MailTemplateListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
    }

    public int LanguageId
    {
      get { return _languageId; }
      set
      {
        _languageId = value;
       // ExecuteQuery();  TODO alleen templates tonen die in die taal beschikbaar zijn
      }
    }

    public string MailContext
    {
      get { return _mailContext; }
      set
      {
        _mailContext = value;
        LoadDataAsync(0);
      }
    }

    public override BaseListViewModel<MailTemplate> Start(int accoid)
    {
      //var orderBySelector = new SortSelector("Name");
      _accoid = accoid;

      LoadDataAsync(0);
      return this;
    }

    public async override void LoadDataAsync(int selection)
    {
      var items = await _unitOfWork.MailTemplates.FindInDataSourceAsync(
        c => (String.IsNullOrEmpty(MailContext) || c.MailContext == MailContext),
        q => q.OrderBy(c => c.DisplaySequence)
        );

      foreach (var template in items)
      {
        var tpl = SystemCodeService.SystemCodeList
          .FirstOrDefault(t => t.Code == template.TemplateType.ToUpper() && t.SystemGroup.Name == SystemGroupName.MailTemplate);
        if (tpl != null)
          template.Description = tpl.Description;
        else
          template.Description = "!!" + template.TemplateType;
      }

      Items = new BindableCollection<MailTemplate>(items);
    }
  }
}