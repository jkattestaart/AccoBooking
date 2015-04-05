using System;
using System.ComponentModel.Composition;
using System.Linq;
using AccoBooking.Authentication;
using Caliburn.Micro;
using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Security;

namespace UnitTestAccoBooking
{
  [TestClass]
  public class UnitTest1
  {
    [Import]
    private IAuthenticationService _authenticationService;
    private AccoBookingEntities _ctx = new AccoBookingEntities();
    private string testSequenceName = "AccoId";

    [TestInitialize]

    public void TestInitialize()
    {

      var provider = new TestCompositionProvider();
      provider.AddOrUpdateInstance<IEventAggregator>(new EventAggregator());
      Composition.SetProvider(provider);

      var cred = new LoginCredential("admin", "password", null);
      _authenticationService = new AccoBookingAuthenticationService();
      _authenticationService.Login(cred);
   
      _ctx.AuthenticationContext = _authenticationService.AuthenticationContext;
      
    }

    [TestMethod]
    public void TestMethod1()
    {
      var sequence = _ctx.Sequences.FirstOrDefault(s => s.Name == testSequenceName);

      Assert.IsNotNull(sequence);
    }

    [TestMethod]
    public void TestMethod2()
    {
      var sequence = _ctx.Sequences.FirstOrDefault(s => s.Name == testSequenceName);

      Assert.IsInstanceOfType(sequence, typeof(Sequence));
    }

    [TestMethod]
    public void TestMethod3()
    {
      var sequence = _ctx.Sequences.FirstOrDefault(s => s.Name == testSequenceName).CurrentId;
      var next = GeneralLibrary.NextValue(_ctx, testSequenceName);
      
      Assert.AreEqual(next, sequence + 1);
      _ctx.RejectChanges();
    }
  }
}
