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
    public class SubscribeController : Controller
    {
        [HttpPost]
        public JsonResult On(Int32 id)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (Context.Subscribe.Count(DB, $"{Context.Subscribe.Follower.Name} = {Global.GetUserID(this)} and {Context.Subscribe.User.Name} = {id}") < 1)
                {
                    return Context.Subscribe.Add(DB, new string[] { Global.GetUserID(this).ToString(), id.ToString() })
                        ? this.Json(Error.Accept)
                        : this.Json(Error.Unknown);
                } else 
                    return this.Json(Error.Exists);
            }
        }

        [HttpPost]
        public JsonResult Off(Int32 id)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                return Context.Subscribe.Remove(DB, $"{Context.Subscribe.Follower.Name} = {Global.GetUserID(this)} and {Context.Subscribe.User.Name} = {id}")
                    ? this.Json(Error.Accept)
                    : this.Json(Error.Unknown);
            }
        }

    }
}