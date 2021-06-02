using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Web.Security;
using FunCloud.Models.DataBase;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Controllers
{
    public class ProfileController : Controller
    {
        public Int32 SetUserInfo(DataBaseExtended DB)
        {
            int UserID = Global.GetUserID(this);
            this.ViewBag.UserID = UserID;
            if (UserID > -1)
            {
                Context.Users.Update(DB, Context.Users.Log_date.Name,
                    $"'{DateTime.Now.ToShortDateString()}'", $"{Context.Users.ID.Name} = {UserID}");
                return 1;
            }
            else
                return -1;
        }

        // GET: Profile
        public ActionResult View(int id)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetUserInfo(DB);
                User User = Context.Users.Find(DB, $"{Context.Users.ID.Name} = {id}", out _);
                if (User != null)
                    this.ViewBag.User = new View.UserView(DB, User);
            }
            return this.View();
        }

        [HttpGet]
        public ActionResult Edit()
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetUserInfo(DB);
            }
            return this.View(new Account.ProfileModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account.ProfileModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    User user = Context.Users.Find(DB,
                        $"{Context.Users.ID.Name} = '{model.ID}' and {Context.Users.Password.Name} like '{AccountController.SHA1(model.Password)}'",
                        out bool _);

                    if (user != null)
                    {
                        if (this.SetUserInfo(DB) == 1 && Global.GetUserID(this) == model.ID)
                        {
                            bool isEdited = (model.NewPassword?.Length > 0)
                                ? Context.Users.Update(DB,
                                    new string[] { Context.Users.Login.Name, Context.Users.Password.Name },
                                    new string[] { $"'{model.Name}'", $"'{AccountController.SHA1(model.NewPassword)}'" }, $"{Context.Users.ID.Name} = {model.ID}")
                                : Context.Users.Update(DB, Context.Users.Login.Name, $"'{model.Name}'", $"{Context.Users.ID.Name} = {model.ID}");

                            if (isEdited)
                            {
                                FormsAuthentication.SetAuthCookie(model.Name, true);
                                Global.LoginedUser.Find(x => x.UserID == model.ID).UserName = model.Name;
                                return this.RedirectToAction("View", "Profile", new { model.ID });
                            }
                            else
                                this.ModelState.AddModelError("", "Произошла ошибка при изменении данных!");
                        }
                        else
                            this.ModelState.AddModelError("", "Доступ запрещен, т.к. обнаружена попытка изменения чужого профиля!");
                    }
                    else
                        this.ModelState.AddModelError("", "Введенный пароль не верный!");
                }
            }

            return this.View(model);
        }

        [HttpPost]
        public JsonResult Remove(Int32 id)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (Global.GetUserID(this) == id)
                {
                    if (Context.Users.Remove(DB, $"{Context.Users.ID.Name} = {id}"))
                    {
                        Context.Serials.Update(DB, $"{Context.Serials.Author.Name}", "-1", $"{Context.Serials.Author.Name} = {id}");
                        Global.LoginedUser.Remove(Global.LoginedUser.Find(x => x.UserID == Global.GetUserID(this)));
                        FormsAuthentication.SignOut();
                        return this.Json(Error.Accept);
                    }
                    else 
                        return this.Json(Error.IsEmpty);
                }

            }
            return this.Json(Error.NotAccess);
        }

    }
}