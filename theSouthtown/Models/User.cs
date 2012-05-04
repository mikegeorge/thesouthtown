using System;
using System.ComponentModel.DataAnnotations;

namespace theSouthtown.Models {
  public class User {
    [Key]
    public int Id { get; set; }

    [Required]
    [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }

    public string Name { get; set; }

    public string Comment { get; set; }

    public bool PasswordNeedsUpdating { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateLastLogin { get; set; }

    public DateTime? DateLastActivity { get; set; }

    public DateTime DateLastPasswordChange { get; set; }
  }
}