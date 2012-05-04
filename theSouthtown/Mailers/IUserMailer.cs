using System.Net.Mail;

namespace theSouthtown.Mailers
{ 
    public interface IUserMailer
    {
      //MailMessage RegistrationMessage(string name, string email, string password, string siteroot);
      //MailMessage ForgotPasswordMessage(string name, string email, string password, string siteroot);

      MailMessage Welcome(string name, string email, string password, string siteroot);


      MailMessage PasswordReset(string name, string email, string password, string siteroot);
		
		
	}
}