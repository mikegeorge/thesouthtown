using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace theSouthtown.Models {
  public class Article {
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public User Author { get; set; }
    public bool Published { get; set; }
    public string MediaId { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
  }
}