using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using theSouthtown;
using theSouthtown.Controllers;

namespace theSouthtown.Tests.Controllers {
  [TestFixture]
  public class HomeControllerTest {
    [Test]
    public void Index() {
      // Arrange
      HomeController controller = new HomeController();

      // Act
      ViewResult result = controller.Index() as ViewResult;

      // Assert
      ViewDataDictionary viewData = result.ViewData;
      Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
    }

    [Test]
    public void About() {
      // Arrange
      HomeController controller = new HomeController();

      // Act
      ViewResult result = controller.About() as ViewResult;

      // Assert
      Assert.IsNotNull(result);
    }

    [Test]
    public void Contact() {
      // Arrange
      HomeController controller = new HomeController();

      // Act
      ViewResult result = controller.Contact() as ViewResult;

      // Assert
      Assert.IsNotNull(result);
    }
  }
}
