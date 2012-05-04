using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DevOne.Security.Cryptography.BCrypt;
using theSouthtown.Models;

namespace theSouthtown.Repositories
{ 
    public class UserRepository : IUserRepository
    {
      theSouthtownContext context = new theSouthtownContext();

        public IQueryable<User> All
        {
            get { return context.Users; }
        }

        public IQueryable<User> AllIncluding(params Expression<Func<User, object>>[] includeProperties)
        {
            IQueryable<User> query = context.Users;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public User Find(int id)
        {
            return context.Users.Find(id);
        }

        public void InsertOrUpdate(User user)
        {
            if (user.Id == default(int)) {
                // New entity
                context.Users.Add(user);
            } else {
                // Existing entity
                context.Entry(user).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var user = context.Users.Find(id);
            context.Users.Remove(user);
        }

        public void Save()
        {
            context.SaveChanges();
        }

      public User GetByEmail(string email) {
        return context.Users.SingleOrDefault(x => x.Email == email);
      }

      public bool ChangePassword(string name, string currentPassword, string newPassword) {
        var user = GetByEmail(name);
        if (user != null) {
          if (BCryptHelper.CheckPassword(currentPassword, user.PasswordHash)) {

            var passwordSalt = BCryptHelper.GenerateSalt(12);
            var passwordHash = BCryptHelper.HashPassword(newPassword, passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.DateLastPasswordChange = DateTime.Now;

            context.SaveChanges();
            return true;
          }
        }
        return false;
      }

      public RegisterUserViewModel ResetPassword(string email) {
        var user = GetByEmail(email);
        if (user != null) {
          string password = GeneratePassword(8);
          SetNewPassword(user, password);

          context.SaveChanges();

          return new RegisterUserViewModel { User = user, GeneratedPassword = password };
        }

        return null;

      }

      public bool AuthenticateUser(string username, string password) {
        var user = GetByEmail(username);

        if (user != null) {
          if (BCryptHelper.CheckPassword(password, user.PasswordHash)) {
            user.DateLastLogin = DateTime.Now;
            context.SaveChanges();
            return true;
          }

        }
        return false;
      }

      public bool CheckEmailExists(string email) {
        return GetByEmail(email) != null;
      }

      public RegisterUserViewModel RegisterUser(User user) {
        var viewModel = new RegisterUserViewModel();

        viewModel.User = user;

        // check if email exists
        if (GetByEmail(user.Email) != null) {
          viewModel.HasError = true;
          viewModel.Message = "Email address is already in use";

          return viewModel;
        }

        viewModel.GeneratedPassword = GeneratePassword(8);

        viewModel.User = SetNewPassword(viewModel.User, viewModel.GeneratedPassword);

        viewModel.User.PasswordNeedsUpdating = true;
        viewModel.User.DateCreated = DateTime.Now;
        viewModel.User.DateLastActivity = DateTime.Now;
        viewModel.User.DateLastLogin = DateTime.Now;
        viewModel.User.DateLastPasswordChange = DateTime.Now;

        context.Users.Add(viewModel.User);
        context.SaveChanges();

        return viewModel;
      }

      private User SetNewPassword(User user, string password) {
        var passwordSalt = BCryptHelper.GenerateSalt(12);
        var passwordHash = BCryptHelper.HashPassword(password, passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.DateLastPasswordChange = DateTime.Now;

        return user;
      }


      private string GeneratePassword(int passwordLength) {
        const string allowedChars = "abcdefghijkmnopqrstuvwxyz123456789";
        var randNum = new Random();
        var chars = new char[passwordLength];

        for (int i = 0; i < passwordLength; i++) {
          chars[i] = allowedChars[(int)((allowedChars.Length) * randNum.NextDouble())];
        }

        return new string(chars);
      }

    }

    public interface IUserRepository
    {
        IQueryable<User> All { get; }
        IQueryable<User> AllIncluding(params Expression<Func<User, object>>[] includeProperties);
        User Find(int id);
        void InsertOrUpdate(User user);
        void Delete(int id);
        void Save();
      User GetByEmail(string email);
      bool ChangePassword(string name, string currentPassword, string newPassword);
      RegisterUserViewModel ResetPassword(string email);
      bool AuthenticateUser(string username, string password);
      bool CheckEmailExists(string email);
      RegisterUserViewModel RegisterUser(User user);
    }
}