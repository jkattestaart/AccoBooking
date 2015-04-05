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
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Resources;
using System.Windows.Controls;
using AccoBooking.ViewModels.Acco;
using AccoBooking.ViewModels.Booking;
using AccoBooking.ViewModels.General;
using C1.Silverlight;
using Caliburn.Micro;
using Common;
using Common.Workspace;
using AccoBooking.ViewModels;

namespace AccoBooking
{
  public class AppBootstrapper : BootstrapperBase<ShellViewModel>
  {
    public AppBootstrapper()
    {
      ConventionManager
              .AddElementConvention<C1NumericBox>(C1NumericBox.ValueProperty,
                                          "Value",
                                          "DataContextChanged");

      ConventionManager
              .AddElementConvention<Label>(Label.ContentProperty,
                                          "Content",
                                          "DataContextChanged");

      Common.Converters.LabelContentConverter.LabelResource = new ResourceManager("AccoBooking.Resources.AccoBooking", Assembly.GetExecutingAssembly());


    }



    protected override void PrepareCompositionContainer(CompositionBatch batch)
    {
      base.PrepareCompositionContainer(batch);


      // Configure workspaces
      
      // Public 
      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_PUBLIC, true, 0, typeof(PublicViewModel), "Public", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_HOME, false, 10, typeof(HomeViewModel), "Public", true, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_HELPDESK, false, 20, typeof(SupportViewModel), "Public", true, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_RATES, false, 30, typeof(TarievenViewModel), "Public", true, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_SUBSCRIBE, false, 40, typeof(AccoSubscribeViewModel), "Public", true, false));

      //Home
      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_HOME_RENTAL, false, 10, typeof(HomeRentalViewModel), "", true, false));   //niet in menu!

      // Members
      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_DISPLAY_AVAILABILITY, true, 10, typeof (AccoAvailablePeriodCalenderViewModel), "Main", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_CREATE_BOOKING, true, 20, typeof(NewBookingFromDepartureViewModel), "Main", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_SEARCH_BOOKING, true, 30, typeof(SearchBookingViewModel), "Main", false, false));
      
      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_SEARCH_BOOKING, true, 30, typeof(SearchBookingTrusteeViewModel), "Main", false, true));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_BLOCK_PERIOD, true, 35, typeof(BlockManagementViewModel), "Main", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_CHECK_REMINDER, true, 40, typeof(CheckRemindersManagementViewModel), "Main", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_UPDATE_SETTINGS, true, 70, typeof(AccoManagementViewModel), "Main", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_REMOVE_ACCO, true, 10, typeof(RemoveAccoSubscribeViewModel), "HIDDEN", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_ADD_ACCO, true, 10, typeof(ExtraAccoSubscribeViewModel), "HIDDEN", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_COPY_ACCO, true, 10, typeof(CopyAccommodationViewModel), "HIDDEN", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_COPY_RENT, true, 10, typeof(CopyAccoRentViewModel), "HIDDEN", false, false));

      //Admin functions
      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_SYSTEM_GROUP, true, 110, typeof(SystemGroupControlViewModel), "Admin", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_SYSTEM_CODE, true, 120, typeof(SystemCodeControlViewModel), "Admin", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_COUNTRY, true, 130, typeof(CountryManagementViewModel), "Admin", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_LANGUAGE, true, 140, typeof(LanguageManagementViewModel), "Admin", false, false));

      batch.AddExportedValue<IWorkspace>(
        new Workspace(Resources.AccoBooking.ws_HELPDESK, false, 170, typeof(HelpdeskViewModel), "Main", false, false));

    }

#if FAKESTORE || DEMO
        [Import] public IEntityManagerProvider<AccoBookingEntities> EntityManagerProvider;

        protected override Task StartRuntimeAsync()
        {
            return EntityManagerProvider.InitializeFakeBackingStoreAsync();
        }
#endif
  }
}