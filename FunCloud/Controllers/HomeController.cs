using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Controllers
{
    public class HomeController : Controller
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

        public ActionResult Index()
        {
            var Fandoms = new List<Typle<int>>();
            var Universes = new List<Typle<int>>();
            var Works = new List<Typle<int>>();
            var Authors = new List<Typle<int>>();

            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                this.SetUserInfo(DB);

                foreach(Models.DataBase.Category item in Context.Categories.FromEntity(Context.Categories.Get(DB)))
                    Fandoms.Add(new Typle<int>(item.Title.Value, item.ID.Value));

                foreach (Models.DataBase.Fandome item in Context.Fandomes.FromEntity(Context.Fandomes.Get(DB)))
                    Universes.Add(new Typle<int>(item.Title.Value, item.ID.Value));

                Entity table = DB.Select($"{Context.Works.Table} left join {Context.ViewWorks.Table} on {Context.Works.Table}.{Context.Works.ID.Name} = {Context.ViewWorks.Table}.{Context.ViewWorks.Work.Name}" +
                    $" group by {Context.Works.Table}.{Context.Works.ID.Name}, {Context.Works.Table}.{Context.Works.Title.Name} order by count desc, {Context.Works.Title.Name}", 
                    $"top 10 {Context.Works.Table}.{Context.Works.ID.Name}, {Context.Works.Table}.{Context.Works.Title.Name}," +
                    $" COUNT({Context.ViewWorks.Table}.{Context.ViewWorks.ID.Name}) as count ");

                foreach (object[] line in table.Lines)
                    Works.Add(new Typle<int>(To.String(line[1]), To.Int(line[0])));

                table = DB.Select($"{Context.Users.Table} left join {Context.Subscribe.Table} on {Context.Users.Table}.{Context.Users.ID.Name} = {Context.Subscribe.Table}.{Context.Subscribe.User.Name}" +
                    $" group by {Context.Users.Table}.{Context.Users.ID.Name}, {Context.Users.Table}.{Context.Users.Login.Name} order by count desc, {Context.Users.Login.Name}",
                    $"top 10 {Context.Users.Table}.{Context.Users.ID.Name}, {Context.Users.Table}.{Context.Users.Login.Name}," +
                    $" COUNT({Context.Subscribe.Table}.{Context.Subscribe.ID.Name}) as count"
                    );

                foreach (object[] line in table.Lines)
                    Authors.Add(new Typle<int>(To.String(line[1]), To.Int(line[0])));
            }

            this.ViewBag.Fandoms = Fandoms;
            this.ViewBag.Universes = Universes;
            this.ViewBag.Works = Works;
            this.ViewBag.Authors = Authors;

            return this.View();
        }
        
    }

}