using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataBaseConnector;
using DataBaseConnector.Ext;
using FunCloud.Models.PublicModel.AdminPanel;

namespace FunCloud.Controllers
{
    public class AdminPanelController : Controller
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

        public void SetLists(DataBase DB)
        {
            Entity Categories = DB.Select($"select id, {Context.Categories.Title.Name} from {Context.Categories.Table}");
            Entity States = DB.Select($"select id, {Context.States.Title.Name} from {Context.States.Table}");
            Entity Fandomes = DB.Select($"select id, {Context.Fandomes.Title.Name} from {Context.Fandomes.Table}");
            Entity Serials = DB.Select($"select id, {Context.Serials.Title.Name} from {Context.Serials.Table} where {Context.Serials.Author.Name} = {Global.GetUserID(this)} or {Context.Serials.Author.Name} = -1");

            this.ViewBag.Categories = To.Typle(Categories);
            this.ViewBag.States = To.Typle(States);
            this.ViewBag.Fandomes = To.Typle(Fandomes);

            List<Typle> Typles = To.Typle(Serials);
            Typles.Insert(0, new Typle("-1", "Без серии"));

            this.ViewBag.Serials = Typles;
        }

        #region сложные таблицы

        [HttpPost]
        public String Users(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {
                            return (Context.Users.Update(DB, Context.Users.Login.Name, $"'{title}'", $"{Context.Users.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Users.Remove(DB, $"{Context.Users.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return Error.Exists;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }
        
        [HttpPost]
        public String UsersRole(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {
                            return (Context.Users.Update(DB, Context.Users.Role.Name, $"{title}", $"{Context.Users.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return Error.Exists;
                        }
                    }
                    else
                    {
                        return Error.Exists;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Users()
        {
            var Users = new List<Account.UserModel>();
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Entity Roles = DB.Select($"select {Context.Roles.ID.Name}, {Context.Roles.Title.Name} from {Context.Roles.Table}");
                this.ViewBag.Roles = To.Typle(Roles);
                foreach (Models.DataBase.User item in Context.Users.FromEntity(Context.Users.Get(DB)))
                {
                    Users.Add(new Account.UserModel(item));
                }
            }
            return this.View(new Table<Account.UserModel>(Users));
        }

        [HttpPost]
        public String WorksOne(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {
                            return (Context.Works.Update(DB, Context.Works.Title.Name, $"'{title}'", $"{Context.Works.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Works.Remove(DB, $"{Context.Works.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return Error.Exists;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpPost]
        public String Works(Int32 id, String title, Int32 category, Int32 fandome, Int32 state)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {
                            return (Context.Works.Update(DB,
                                new string[] { Context.Works.Title.Name, Context.Works.Category.Name, Context.Works.Fandome.Name, Context.Works.State.Name },
                                new string[] { $"'{title}'", To.String(category), To.String(fandome), To.String(state) },
                                $"{Context.Works.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Works.Remove(DB, $"{Context.Works.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return Error.Exists;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Works()
        {
            var Works = new List<Work.WorkPublicModel>();
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetLists(DB);
                foreach (Models.DataBase.Work item in Context.Works.FromEntity(Context.Works.Get(DB)))
                {
                    Works.Add(new Work.WorkPublicModel(item));
                }
            }
            return this.View(new Table<Work.WorkPublicModel>(Works));
        }

        [HttpPost]
        public String RequestsOne(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {
                            return (Context.Requests.Update(DB, Context.Requests.Title.Name, $"'{title}'", $"{Context.Requests.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Requests.Remove(DB, $"{Context.Requests.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return Error.Exists;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }
        [HttpPost]
        public String Requests(Int32 id, String title, Int32 category, Int32 fandome)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {
                            return (Context.Requests.Update(DB,
                                new string[] { Context.Requests.Title.Name, Context.Requests.Category.Name, Context.Requests.Fandome.Name },
                                new string[] { $"'{title}'", To.String(category), To.String(fandome) },
                                $"{Context.Requests.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Requests.Remove(DB, $"{Context.Requests.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return Error.Exists;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Requests()
        {
            var Requests = new List<Request.RequestPublicModel>();
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetLists(DB);
                foreach (Models.DataBase.Request item in Context.Requests.FromEntity(Context.Requests.Get(DB)))
                {
                    Requests.Add(new Request.RequestPublicModel(item));
                }
            }
            return this.View(new Table<Request.RequestPublicModel>(Requests));
        }

        [HttpPost]
        public String Comments(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {

                            return (Context.Comments.Update(DB, Context.Comments.Text.Name, $"'{title}'", $"{Context.Comments.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Comments.Remove(DB, $"{Context.Comments.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return Error.Exists;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Comments()
        {
            var Comments = new List<Comment.CommentPublicModel>();
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                foreach(Models.DataBase.Comment item in Context.Comments.FromEntity(Context.Comments.Get(DB)))
                {
                    Comments.Add(new Comment.CommentPublicModel(item));
                }
            }
            return this.View(new Table<Comment.CommentPublicModel>(Comments));
        }

        #endregion

        #region простые таблицы 

        [HttpPost]
        public String Categories(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {

                            return (Context.Categories.Update(DB, Context.Categories.Title.Name, $"'{title}'", $"{Context.Categories.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Categories.Remove(DB, $"{Context.Categories.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return (Context.Categories.Add(DB, new string[] { $"'{title}'" }))
                            ? Error.Accept
                            : Error.Unknown;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Categories()
        {
            List<Models.DataBase.Category> Categories = null;
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Categories = Context.Categories.FromEntity(Context.Categories.Get(DB));
            }
            return this.View(new Table<Models.DataBase.Category>(Categories));
        }

        [HttpPost]
        public String Fandoms(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {

                            return (Context.Fandomes.Update(DB, Context.Fandomes.Title.Name, $"'{title}'", $"{Context.Fandomes.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Fandomes.Remove(DB, $"{Context.Fandomes.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return (Context.Fandomes.Add(DB, new string[] { $"'{title}'" }))
                            ? Error.Accept
                            : Error.Unknown;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Fandoms()
        {
            List<Models.DataBase.Fandome> Fandoms = null;
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Fandoms = Context.Fandomes.FromEntity(Context.Fandomes.Get(DB));
            }
            return this.View(new Table<Models.DataBase.Fandome>(Fandoms));
        }

        [HttpPost]
        public String Roles(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {

                            return (Context.Roles.Update(DB, Context.Roles.Title.Name, $"'{title}'", $"{Context.Roles.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        } else
                        {
                            return (Context.Roles.Remove(DB, $"{Context.Roles.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    } else
                    {
                        return (Context.Roles.Add(DB, new string[] { $"'{title}'" }))
                            ? Error.Accept
                            : Error.Unknown;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Roles()
        {
            List<Models.DataBase.Role> Roles = null;
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Roles = Context.Roles.FromEntity(Context.Roles.Get(DB));
            }
            return this.View(new Table<Models.DataBase.Role>(Roles));
        }

        [HttpPost]
        public String States(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {

                            return (Context.States.Update(DB, Context.States.Title.Name, $"'{title}'", $"{Context.States.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.States.Remove(DB, $"{Context.States.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return (Context.States.Add(DB, new string[] { $"'{title}'" }))
                            ? Error.Accept
                            : Error.Unknown;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult States()
        {
            List<Models.DataBase.State> States = null;
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                States = Context.States.FromEntity(Context.States.Get(DB));
            }
            return this.View(new Table<Models.DataBase.State>(States));
        }

        [HttpPost]
        public String Marks(Int32 id, String title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (this.SetUserInfo(DB) == 1 && Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    if (id > -1)
                    {
                        if (title.Length > 0)
                        {

                            return (Context.Marks.Update(DB, Context.Marks.Title.Name, $"'{title}'", $"{Context.Marks.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                        else
                        {
                            return (Context.Marks.Remove(DB, $"{Context.Marks.ID.Name} = {id}"))
                                ? Error.Accept
                                : Error.Unknown;
                        }
                    }
                    else
                    {
                        return (Context.Marks.Add(DB, new string[] { $"'{title}'" }))
                            ? Error.Accept
                            : Error.Unknown;
                    }
                }
                else
                    return Error.NotAccess;
            }
        }

        [HttpGet]
        public ActionResult Marks()
        {
            List<Models.DataBase.Mark> Marks = null;
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Marks = Context.Marks.FromEntity(Context.Marks.Get(DB));
            }
            return this.View(new Table<Models.DataBase.Mark>(Marks));
        }

        #endregion

    }
}