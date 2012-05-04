using System.Web.Mvc;
using BabyOfTheMonth.Helpers;
using Mvc.Mailer;
using theSouthtown.Mailers;
using theSouthtown.Models;
using theSouthtown.Repositories;

namespace theSouthtown.Areas.Admin.Controllers {
  [Authorize]
  public class UsersController : Controller {
    private readonly IUserRepository _userRepository;
    private readonly IUserMailer _userMailer;


    public UsersController(IUserRepository userRepository, IUserMailer userMailer) {
      _userRepository = userRepository;
      _userMailer = userMailer;
    }

    //
    // GET: /Users/

    public ViewResult Index() {
      return View(_userRepository.All);
    }

    //
    // GET: /Users/Details/5

    public ViewResult Details(int id) {
      return View(_userRepository.Find(id));
    }

    //
    // GET: /Users/Create

    public ActionResult Register() {
      return View();
    }

    //
    // POST: /Users/Create

    [HttpPost]
    public ActionResult Register(User user) {
      var userDto = _userRepository.RegisterUser(user);

      if (ModelState.IsValid) {
        if (!userDto.HasError) {
          // SEND EMAIL TO USER
          var siteroot = string.Format("{0}{1}", Request.DomainApplicationPath().TrimEnd('/'),
                                       Url.Action("ChangePassword", "Account"));

          _userMailer.Welcome(userDto.User.Name, userDto.User.Email, userDto.GeneratedPassword, siteroot).Send();


          TempData["SuccessMessage"] = "The user was successfully registered.";
          return RedirectToAction("Edit", new {id = userDto.User.Id});

        }
        ViewData.ModelState.AddModelError("_FORM", userDto.Message);

      }
      userDto.User = user;

      //MvcValidationAdapter.TransferValidationMessagesTo(ViewData.ModelState, userDto.User.ValidationResults());

      return View(userDto.User);
    }

    //
    // GET: /Users/Edit/5

    public ActionResult Edit(int id) {
      return View(_userRepository.Find(id));
    }

    //
    // POST: /Users/Edit/5

    [HttpPost]
    public ActionResult Edit(User userFromForm) {
      var userToUpdate = _userRepository.Find(userFromForm.Id);

      userToUpdate.Email = userFromForm.Email;
      userToUpdate.Name = userFromForm.Name;
      userToUpdate.Comment = userFromForm.Comment;

      if (ModelState.IsValid) {
        _userRepository.InsertOrUpdate(userToUpdate);
        _userRepository.Save();
        return RedirectToAction("Index");
      }
      else {
        return View(userToUpdate);
      }
    }

    //
    // GET: /Users/Delete/5

    public ActionResult Delete(int id) {
      return View(_userRepository.Find(id));
    }

    //
    // POST: /Users/Delete/5

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id) {
      _userRepository.Delete(id);
      _userRepository.Save();

      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult ResetPassword(string email) {
      var viewModel = _userRepository.ResetPassword(email);


      // SEND EMAIL TO USER
      var siteroot = string.Format("{0}{1}", Request.DomainApplicationPath().TrimEnd('/'), Url.Action("ChangePassword", "Account"));

      _userMailer.Welcome(viewModel.User.Name, viewModel.User.Email, viewModel.GeneratedPassword, siteroot).Send();

      return Content(string.Format("Password Reset For {0}", viewModel.User.Name));
    }
  }
}

