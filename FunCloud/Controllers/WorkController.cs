using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DataBaseConnector;
using DataBaseConnector.Ext;
using System.Text;
using iTextSharp.text.pdf;

namespace FunCloud.Controllers
{
    public class WorkController : Controller
    {

        public void SetLists(DataBase DB)
        {
            Entity Categories = DB.Select($"select id, {Context.Categories.Title.Name} from {Context.Categories.Table}");
            Entity States = DB.Select($"select id, {Context.States.Title.Name} from {Context.States.Table}");
            Entity Fandomes = DB.Select($"select id, {Context.Fandomes.Title.Name} from {Context.Fandomes.Table}");
            Entity Serials = DB.Select($"select id, {Context.Serials.Title.Name} from {Context.Serials.Table} where {Context.Serials.Author.Name} = {Global.GetUserID(this)} or {Context.Serials.Author.Name} = -1");
            Entity Lists = DB.Select($"select id, {Context.Lists.Title.Name} from {Context.Lists.Table} where {Context.Lists.Author.Name} = {Global.GetUserID(this)}");

            this.ViewBag.Categories = new SelectList(To.Typle(Categories), "Name", "Value");
            this.ViewBag.States = new SelectList(To.Typle(States), "Name", "Value");
            this.ViewBag.Fandomes = new SelectList(To.Typle(Fandomes), "Name", "Value");

            List<Typle> Typles = To.Typle(Serials);
            Typles.Insert(0, new Typle("-1", "Без серии"));

            this.ViewBag.Serials = new SelectList(Typles, "Name", "Value");

            Typles = To.Typle(Lists);

            this.ViewBag.Lists = Typles;
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

        // GET: Works
        public ActionResult Index(Int32 page = 0)
        {

            int max_in_page = Int32.Parse(WebConfigurationManager.AppSettings["MaxWorkInPage"]);

            this.ViewBag.Action = "Work?";

            var Items = new List<View.WorkView>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetUserInfo(DB);

                this.ViewBag.Count = Context.Works.Count(DB);
                this.ViewBag.Pages = (int)Math.Ceiling((double)this.ViewBag.Count / max_in_page);
                this.ViewBag.Page = page;

                List<Models.DataBase.Work> Works = Context.Works.FromEntity(Context.Works.Get(DB, max_in_page, max_in_page * page, $"1 = 1"));

                foreach (Models.DataBase.Work Item in Works)
                    Items.Add(new FunCloud.View.WorkView(DB, Item, this.Server.MapPath(Context.Works.FilesPath)));

            }

            this.ViewBag.Items = Items;
            return this.View();
        }
        [HttpGet]
        public ActionResult Public(Int32 Request = -1)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetLists(DB);
                this.SetUserInfo(DB);
            }
            return this.View(new Work.WorkPublicModel() { Request = Request });
        }

        public ActionResult Public(Work.WorkPublicModel model, HttpPostedFileBase basefile, IEnumerable<HttpPostedFileBase> uploads)
        {

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                this.ViewBag.Request = model.Request;

                int val = this.SetUserInfo(DB);
                this.SetLists(DB);

                if (val == 1)
                {
                    if (this.ModelState.IsValid)
                    {
                        this.ViewBag.isPublish = false;
                        this.ViewBag.isFileLoad = false;
                        this.ViewBag.isFilesLoad = false;

                        Context.Works.Add(DB, model.ToAttributes());
                        model.ID = DB.Scalar($"select max(id) from {Context.Works.Table}");

                        if (model.Request > -1)
                            if (DB.Scalar($"select count({Context.WorksOnRequest.ID.Name}) from {Context.WorksOnRequest.Table} where {Context.WorksOnRequest.Work.Name} = {model.ID} and {Context.WorksOnRequest.Request.Name} = {model.Request}") < 1)
                                Context.WorksOnRequest.Add(DB, new string[] { model.ID.ToString(), model.Request.ToString() });

                        if (model.Serial > -1)
                            if (DB.Scalar($"select count({Context.WorksInSerial.ID.Name}) from {Context.WorksInSerial.Table} where {Context.WorksInSerial.Work.Name} = {model.ID} and {Context.WorksInSerial.Serial.Name} = {model.Serial}") < 1)
                                Context.WorksInSerial.Add(DB, new string[] { model.ID.ToString(), model.Serial.ToString() });

                        if (!Directory.Exists(this.Server.MapPath(Context.Works.FilesPath + model.ID)))
                            Directory.CreateDirectory(this.Server.MapPath(Context.Works.FilesPath + model.ID));

                        var filenames = new List<string>();

                        if (basefile != null)
                        {
                            string ext = System.IO.Path.GetExtension(basefile.FileName);
                            // сохраняем файл в папку Files в проекте
                            basefile.SaveAs(this.Server.MapPath(Context.Works.FilesPath + model.ID + "/" + Context.Works.MainFileName + ext));
                            this.ViewBag.isFileLoad = true;
                        }

                        foreach (HttpPostedFileBase upload in uploads)
                        {
                            if (upload != null)
                            {
                                // получаем имя файла
                                string fileName = System.IO.Path.GetFileName(upload.FileName);
                                // сохраняем файл в папку Files в проекте
                                upload.SaveAs(this.Server.MapPath(Context.Works.FilesPath + model.ID + "/" + fileName));
                                filenames.Add(fileName);
                                this.ViewBag.isFilesLoad = true;
                            }
                        }

                        if (this.ViewBag.isFilesLoad)
                        {
                            Context.Works.AddFiles(DB, model.ID, To.String(filenames.ToArray(), ","));
                        }

                        this.ViewBag.isPublish = "true";
                        return this.View();
                    }
                }
            }
            return this.View(model);
        }
        
        [HttpPost]
        public ActionResult Edit(Work.WorkPublicModel model, HttpPostedFileBase basefile = null)
        {

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                int val = this.SetUserInfo(DB);
                this.SetLists(DB);

                if (val == 1)
                {
                    if (Global.GetUserID(this) == model.Author)
                    {
                        Context.Works.Update(DB, Context.Works.Fields, model.ToAttributes(), $"{Context.Works.ID.Name} = {model.ID}");
                        
                        if (basefile != null)
                        {
                            string filepath = this.Server.MapPath(Context.Works.FilesPath + model.ID + "/");

                            if (!Directory.Exists(filepath))
                                Directory.CreateDirectory(filepath);

                            string[] files = Directory.GetFiles(filepath, Context.Works.MainFileName + ".*");
                            foreach (string file in files)
                                System.IO.File.Delete(file);

                            if (Directory.Exists(filepath + Context.Works.MainFileName + "/"))
                            {
                                files = Directory.GetFiles(filepath + Context.Works.MainFileName + "/", "*.*");
                                foreach (string file in files)
                                    System.IO.File.Delete(file);
                            }

                            string ext = System.IO.Path.GetExtension(basefile.FileName);
                            // сохраняем файл в папку Files в проекте
                            basefile.SaveAs(filepath + Context.Works.MainFileName + ext);
                            this.ViewBag.isFileLoad = true;
                        }

                        if (model.Request > -1)
                        {
                            if (DB.Scalar($"select count({Context.WorksOnRequest.ID.Name}) from {Context.WorksOnRequest.Table} where {Context.WorksOnRequest.Work.Name} = {model.ID} and {Context.WorksOnRequest.Request.Name} = {model.Request}") < 1)
                                Context.WorksOnRequest.Add(DB, new string[] { model.ID.ToString(), model.Request.ToString() });
                        }
                        else
                        {
                            Context.WorksOnRequest.Remove(DB, $"{Context.WorksOnRequest.Work.Name} = {model.ID} and {Context.WorksOnRequest.Request.Name} = {model.Request}");
                        }
                        if (model.Serial > -1)
                        {
                            if (DB.Scalar($"select count({Context.WorksInSerial.ID.Name}) from {Context.WorksInSerial.Table} where {Context.WorksInSerial.Work.Name} = {model.ID} and {Context.WorksInSerial.Serial.Name} = {model.Serial}") < 1)
                                Context.WorksInSerial.Add(DB, new string[] { model.ID.ToString(), model.Serial.ToString() });
                        }
                        else
                        {
                            Context.WorksInSerial.Remove(DB, $"{Context.WorksInSerial.Work.Name} = {model.ID} and {Context.WorksInSerial.Serial.Name} = {model.Serial}");
                        }
                        return this.RedirectToAction("View", "Work", new { model.ID });
                    }
                }

            }

            return this.View(model);
        }

        [HttpGet]
        public ActionResult Edit(Int32 id = -1)
        {
            this.ViewBag.isAccess= false;

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                this.SetLists(DB);
                this.SetUserInfo(DB);

                Models.DataBase.Work Item = Context.Works.Find(DB, $"{Context.Works.ID.Name} = {id}", out _);

                if (Item != null && Global.GetUserID(this) == Item.Author.Value || Global.GetUserRole(this) == Global.AdminRoleID)
                {
                    this.ViewBag.isAccess = true;
                    return this.View(new Work.WorkPublicModel(Item));
                }
                else
                    this.ModelState.AddModelError("", "Вы не являетесь автором заявки либо заявка не найдена!");

            }

            return this.View(new Work.WorkPublicModel());
        }
        [HttpPost]
        public String Remove(Int32 id = -1)
        {

            this.ViewBag.isRemove = "false";

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                if (this.SetUserInfo(DB) == 1)
                {
                    Models.DataBase.Work Item = Context.Works.Find(DB, $"{Context.Works.ID.Name} = {id}", out bool _);

                    if (Item != null && Item.Author.Value == Global.GetUserID(this) || Global.GetUserRole(this) == Global.AdminRoleID)
                    {
                        this.ViewBag.isRemove = "true";
                        Context.Works.Remove(DB, $"{Context.Works.ID.Name} = {id}");
                        Context.Likes.Remove(DB, $"{Context.Likes.Work.Name} = {id}");
                        Context.Comments.Remove(DB, $"{Context.Comments.Work.Name} = {id}");

                        string filepath = this.Server.MapPath(Item.FilesPath + Item.ID.Value + "/");

                        if (System.IO.File.Exists(filepath + "text.txt"))
                            System.IO.File.Delete(filepath + "text.txt");

                        foreach (string FileName in From.String(Item.Files.Value, ","))
                        {
                            if (System.IO.File.Exists(filepath + FileName))
                                System.IO.File.Delete(filepath + FileName);
                        }
                        if (System.IO.Directory.Exists(filepath))
                            System.IO.Directory.Delete(filepath);
                    }
                    else return Error.NotAccess;
                }
                else return Error.NotAccess;

            }

            return Error.Accept;
        }
        [HttpPost]
        public String Like(Int32 id, Int32 author, Boolean isLike)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                string field = (isLike) ? Context.Works.Like.Name : Context.Works.Dislike.Name;
                string nfield = (!isLike) ? Context.Works.Like.Name : Context.Works.Dislike.Name;
                int like_id = Context.Likes.FindIndex(DB, $"{Context.Likes.Author.Name} = {author} and {Context.Likes.Work.Name} = {id}", out bool _);

                if (like_id > -1)
                {
                    int like = -1;
                    Entity temp = Context.Likes.Get(DB, $"{Context.Likes.ID.Name} = {like_id}");
                    if (temp.Lines.Count > 0)
                        like = To.Int(temp.Lines[0][3]);
                    if (like > -1)
                    {
                        if ((isLike && like == 1) || (!isLike && like == 0))
                        {
                            //nothing
                        }
                        else
                        {
                            Context.Likes.Update(DB, new string[] { Context.Likes.IsLike.Name }, new string[] { (isLike) ? "1" : "0" }, $"{Context.Likes.ID.Name} = {like_id}");
                            Context.Works.Update(DB, new string[] { field, nfield }, new string[] { $"{field} + 1", $"{nfield} - 1" }, $"{Context.Works.ID.Name} = {id}");
                        }
                    }
                }
                else
                {
                    Context.Works.Update(DB, new string[] { field }, new string[] { $"{field} + 1" }, $"{Context.Works.ID.Name} = {id}");
                    Context.Likes.Add(DB, new string[] { id.ToString(), author.ToString(), (isLike) ? "1" : "0" });
                }

            }
            return Error.Accept;
        }

        public ActionResult View(Int32 id, Int32 page = 0)
        {
            int max_in_page = Int32.Parse(WebConfigurationManager.AppSettings["MaxCommentInPage"]);

            this.ViewBag.Action = $"Work/View?id={id}";

            var Items = new List<View.WorkView>();
            var Items2 = new List<View.CommentView>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                this.SetLists(DB);
                this.SetUserInfo(DB);

                this.ViewBag.Count = Context.Comments.Count(DB, $"{Context.Comments.Work.Name} = {id} and {Context.Comments.Answer.Name} = -1");
                this.ViewBag.Pages = (int)Math.Ceiling((double)this.ViewBag.Count / max_in_page);
                this.ViewBag.Page = page;

                List<Models.DataBase.Work> Works = Context.Works.FromEntity(
                    DB.Select($"SELECT * from {Context.Works.Table} where {Context.Works.ID.Name} = {id}")
                    );

                foreach (Models.DataBase.Work Item in Works)
                {
                    Items.Add(new FunCloud.View.WorkView(DB, Item, this.Server.MapPath(Context.Works.FilesPath)));
                }

                List<Models.DataBase.Comment> Comments = Context.Comments.FromEntity(Context.Comments.Get(DB, max_in_page, max_in_page * page, $"{Context.Comments.Work.Name} = {id} and {Context.Comments.Answer.Name} = -1"));
                foreach (Models.DataBase.Comment Item in Comments)
                {
                    Items2.Add(new FunCloud.View.CommentView(DB, Item));
                }

                if (Items.Count > 0)
                {
                    if (Context.ViewWorks.Count(DB, $"{Context.ViewWorks.Work.Name} = {id} and {Context.ViewWorks.User.Name} = {Global.GetUserID(this)} and {Context.ViewWorks.Day.Name} = '{DateTime.Now.ToString("yyyy-MM-dd")}'") < 1)
                    {
                        Context.ViewWorks.Add(DB, new string[] { id.ToString(), Global.GetUserID(this).ToString(), $"'{DateTime.Now.ToString("yyyy-MM-dd")}'" });
                    }
                }

            }

            this.ViewBag.Item = (Items.Count > 0) ? Items[0] : null;

            this.ViewBag.Comments = Items2;

            return this.View();
        }

        [HttpPost]
        public JsonResult FindValue(String Text, Int32 ThisIs)
        {
            var items = new List<Typle<Int32>>();
            if (Text.Length > 0)
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {

                    Entity Items = null;
                    if (ThisIs == 0)
                    {
                        Items = DB.Select($"select {Context.Users.ID.Name}, {Context.Users.Login.Name} from {Context.Users.Table} where {Context.Users.Login.Name} like '%{Text}%'");
                    }
                    else if (ThisIs == 1)
                    {
                        Items = DB.Select($"select {Context.Requests.ID.Name}, {Context.Requests.Title.Name} from {Context.Requests.Table} where {Context.Requests.Title.Name} like '%{Text}%'");
                    }
                    else if (ThisIs == 2)
                    {
                        Text = Text.Trim();
                        Items = (Text.Length > 0)
                            ? DB.Select(Context.Marks.Table, $"top 10 {Context.Marks.ID.Name},{Context.Marks.Title.Name}", $"{Context.Marks.Title.Name} like '%{Text}%'")
                            : DB.Select(Context.Marks.Table, $"top 10 {Context.Marks.ID.Name},{Context.Marks.Title.Name}");
                    }
                    foreach (object[] Line in Items.Lines)
                        items.Add(new Typle<int>(To.String(Line[1]), To.Int(Line[0])));

                }
                
            }
            return this.Json(items);
        }

        [HttpPost]
        public ActionResult Find(Find.FindPublicModel model) 
            => this.Find(model.Text, model.Author, model.Category, model.Fandome, model.Request, model.Serial, model.Popular, model.StartDate, model.EndDate, model.Page);

        [HttpGet]
        public ActionResult Find(String text = "", 
            Int32 author = -1, Int32 category = -1, Int32 fandome = -1,
            Int32 request = -1, Int32 serial = -2, 
            Boolean popular = false, String start_date = "", String end_date = "",
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

            if (request > -1)
                where += $" and {Context.Works.ID.Name} in " +
                    $"(select {Context.WorksOnRequest.Work.Name} from {Context.WorksOnRequest.Table} where {Context.WorksOnRequest.Request.Name} = {request})";
            if(serial == -1)
                where += $" and {Context.Works.ID.Name} not in " +
                    $"(select {Context.WorksInSerial.Work.Name} from {Context.WorksInSerial.Table})";
            else if (serial > -1)
                where += $" and {Context.Works.ID.Name} in " +
                    $"(select {Context.WorksInSerial.Work.Name} from {Context.WorksInSerial.Table} where {Context.WorksInSerial.Serial.Name} = {serial})";

            if (popular)
            {
                start_date = start_date ?? DateTime.Now.AddDays(-10).ToString("yyyy-mm-dd");
                end_date = end_date ?? DateTime.Now.ToString("yyyy-mm-dd");

                where += $"and (select count({Context.ViewWorks.ID.Name}) from {Context.ViewWorks.Table} where {Context.ViewWorks.Table}.{Context.ViewWorks.Work.Name} = {Context.Works.Table}.{Context.Works.ID.Name} and {Context.ViewWorks.Day.Name} between '{start_date}' and '{end_date}') > 0";
            }

            // -- find --

            int max_in_page = Int32.Parse(WebConfigurationManager.AppSettings["MaxWorkInPage"]);

            var action = new StringBuilder("Work/Find?");

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
            if (request > -1)
            {
                action.Append($"{((amp) ? "&" : "")}request={request}");
                amp = true;
            }
            if (serial > -2)
            {
                action.Append($"{((amp) ? "&" : "")}serial={serial}");
                amp = true;
            }

            if (popular)
            {
                action.Append($"{((amp) ? "&" : "")}popular={popular}");
                amp = true;
                action.Append($"{((amp) ? "&" : "")}start_date={start_date}");
                action.Append($"{((amp) ? "&" : "")}end_date={end_date}");
            }

            this.ViewBag.Action = action.ToString();
            
            var Items = new List<View.WorkView>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                Entity Categories = DB.Select($"select id, {Context.Categories.Title.Name} from {Context.Categories.Table} order by {Context.Categories.Title.Name}");
                Entity Fandomes = DB.Select($"select id, {Context.Fandomes.Title.Name} from {Context.Fandomes.Table} order by {Context.Fandomes.Title.Name}");
                Entity Serials = DB.Select($"select id, {Context.Serials.Title.Name} from {Context.Serials.Table} order by {Context.Serials.Title.Name}");

                List<Typle> TCategories = To.Typle(Categories);
                List<Typle> TFandomes = To.Typle(Fandomes);
                List<Typle> TSerials = To.Typle(Serials);

                TCategories.Insert(0, new Typle("-1", "Любой фандом"));
                TFandomes.Insert(0, new Typle("-1", "Любая вселенная"));
                TSerials.Insert(0, new Typle("-1", "Без серии"));
                TSerials.Insert(0, new Typle("-2", "Любая серия"));

                this.ViewBag.Categories = new SelectList(TCategories, "Name", "Value");
                this.ViewBag.Fandomes = new SelectList(TFandomes, "Name", "Value");
                this.ViewBag.Serials = new SelectList(TSerials, "Name", "Value");

                this.SetUserInfo(DB);

                this.ViewBag.Count = Context.Works.Count(DB, where);
                this.ViewBag.Pages = (int)Math.Ceiling((double)this.ViewBag.Count / max_in_page);
                this.ViewBag.Page = page;

                List<Models.DataBase.Work> Works = Context.Works.FromEntity(Context.Works.Get(DB, max_in_page, max_in_page * page, where));

                foreach (Models.DataBase.Work Item in Works)
                    Items.Add(new FunCloud.View.WorkView(DB, Item, this.Server.MapPath(Context.Works.FilesPath)));

            }

            this.ViewBag.Items = Items;

            // -- end --

            return this.View(new Find.FindPublicModel() { Text = text, Author = author, Category = category, Fandome = fandome, Request = request, Serial = serial, Page = page });

        }

        public ActionResult Read(Int32 WorkID, Int32 page = 0)
        {
            int max_in_page = 20;
            int count = 0;
            int lost = 0;
            int allcount = 0;

            this.ViewBag.Action = $"Work/Read?WorkID={WorkID}";

            var Items = new List<String>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {

                this.SetUserInfo(DB);

                Models.DataBase.Work Work = Context.Works.Find(DB, $"{Context.Works.ID.Name} = {WorkID}", out _);

                this.ViewBag.Item = new View.WorkView(DB, Work, this.Server.MapPath(Context.Works.FilesPath));

            }

            string filepath = this.Server.MapPath(Context.Works.FilesPath + WorkID.ToString() + "/");
            if (Directory.Exists(filepath))
            {
                string[] files = Directory.GetFiles(filepath, Context.Works.MainFileName + ".*");

                if (files.Length > 0)
                {

                    if (System.IO.File.Exists(files[0]))
                    {
                        string ext = System.IO.Path.GetExtension(files[0]);
                        switch (ext)
                        {
                            case ".doc":
                            case ".docx":
                                var word = new Microsoft.Office.Interop.Word.Application();
                                object miss = System.Reflection.Missing.Value;
                                object path = files[0];
                                object readOnly = true;
                                Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
                                allcount = docs.Paragraphs.Count;
                                for (int i = 0; i < max_in_page && (max_in_page * page) + i < allcount; i++)
                                    Items.Add(docs.Paragraphs[(max_in_page * page) + i + 1].Range.Text.ToString());
                                docs.Close();
                                word.Quit();
                                break;
                            case ".pdf":
                                max_in_page = 1;
                                var strategy = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
                                using (var pdf = new iTextSharp.text.pdf.PdfReader(files[0]))
                                {
                                    allcount = pdf.NumberOfPages;
                                    //for (int i = 0; i < max_in_page && (max_in_page * page) + i < allcount; i++)
                                    string text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdf, page + 1, strategy);
                                    Items = DataBaseConnector.Ext.From.String(text, '\n');
                                }
                                break;
                            case ".txt":
                                using (var sr = new StreamReader(files[0]))
                                {
                                    while (lost < max_in_page * page && !sr.EndOfStream)
                                    {
                                        sr.ReadLine();
                                        lost++;
                                        allcount++;
                                    }
                                    while (!sr.EndOfStream && count < max_in_page)
                                    {
                                        Items.Add(sr.ReadLine());
                                        count++;
                                        allcount++;
                                    }
                                    while (!sr.EndOfStream)
                                    {
                                        sr.ReadLine();
                                        allcount++;
                                    }
                                }
                                break;
                            default:
                                Items.Add($"Файл не может быть открыт т.к. расширение (.{ext}) не поддерживается в настоящий момент!");
                                break;
                        }

                    }

                }
                else
                {
                    Items.Add("Файл не найден!");
                }
            } else
            {
                Items.Add("Папка для хранения файлов не найдена!");
            }
            this.ViewBag.Pages = (int)Math.Ceiling((double)allcount / max_in_page);
            this.ViewBag.Page = page;

            this.ViewBag.Items = Items;
            return this.View();
        }

        public ActionResult Random()
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Int32 Count = Context.Works.Count(DB);
                Models.DataBase.Work Item = null;
                do
                {
                    int rand = new System.Random().Next(0, Count);
                    Item = Context.Works.Find(DB, $"{Context.Works.ID.Name} = {rand}", out _);
                } while (Item == null);
                return this.RedirectToAction($"/View/{Item.ID.Value}");

            }
        }

        public ActionResult FindAnalog(Int32 WorkID, Int32 page = 0)
        {
            int max_in_page = Int32.Parse(WebConfigurationManager.AppSettings["MaxWorkInPage"]);

            var Items = new List<View.WorkView>();
            this.ViewBag.Action = $"/Work/FindAnalog?WorkID={WorkID}";

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetUserInfo(DB);
                this.ViewBag.Count = 0;

                Models.DataBase.Work Item = Context.Works.Find(DB, $"{Context.Works.ID.Name} = {WorkID}", out _);
                if(Item != null)
                {
                    Models.DataBase.WorksInSerial tmp = Context.WorksInSerial.Find(DB, $"{Context.WorksInList.Work.Name} = {Item.ID}", out _);
                    int ItemSerial = (tmp != null) ? tmp.ID.Value : -1;

                    this.ViewBag.Count = Context.Works.Count(DB, $"{Context.Works.Author.Name} = {Item.Author.Value} or " +
                        $"{Context.Works.Category.Name} = {Item.Category.Value} or " +
                        $"{Context.Works.Fandome.Name} = {Item.Fandome.Value} or " +
                        $"{Context.Works.ID.Name} in (select {Context.WorksInSerial.Work.Name} from {Context.WorksInSerial.Table} where {Context.WorksInSerial.ID.Name} = {ItemSerial})"
                        );

                    // Выбираем массив Паретто
                    List<Models.DataBase.Work> Paretto = Context.Works.FromEntity(Context.Works.Get(DB,
                        $"{Context.Works.Author.Name} = {Item.Author.Value} or " +
                        $"{Context.Works.Category.Name} = {Item.Category.Value} or " +
                        $"{Context.Works.Fandome.Name} = {Item.Fandome.Value} or " +
                        $"{Context.Works.ID.Name} in (select {Context.WorksInSerial.Work.Name} from {Context.WorksInSerial.Table} where {Context.WorksInSerial.ID.Name} = {ItemSerial})"
                        ));

                    List<string> BasicMarks = From.String(Item.Marks.Value, ",");

                    var items = new List<Helpers.Work>();
                    foreach (Models.DataBase.Work item in Paretto)
                        items.Add(new Helpers.Work(item, BasicMarks));

                    items = Helpers.KemeniSnell.GetAnalog(new Helpers.Work(Item, BasicMarks), items);

                    foreach(Helpers.Work item in items)
                        Items.Insert(Items.Count, new View.WorkView(DB, Paretto.Find(x => x.ID.Value == item.ID), ""));
                }
                this.ViewBag.Pages = (int)Math.Ceiling((double)this.ViewBag.Count / max_in_page);
                this.ViewBag.Page = page;
            }

            this.ViewBag.Items = Items;

            return this.View();
        }

    }
}