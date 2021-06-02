using System;
using System.Web.Security;
using System.Web.Mvc;
using DataBaseConnector;
using System.Web.Configuration;
using System.Security.Cryptography;
using FunCloud.Models.DataBase;
using System.Text;

namespace FunCloud.Controllers
{
    public class AccountController : Controller
    {

        public static String SHA1(String str) 
            => Encoding.UTF8.GetString(getHash(Encoding.UTF8.GetBytes(str))).Replace("'","\"");
        private static byte[] getHash(byte[] bytes)
        {
            using (var sha = System.Security.Cryptography.SHA1.Create())
            {
                byte[] hash = sha.ComputeHash(sha.ComputeHash(bytes));
                return hash;
            }
        }

        public ActionResult Login() 
            => this.View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account.LoginModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    User user = Context.Users.Find(DB,
                        $"{Context.Users.Login.Name} like '{model.Name}' and {Context.Users.Password.Name} like '{SHA1(model.Password)}'",
                        out bool _);

                    if (user != null)
                    {
                        Global.LoginedUser.Add(new UserData(user.ID.Value, user.Login.Value, user.Role.Value));
                        FormsAuthentication.SetAuthCookie(model.Name, true);
                        return this.RedirectToAction("Index", "Home");
                    }
                    else
                        this.ModelState.AddModelError("", "Пользователя с таким логином и паролем нет!");
                }
            }

            return this.View(model);
        }

        public ActionResult Register() 
            => this.View();
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account.RegisterModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    User user = Context.Users.Find(DB,
                        $"{Context.Users.Login.Name} like '{model.Name}'", out _);

                    if (user == null)
                    {
                        if (Context.Users.Add(DB, model.Name, SHA1(model.Password).ToString(), 1))
                        {
                            int newID = DB.Scalar($"select max({Context.Users.ID.Name}) from {Context.Users.Table}");
                            Global.LoginedUser.Add(new UserData(newID, model.Name, 1));
                            FormsAuthentication.SetAuthCookie(model.Name, true);
                            return this.RedirectToAction("Index", "Home");
                        }
                        else
                            this.ModelState.AddModelError("", "Произошла ошибка регистрации!");
                    }
                    else
                        this.ModelState.AddModelError("", "Пользователь с таким логином уже существует!");

                }
            }

            return this.View(model);
        }
        
        public ActionResult Logoff()
        {
            Global.LoginedUser.Remove(Global.LoginedUser.Find(x => x.UserID == Global.GetUserID(this)));
            FormsAuthentication.SignOut();
            return this.RedirectToAction("Index", "Home");
        }

    }
}