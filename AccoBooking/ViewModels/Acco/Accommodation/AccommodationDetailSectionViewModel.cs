using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccommodationDetailSectionViewModel : BaseSectionViewModel<DomainModel.Acco, AccoSummaryViewModel>
  {
    [ImportingConstructor]
    public AccommodationDetailSectionViewModel(AccoSummaryViewModel accommodationSummary)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_ACCOMMODATION;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = accommodationSummary;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 10; }
    }
    #endregion

    //public bool IsVisible { get; set; }

    //void IBaseDetailSection<DomainModel.Acco>.Start(int entityid)
    //{
    //  Start(entityid);
    //}

    //void IBaseDetailSection<DomainModel.Acco>.Start(DomainModel.Acco entity)
    //{
    //  Start(entity);
    //}


    public override BaseSectionViewModel<DomainModel.Acco, AccoSummaryViewModel> Start(int accoId)
    {
      ActivateItem(Section.Start(accoId));
      return this;
    }

    public override BaseSectionViewModel<DomainModel.Acco, AccoSummaryViewModel> Start(DomainModel.Acco acco)
    {
      ActivateItem(Section.Start(acco));
      return this;
    }


    
  }
}