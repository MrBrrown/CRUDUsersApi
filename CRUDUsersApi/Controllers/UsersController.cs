using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUDUsersApi.Date;
using CRUDUsersApi.Models;
using CRUDUsersApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace CRUDUsersApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApiContext _context;       // Контектс базы данных 
        private readonly UserValidate validator = new UserValidate();       // Валидация значений вводимых пользователем
        private readonly AttributeCheck attributeCheck = new AttributeCheck();

        public struct UserFields        // Структура для вывода конктретных знаечений у пользователя в рамках одного объекта, для дальнейшего взаимодействия
        {
            public string Name { get; set; }

            public int Gender { get; set; }

            public DateTime? Birthday { get; set; }

            public bool IsActive { get; set; }
        }

        public UsersController(ApiContext context)
        {
            _context = context;

            var Admin = _context.Users      // Если отсутвует администратор, то создает его
                .Where(x => x.Login == "Admin")
                .FirstOrDefault();

            if (Admin == default)
            {
                User NewUser = new User()
                {
                    Guid = Guid.NewGuid(),
                    Login = "Admin",
                    Password = "Admin",
                    Name = "Admin",
                    Gender = 0,
                    Birthday = new DateTime(999, 01, 01),
                    Admin = true,
                    CreatedOn = DateTime.Now,
                    CreatedBy = "",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "",
                    RevokedOn = DateTime.MaxValue,
                    RevokedBy = ""
                };

                _context.Users.Add(NewUser);
                _context.SaveChanges();
            }
        }

        [HttpPost("Create User")]
        public async Task<ActionResult> CreateUser(string login, string password, string UserLogin, string UserPassword, string UserName, int UserGender, DateTime? UserBirthday, bool isAdmin)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (!RequestSender.Admin)
                return BadRequest("User can be created only by admins");

            if (!RequestSender.Admin)       // Параметр администратора может проставить только администратор
                isAdmin = false;

            if (!validator.PasswordLoginValidate(UserLogin))
                return BadRequest("Invalid new user login: Login must contains only EU letters and numbers");

            if (_context.Users.Where(x => x.Login == UserLogin).FirstOrDefault() != default)
                return BadRequest("Invalid new user login: User with this login already exist");

            if (!validator.PasswordLoginValidate(UserPassword))
                return BadRequest("Invalid new user password: Password must contains only EU letters and numbers");

            if (!validator.NameValidate(UserName))
                return BadRequest("Invalid new user name: Name must contains only EU/RU letters");

            if (!validator.BirthdayValidate(UserBirthday))
                return BadRequest("Invalid new user birthday: Birthday cannot be greater than the current day");

            if (!validator.GenderValidate(UserGender))
                return BadRequest("Invalid new user gender: 0 - is male, 1 - is female, 2 - is unknown");

            User NewUser = new User()
            {
                Guid = Guid.NewGuid(),
                Login = UserLogin,
                Password = UserPassword,
                Name = UserName,
                Gender = UserGender,
                Birthday = UserBirthday,
                Admin = isAdmin,
                CreatedOn = DateTime.Now,
                CreatedBy = RequestSender.Login,
                ModifiedOn = DateTime.Now,
                ModifiedBy = RequestSender.Login,
                RevokedOn = DateTime.MaxValue,      // Пользователь считается не активным если проставленна любая дата отличная от максимальной
                RevokedBy = ""
            };

            _context.Users.Add(NewUser);
            await _context.SaveChangesAsync();

            return Ok("User Added");
        }

        [HttpPut("Update User")]
        public async Task<ActionResult> UpdateUser(string login, string password, string UserLogin, string? NewUserName = null, int NewUserGender = -1, DateTime? NewUserBirthday = null)
        {
            // Данный метод не обязательно должен принимать все параметры для изминения  
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();
            var ModifiedUser = _context.Users.Where(x => x.Login == UserLogin).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (ModifiedUser == default)
                return BadRequest("User dosen't exist");

            if (!attributeCheck.IsSenderAdminOrHimself(RequestSender, UserLogin))
                return BadRequest("User can be updated only by admin or himself(User must be active)");

            if (NewUserName != null)
            {
                if (validator.NameValidate(NewUserName)) { ModifiedUser.Name = NewUserName; }
                else { return BadRequest("Invalid new user name: Name must contains only EU/RU letters"); }
            }

            if (NewUserGender != -1)
            {
                if (validator.GenderValidate(NewUserGender)) { ModifiedUser.Gender = NewUserGender; }
                else { return BadRequest("Invalid new user gender: 0 - is male, 1 - is female, 2 - is unknown"); }
            }

            if (NewUserBirthday != null)
            {
                if (validator.BirthdayValidate(NewUserBirthday)) { ModifiedUser.Birthday = NewUserBirthday; }
                else { return BadRequest("Invalid new user birthday: Birthday cannot be greater than the current day"); }
            }

            ModifiedUser.ModifiedBy = login;
            ModifiedUser.ModifiedOn = DateTime.Now;

            _context.Users.Update(ModifiedUser);
            await _context.SaveChangesAsync();
            return Ok("User Updated");
        }

        [HttpPut("Update Password")]
        public async Task<ActionResult> UpdatePassword(string login, string password, string UserLogin, string NewPassword)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();
            var ModifiedUser = _context.Users.Where(x => x.Login == UserLogin).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (ModifiedUser == default)
                return BadRequest("User dosen't exist");

            if (!attributeCheck.IsSenderAdminOrHimself(RequestSender, UserLogin))
                return BadRequest("User can be updated only by admin or himself(User must be active)");

            if (!validator.PasswordLoginValidate(NewPassword))
                return BadRequest("Invalid new user password: Password must contains only EU letters and numbers");

            ModifiedUser.Password = NewPassword;
            ModifiedUser.ModifiedBy = login;
            ModifiedUser.ModifiedOn = DateTime.Now;

            _context.Users.Update(ModifiedUser);
            await _context.SaveChangesAsync();
            return Ok("User password updated");
        }

        [HttpPut("Update Login")]
        public async Task<ActionResult> UpdateLogin(string login, string password, string UserLogin, string NewLogin)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();
            var ModifiedUser = _context.Users.Where(x => x.Login == UserLogin).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (ModifiedUser == default)
                return BadRequest("User dosen't exist");

            if (!attributeCheck.IsSenderAdminOrHimself(RequestSender, UserLogin))
                return BadRequest("User can be updated only by admin or himself(User must be active)");

            if (!validator.PasswordLoginValidate(NewLogin))
                return BadRequest("Invalid new user login: Login must contains only EU letters and numbers");

            if (_context.Users.Where(x => x.Login == NewLogin).FirstOrDefault() != default)
                return BadRequest("Users new login must be unique");

            ModifiedUser.Login = NewLogin;
            ModifiedUser.ModifiedBy = login;
            ModifiedUser.ModifiedOn = DateTime.Now;

            _context.Users.Update(ModifiedUser);
            await _context.SaveChangesAsync();
            return Ok("User password updated");
        }

        [HttpPut("User Recovery")]
        public async Task<ActionResult> RecoveryUser(string login, string password, string UserLogin)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();
            var UserToRecovery = _context.Users.Where(x => x.Login == UserLogin).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (!RequestSender.Admin)
                return BadRequest("Only admins can do this request");

            if (RequestSender.Login == UserLogin)
                return BadRequest("User can't recovery himself");

            if (UserToRecovery == default)
                return BadRequest("User dosen't exist");

            if (UserToRecovery.RevokedOn == DateTime.MaxValue)
                return BadRequest("User is active");

            UserToRecovery.RevokedOn = DateTime.MaxValue;
            UserToRecovery.RevokedBy = "";
            UserToRecovery.ModifiedOn = DateTime.Now;
            UserToRecovery.ModifiedBy = login;

            _context.Users.Update(UserToRecovery);
            await _context.SaveChangesAsync();
            return Ok("User recovery sucessfull");

        }

        [HttpGet("Get All Active Users")]
        public ActionResult<List<User>> GetAllActiveUsers(string login, string password)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (!RequestSender.Admin)
                return BadRequest("Only admins can do this request");

            var ActiveUsers = _context.Users
                .Where(x => x.RevokedOn == DateTime.MaxValue)
                .OrderBy(x => x.CreatedOn)
                .ToList();

            return Ok(ActiveUsers);
        }

        [HttpGet("Get User By Login")]
        public ActionResult<UserFields> GetUserByLogin(string login, string password, string UserLogin)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();
            var UserToGet = _context.Users.Where(x => x.Login == UserLogin).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (!RequestSender.Admin)
                return BadRequest("Only admins can do this request");

            if (UserToGet == default)
                return BadRequest("User dosen't exist");

            UserFields userFields = new UserFields()
            {
                Name = UserToGet.Name,
                Gender = UserToGet.Gender,
                Birthday = UserToGet.Birthday,
                IsActive = false
            };

            userFields.IsActive = UserToGet.RevokedOn == DateTime.MaxValue ? true : false;

            return BadRequest(userFields);
        }

        [HttpGet("Get User")]
        public ActionResult<User> GetUser(string login, string password)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (RequestSender.RevokedOn != DateTime.MaxValue)
                return BadRequest("User must be active");

            return Ok(RequestSender);
        }

        [HttpGet("Get Older Users")]
        public ActionResult<List<User>> GetOlderUsers(string login, string password, int age)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();
            var ResponseList = new List<User>();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (!RequestSender.Admin)
                return BadRequest("Only admins can do this request");

            var UserList = _context.Users.ToList();

            foreach (var user in UserList)
            {
                DateTime BirthDate = user.Birthday ?? DateTime.MaxValue;

                if (BirthDate == DateTime.MaxValue)
                    continue;

                DateTime now = DateTime.Today;
                int _age = now.Year - BirthDate.Year;
                if (BirthDate.AddYears(_age) > now)
                    _age--;

                if (_age > age)
                    ResponseList.Add(user);
            }

            return Ok(ResponseList);
        }

        [HttpDelete("Delet User")]
        public async Task<ActionResult> DeleteUser(string login, string password, string UserLogin, bool isHard)
        {
            var RequestSender = _context.Users.Where(x => x.Login == login).FirstOrDefault();
            var UserToDelete = _context.Users.Where(x => x.Login == UserLogin).FirstOrDefault();

            if (!attributeCheck.IsSenderAttributCorrect(RequestSender, password))
                return BadRequest("Invalid request sender attribute");

            if (!RequestSender.Admin)
                return BadRequest("Only admins can do this request");

            if(RequestSender.Login == UserLogin)
                return BadRequest("User can't delete himself");

            if (UserToDelete == default)
                return BadRequest("User dosen't exist");

            if (isHard)
            {
                _context.Users.Remove(UserToDelete);
                await _context.SaveChangesAsync();
                return Ok("Hard remove sucessfull");
            }
            else
            {
                if (UserToDelete.RevokedOn != DateTime.MaxValue)
                    return BadRequest("User inactive, try hard remove");

                UserToDelete.RevokedOn = DateTime.Now;
                UserToDelete.RevokedBy = login;
                UserToDelete.ModifiedOn = DateTime.Now;
                UserToDelete.ModifiedBy = login;

                _context.Users.Update(UserToDelete);
                await _context.SaveChangesAsync();
                return Ok("Not hard remove sucessfull");
            }

        }
    }
}

