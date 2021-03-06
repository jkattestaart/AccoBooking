﻿using System.ComponentModel.Composition;
using AccoBooking.ViewModels.Acco;
using Caliburn.Micro;
using Cocktail;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class ProposeViewModel : BaseScreen<DomainModel.Booking>
  {
    [ImportingConstructor]
    public ProposeViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                            SendMailWizardViewModel sendMailWizard,
                            IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      SendMailWizard = sendMailWizard;
    }

    public SendMailWizardViewModel SendMailWizard { get; set; }


    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    public BaseScreen<DomainModel.Booking> Send()
    {
      ((IActivate)SendMailWizard).Activate();
      SendMailWizard.Parent = this;
      SendMailWizard.Send();
      return this;
    }

    public override BaseScreen<DomainModel.Booking> Start(int entityid)
    {
      _entityid = entityid;
      ((IActivate)SendMailWizard).Activate();
      SendMailWizard.Parent = this;
      SendMailWizard.Start();
      return this;
    }

    public void Cancel()
    {
      if (Parent.GetType() == typeof(DepartureManagementViewModel))
        (Parent as DepartureManagementViewModel).Cancel();
      TryClose();
    }

  }

 }
