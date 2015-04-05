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

using System.ComponentModel;
using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainServices;

namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MailTemplateSummaryViewModel : BaseScreen<MailTemplate>
  {
    [ImportingConstructor]
    public MailTemplateSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                        SystemCodeListViewModel mailContextList,
                                        // MailTemplateContentMasterViewModel mailContentManagement,
                                        IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
     //MailContentManagement = mailContentManagement;
     MailContextList = mailContextList;
     MailContextList.PropertyChanged += MailContextListOnPropertyChanged;

    }

    //public MailTemplateContentMasterViewModel MailContentManagement { get; set; }

    private void MailContextListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((MailTemplate)Entity).MailContext = MailContextList.ShortName;
        }
      }
    }

    public SystemCodeListViewModel MailContextList { get; set; }

    protected override IRepository<MailTemplate> Repository()
    {
      return UnitOfWork.MailTemplates;
    }

    public override IdeaBlade.EntityModel.Entity Entity
    {
      get
      {
        return base.Entity;
      }
      set
      {
        base.Entity = value;
        if (Entity != null)
        {
         MailContextList.ShortName = ((MailTemplate)Entity).MailContext;
         //NotifyOfPropertyChange(() => MailContentManagement);
        }
      }
    }
    public override BaseScreen<MailTemplate> Start(int entityid)
    {
      MailContextList.Start("MAILCONTEXT");
      //MailContentManagement.Start(entityid);
      return base.Start(entityid);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        MailContextList.PropertyChanged -= MailContextListOnPropertyChanged;
      }
    }


  }
}