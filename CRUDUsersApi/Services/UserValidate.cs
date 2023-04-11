using System;
using System.Text.RegularExpressions;

namespace CRUDUsersApi
{
	public class UserValidate
	{
		public bool PasswordLoginValidate(string login)
		{
			string loginRegex = @"^[a-zA-Z0-9]+$";

			if (Regex.IsMatch(login, loginRegex))
				return true;
			else
				return false;
		}

		public bool NameValidate(string name)
		{
            string nameRegex = @"^[a-zA-Zа-яА-ЯёЁ]+$";

            if (Regex.IsMatch(name, nameRegex))
                return true;
            else
                return false;
        }

		public bool BirthdayValidate(DateTime? dateTime)
		{
			if (dateTime == null)
				return true;

			if (dateTime <= DateTime.Now)
				return true;
			else
				return false;
		}

        public bool GenderValidate(int gender)
        {
            if (gender == 0 || gender == 1 || gender == 2)
                return true;
            else
                return false;
        }
    }
}

