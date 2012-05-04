using System;
using System.Web;
using System.Web.Security;

namespace theSouthtown.Controllers {
  public interface IFormsAuthentication {
    void SignIn(string username, bool createPersistentCookie);
    void SignOut();
  }

  public class FormsAuthenticationService : IFormsAuthentication {
    public void SignIn(string username, bool createPersistentCookie) {
      //      FormsAuthentication.SetAuthCookie(username, createPersistentCookie);
      var authTicket = new FormsAuthenticationTicket(
        1,
        username, //user id
        DateTime.Now,
        DateTime.Now.AddMinutes(259200), // expiry
        createPersistentCookie, //do not remember
        "",
        "/");
      var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
      HttpContext.Current.Response.Cookies.Add(cookie);
    }
    public void SignOut() {
      FormsAuthentication.SignOut();
    }
  }
}