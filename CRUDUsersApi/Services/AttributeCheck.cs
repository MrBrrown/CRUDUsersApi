using System;
using CRUDUsersApi.Models;

namespace CRUDUsersApi.Services
{
	public class AttributeCheck
	{
        public bool IsSenderAttributCorrect(User user, string password)
        {
            if (user != default)
            {
                if (user.Password != password)
                    return false;
            }
            else { return false; }

            return true;
        }

        public bool IsSenderAdminOrHimself(User user, string UserLogin)
        {
            if (user.Admin)
            {
                if (user.RevokedOn == DateTime.MaxValue)
                    return true;
                else
                    return false;
            }
            else
            {
                if (user.Login == UserLogin && user.RevokedOn == DateTime.MaxValue)
                    return true;
                else
                    return false;
            }
        }
    }
}

