using System;
using System.Globalization;
using System.Web.Mvc;
using BabyOfTheMonth.Helpers;
using Mvc.Mailer;
using theSouthtown.Controllers;
using theSouthtown.Mailers;
using theSouthtown.Repositories;

namespace theSouthtown.Areas.Admin.Controllers
{
    public class AccountController : Controller {
      private const int MinPasswordLength = 8;

      private IFormsAuthentication _formsAuth;
      private IUserMailer _userMailer;
      private IUserRepository _userRepository;

      public AccountController(IUserRepository userRepository, IFormsAuthentication formsAuth, IUserMailer userMailer) {
        _userRepository = userRepository;
        _formsAuth = formsAuth;
        _userMailer = userMailer;
      }


      [Authorize]
        public ActionResult Index()
        {
            return View();
        }

      public ActionResult Login() {
        return View();
      }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, bool rememberMe, string returnUrl) {
          if (LoginIsValid(email, password)) {
            var user = _userRepository.GetByEmail(email);

            _formsAuth.SignIn(user.Email, rememberMe);

            if (!String.IsNullOrEmpty(returnUrl)) {
              return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Entries");
          }

          // Add Errors
          return View();
        }

        public ActionResult LogOut() {
          _formsAuth.SignOut();

          return RedirectToAction("Login", "Account");
        }


        [Authorize]
        public ActionResult ChangePassword() {
          return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword) {
          if (ValidateChangePassword(currentPassword, newPassword, confirmPassword)) {
            if (_userRepository.ChangePassword(User.Identity.Name, currentPassword, newPassword)) {
              TempData["successMessage"] = "Your Password has been updated.";
              return RedirectToAction("Index", "Entries");
            }
            ModelState.AddModelError("currentPassword", "Your password is incorrect.");
          }
          return View();
        }

        public ActionResult ForgotPassword() {
          return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email) {
          if (ForgotPasswordIsValid(email)) {
            var viewModel = _userRepository.ResetPassword(email);
            if (viewModel != null) {
              var siteroot = string.Format("{0}{1}", Request.DomainApplicationPath().TrimEnd('/'),
                                           Url.Action("ChangePassword"));

              _userMailer.PasswordReset(viewModel.User.Name, viewModel.User.Email, viewModel.GeneratedPassword,
                                              siteroot).Send();

              ViewBag.PageMessage = "You've got mail ... and a new temporary password";

              return RedirectToAction("Login");
            }
          }
          ViewBag.PageMessage =
            "We can't find this address, you might want to consider <a href=\"/Account/Register/\">Registering an Account.</a>";
          return View();
        }


        /// Validation Methods
        /// 
        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword) {
          if (String.IsNullOrEmpty(currentPassword)) {
            ModelState.AddModelError("currentPassword", "You must specify a current password.");
          }
          if (newPassword == null || newPassword.Length < MinPasswordLength) {
            ModelState.AddModelError("newPassword",
                                     String.Format(CultureInfo.CurrentCulture,
                                                   "You must specify a new password of {0} or more characters.",
                                                   MinPasswordLength));
          }

          if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal)) {
            ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
          }

          return ModelState.IsValid;
        }

        private bool LoginIsValid(string username, string password) {
          if (String.IsNullOrEmpty(username)) {
            ModelState.AddModelError("email", "Enter your email");
          }
          if (String.IsNullOrEmpty(password)) {
            ModelState.AddModelError("password", "Password please.");
          }
          //if (!_userService.CheckEmailExists(email)) not using email
          //{
          //  ModelState.AddModelError("email", "Email is not registered");
          //}
          if (!_userRepository.AuthenticateUser(username, password)) {
            ModelState.AddModelError("password", "Wrong password");
          }

          return ModelState.IsValid;
        }

        private bool ForgotPasswordIsValid(string email) {
          if (String.IsNullOrEmpty(email)) {
            ModelState.AddModelError("email", "Enter your email");
          }
          if (!_userRepository.CheckEmailExists(email)) {
            ModelState.AddModelError("email", "Email is not registered");
          }

          return ModelState.IsValid;
        }

    }
}
