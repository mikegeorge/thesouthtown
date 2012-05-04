namespace theSouthtown.Models {
  public class RegisterUserViewModel {
    public User User { get; set; }
    public string GeneratedPassword { get; set; }

    public bool HasError { get; set; }
    public string Message;
  }
}