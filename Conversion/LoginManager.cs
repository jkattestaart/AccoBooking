using System;
using System.Security.Principal;
using IdeaBlade.EntityModel;

namespace Conversion
{

  /// Server-side login manager using Windows Authentication. 
  ///  - Remember to place Server.dll in exe directory of client if 2-tier or on server in n-tier.
  ///  
  ///  To use Windows authentication with DevForce: 
  ///    We pass credentials along the WCF channel from the client to the EntityService, 
  ///    which requires defining a system.ServiceModel section in the configuration files
  ///    on both client and server and setting binding information.  Notice the binding
  ///    called "wsBindingCustom" in both the client and server config file samples - this
  ///    binding passes Windows credentials to the service.  Other binding configurations
  ///    are also possible.
  ///    
  ///  The client application logs in with "null credentials", and here in the
  ///  LoginManager we'll grab the Thread.CurrentPrincipal and use that as the authenticated
  ///  user.  We need to return an IPrincipal here; DevForce uses whatever is returned here
  ///  as the "logged in" user on the client, and also for all subsequent requests from the client
  ///  to the BOS.
  public class LoginManager : IEntityLoginManager
  {

    public LoginManager() { }

    public IPrincipal Login(ILoginCredential credential, EntityManager entityManager)
    {

      if (credential == null)
      {

        // if running n-tier the user's Windows credentials will be passed and available on the thread:
        var principal = System.Threading.Thread.CurrentPrincipal;

        // Note that we can't return the WindowsPrincipal/WindowsIdentity back to the client, so
        // create a new one, here we use the DevForce UserBase class, but you can create your own too. 
        if (principal.Identity.IsAuthenticated)
          return CreateUser(principal);

        // This is here for 2-tier testing, since you'll probably want to throw an error or do
        // some custom logic in n-tier.  When not running n-tier, the thread principal has not been set 
        // but you can obtain the Windows Identity and use that.
        var wi = WindowsIdentity.GetCurrent();
        var wp = new WindowsPrincipal(wi);
        return CreateUser(wp);
      }

      throw new NotImplementedException("Only Windows authentication is supported here");
    }

    public void Logout(IPrincipal principal, EntityManager entityManager)
    {
    }

    private IPrincipal CreateUser(IPrincipal currentUser)
    {
      // The UserBase and UserIdentity classes are custom IPrincipal/IIdentity implementations
      // within DevForce.  

      var identity = new UserIdentity(currentUser.Identity.Name, currentUser.Identity.AuthenticationType, currentUser.Identity.IsAuthenticated);

      // You can determine what roles to set, if any.
      var roles = new string[] { };
      var user = new UserBase(identity, roles);
      return user;
    }

  }
}
