using Mvc.Mailer;
using System.Net.Mail;

namespace theSouthtown.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer     
	{
		public UserMailer():
			base()
		{
			MasterName="_Layout";
		}

    public virtual MailMessage Welcome(string name, string email, string password, string siteroot)
		{
      var mailMessage = new MailMessage { Subject = "Welcome to theSouthtown.com" };
      ViewBag.Name = name;
      ViewBag.Password = password;
      ViewBag.SiteRoot = siteroot;
      mailMessage.To.Add(email);
      PopulateBody(mailMessage, viewName: "Welcome");

			return mailMessage;
		}


    public virtual MailMessage PasswordReset(string name, string email, string password, string siteroot)
		{
      var mailMessage = new MailMessage { Subject = "Credentials to theSouthtown.com" };
      ViewBag.Name = name;
      ViewBag.Password = password;
      ViewBag.SiteRoot = siteroot;
      mailMessage.To.Add(email);
      PopulateBody(mailMessage, viewName: "PasswordReset");

			return mailMessage;
		}

		
	}
}