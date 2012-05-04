using System;

public static class DateTimeExtentions {
  public static string ToDisplayAge(this DateTime dob) {
    DateTime today = DateTime.Now;

    //string fmt = "{0:0##}{1}";
    const string fmt = "{0} {1}";

    if (dob < today.AddYears(-2)) return string.Format(fmt, today.Year - dob.Year, "Years");
    if (dob < today.AddMonths(-6)) return string.Format(fmt, (today.Year - dob.Year)*12 + (today.Month - dob.Month), "Months");
    if (dob < today.AddDays(-6*7)) return string.Format(fmt, (today - dob).Days/7, "Weeks");
    return string.Format(fmt, (today - dob).Days, "Days");
  }
}

