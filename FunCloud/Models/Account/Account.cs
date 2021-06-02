using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FunCloud.Account
{
    public class LoginModel
    {
        [Required]
        public String Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        public String Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public String ConfirmPassword { get; set; }


    }

    public class ProfileModel
    {
        public Int32 ID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        public String NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают!")]
        public String ConfirmPassword { get; set; }


    }

    public class UserModel
    {
        public UserModel(Models.DataBase.User User)
        {
            this.ID = User.ID.Value;
            this.Login = User.Login.Value;
            this.Role = User.Role.Value;
        }

        public Int32 ID { get; set; }
        public String Login { get; set; }
        public Int32 Role { get; set; }
    }
}