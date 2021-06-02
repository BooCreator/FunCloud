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

namespace FunCloud.Controllers
{
    public class SerialController : Controller
    {
        
        [HttpPost]
        public JsonResult Add(Int32 Author, String Title)
        {
            if(Author == Global.GetUserID(this))
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    if (Context.Serials.Count(DB, $"{Context.Serials.Title.Name} like '{Title}' and {Context.Serials.Author.Name} = {Author}") < 1) {
                        return Context.Serials.Add(DB, new string[] { $"'{Title}'", Author.ToString() })
                            ? this.Json(Error.Accept)
                            : this.Json(Error.Unknown);
                    } else
                        return this.Json(Error.Exists);
                }
            }
            return this.Json(Error.NotAccess);
        }

        [HttpPost]
        public JsonResult Edit(Int32 id, String Title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                return Context.Serials.Update(DB, Context.Serials.Title.Name, $"'{Title}'", $"{Context.Serials.ID.Name} = {id} and {Context.Serials.Author.Name} = {Global.GetUserID(this)}")
                    ? this.Json(Error.Accept)
                    : this.Json(Error.Unknown);
            }
        }

        [HttpPost]
        public JsonResult Remove(Int32 id)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (Context.WorksInSerial.Count(DB, $"{Context.WorksInSerial.Serial.Name} = {id}") == 0)
                    Context.Serials.Remove(DB, $"{Context.Serials.ID.Name} = {id} and {Context.Serials.Author.Name} = {Global.GetUserID(this)}");
                else
                    Context.Serials.Update(DB, Context.Serials.Author.Name, "-1", $"{Context.Serials.ID.Name} = {id} and {Context.Serials.Author.Name} = {Global.GetUserID(this)}");
            }
            return this.Json(Error.Accept);
        }

    }
}