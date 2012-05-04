using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabyOfTheMonth.Helpers {
  public static class StringExtensions {
    public static MvcHtmlString ToHtmlLines(this string text) {
      return text.IsNotEmptyOrWhiteSpace() ? MvcHtmlString.Create(text.Replace('\n'.ToString(), "<br />")) : MvcHtmlString.Empty;
    }
  }
}