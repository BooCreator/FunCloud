using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Controllers
{
    public class RequestController : Controller
    {

        public void SetLists(DataBaseExtended DB)
        {
            Entity Categories = Context.Categories.Get(DB);
            Entity Fandomes = Context.Fandomes.Get(DB);

            this.ViewBag.Categories = new SelectList(To.Typle(Categories), "Name", "Value");
            this.ViewBag.Fandomes = new SelectList(To.Typle(Fandomes), "Name", "Value");
        }

        public Int32 SetUserInfo(DataBaseExtended DB)
        {
            int UserID = Global.GetUserID(this);
            this.ViewBag.UserID = UserID;
            this.ViewBag.UserRole = Global.GetUserRole(this);
            if (UserID > -1)
            {
                Context.Users.Update(DB, Context.Users.Log_date.Name,
                    $"'{DateTime.Now.ToShortDateString()}'", $"{Context.Users.ID.Name} = {UserID}");
                return 1;
            }
            else
                return -1;
        }

        // GET: Request
        public ActionResult Index(int page = 0)
        {
            int max_in_page = Int32.Parse(WebConfigurationManager.AppSettings["MaxWorkInPage"]);

            this.ViewBag.Action = "Request?";

            var Items = new List<View.RequestView>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetUserInfo(DB);

                this.ViewBag.Count = Context.Requests.Count(DB);
                this.ViewBag.Pages = (int)Math.Ceiling((double)this.ViewBag.Count / max_in_page);
                this.ViewBag.Page = page;

                List<Models.DataBase.Request> Requests = Context.Requests.FromEntity(Context.Requests.Get(DB, max_in_page, max_in_page * page));

                foreach (Models.DataBase.Request Item in Requests)
                    Items.Add(new View.RequestView(DB, Item));
            }
            this.ViewBag.Items = Items;
            return this.View();
        }

        public ActionResult Public(Request.RequestPublicModel model)
        {
            this.ViewBag.isPublish = false;

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetLists(DB);

                if (this.SetUserInfo(DB) == 1)
                {
                    if (this.ModelState.IsValid)
                    {
                        Context.Requests.Add(DB, model.ToAttributes());
                        this.ViewBag.isPublish = true;
                        return this.View();
                    }
                }
                else
                    this.ModelState.AddModelError("", "Войдите в учетную запись!");
            }
            return this.View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id = -1)
        {
            this.ViewBag.isPublish = false;
            this.ViewBag.isAccess = false;
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetLists(DB);
                this.SetUserInfo(DB);

                Models.DataBase.Request Item = Context.Requests.Find(DB, $"{Context.Requests.ID.Name} = {id}", out _);

                if (Item != null && Global.GetUserID(this) == Item.Author.Value || Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    this.ViewBag.isAccess = true;
                    return this.View(new Request.RequestPublicModel(Item));
                }
                else
                    this.ModelState.AddModelError("", "Вы не являетесь автором заявки либо заявка не найдена!");
            }
            return this.View(new Request.RequestPublicModel());
        }
        [HttpPost]
        public ActionResult Edit(Request.RequestPublicModel model)
        {
            this.ViewBag.isPublish = false;
            this.ViewBag.isAccess = true;
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetLists(DB);
                this.SetUserInfo(DB);
                if (this.ModelState.IsValid)
                {
                    if (Global.GetUserID(this) == model.Author)
                    {
                        if (Context.Requests.Update(DB, Context.Requests.Fields, model.ToAttributes(), $"{Context.Requests.ID.Name} = {model.ID}"))
                        {
                            this.ViewBag.isPublish = true;
                            return this.View(model);
                        }
                        else
                            this.ModelState.AddModelError("", "Произошла ошибка при изменении данных!");
                    }
                }

            }
            return this.View(model);
        }

        [HttpPost]
        public String Remove(int id = -1)
        {
            this.ViewBag.isRemove = "false";

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Models.DataBase.Request Item = Context.Requests.Find(DB, $"{Context.Requests.ID.Name} = {id}", out _);

                if (Item != null && Global.GetUserID(this) == Item.Author.Value || Global.GetUserRole(this) == Global.AdminRoleID)
                    this.ViewBag.isRemove = Context.Requests.Remove(DB, $"{Context.Requests.ID.Name} = {id}");
                else return Error.NotAccess;
            }
            return Error.Accept;
        }

        public ActionResult View(int id)
        {

            this.ViewBag.Action = $"Request/View?id={id}";

            var Items = new List<View.RequestView>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetUserInfo(DB);
                List<Models.DataBase.Request> Requests = Context.Requests.FromEntity(Context.Requests.Get(DB, $"{Context.Requests.ID.Name} = {id}"));
                foreach (Models.DataBase.Request Item in Requests)
                    Items.Add(new FunCloud.View.RequestView(DB, Item));
            }

            this.ViewBag.Item = (Items.Count > 0) ? Items[0] : null;

            return this.View();
        }
        [HttpPost]
        public ActionResult Find(Find.FindPublicModel model)
            => this.Find(model.Text, model.Author, model.Category, model.Fandome, model.Page);
        public ActionResult Find(String text = "",
            Int32 author = -1, Int32 category = -1, Int32 fandome = -1,
            Int32 page = 0)
        {
            string where = "1 = 1";

            if (text?.Length > 0)
                where += $" and {Context.Works.Title.Name} Like '%{text}%'";

            if (author > -1)
                where += $" and {Context.Works.Author.Name} = {author}";

            if (category > -1)
                where += $" and {Context.Works.Category.Name} = {category}";

            if (fandome > -1)
                where += $" and {Context.Works.Fandome.Name} = {fandome}";

            // -- find --

            int max_in_page = Int32.Parse(WebConfigurationManager.AppSettings["MaxWorkInPage"]);

            var action = new StringBuilder("Request/Find?");

            bool amp = false;

            if (text?.Length > 0)
            {
                action.Append($"text={text}");
                amp = true;
            }
            if (author > -1)
            {
                action.Append($"{((amp) ? "&" : "")}author={author}");
                amp = true;
            }
            if (category > -1)
            {
                action.Append($"{((amp) ? "&" : "")}category={category}");
                amp = true;
            }
            if (fandome > -1)
            {
                action.Append($"{((amp) ? "&" : "")}fandome={fandome}");
                amp = true;
            }

            this.ViewBag.Action = action.ToString();

            var Items = new List<View.RequestView>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                Entity Categories = DB.Select($"select id, {Context.Categories.Title.Name} from {Context.Categories.Table} order by {Context.Categories.Title.Name}");
                Entity Fandomes = DB.Select($"select id, {Context.Fandomes.Title.Name} from {Context.Fandomes.Table} order by {Context.Fandomes.Title.Name}");

                List<Typle> TCategories = To.Typle(Categories);
                List<Typle> TFandomes = To.Typle(Fandomes);

                TCategories.Insert(0, new Typle("-1", "Любой фандом"));
                TFandomes.Insert(0, new Typle("-1", "Любая вселенная"));

                this.ViewBag.Categories = new SelectList(TCategories, "Name", "Value");
                this.ViewBag.Fandomes = new SelectList(TFandomes, "Name", "Value");

                this.SetUserInfo(DB);

                this.ViewBag.Count = Context.Works.Count(DB, where);
                this.ViewBag.Pages = (int)Math.Ceiling((double)this.ViewBag.Count / max_in_page);
                this.ViewBag.Page = page;

                List<Models.DataBase.Request> Requests = Context.Requests.FromEntity(Context.Requests.Get(DB, max_in_page, max_in_page * page, where));

                foreach (Models.DataBase.Request Item in Requests)
                    Items.Add(new View.RequestView(DB, Item));

            }

            this.ViewBag.Items = Items;

            // -- end --

            return this.View(new Find.FindPublicModel() { Text = text, Author = author, Category = category, Fandome = fandome, Page = page });

        }

    }
}