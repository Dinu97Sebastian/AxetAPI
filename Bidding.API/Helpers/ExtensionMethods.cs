using Bidding.API.Models;
using System.Collections.Generic;

namespace Bidding.API.Helpers
{
    public static class ExtensionMethods
    {
        public static List<User> WithoutPasswords(this List<User> users)
        {
            users.ForEach(x => x.WithoutPassword());
            return users;
        }

        public static User WithoutPassword(this User user)
        {
            //user.Password = null;
            user.PasswordHash = null;
            user.PasswordSalt = null;
            return user;
        }

    }
}
