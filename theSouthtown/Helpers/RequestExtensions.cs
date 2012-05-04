using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabyOfTheMonth.Helpers {
  public static class RequestExtensions {
    public static string DomainApplicationPath(this HttpRequestBase requestBase) {
      string Port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
      if (Port == null || Port == "80" || Port == "443")
        Port = "";
      else
        Port = ":" + Port;

      string Protocol = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
      if (Protocol == null || Protocol == "0")
        Protocol = "http://";
      else
        Protocol = "https://";


      // *** Figure out the base Url which points at the application's root
      return Protocol + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + Port + HttpContext.Current.Request.ApplicationPath;

    }
  }
}