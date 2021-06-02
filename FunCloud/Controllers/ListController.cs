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
    public class ListController : Controller
    {
        
        [HttpPost]
        public JsonResult Add(Int32 Author, String Title)
        {
            if (Author == Global.GetUserID(this))
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    return Context.Lists.Add(DB, new string[] { $"'{Title}'", Author.ToString() }) 
                        ? this.Json(Error.Accept) 
                        : this.Json(Error.Unknown);
                }
            }
            return this.Json(Error.NotAccess);
        }

        [HttpPost]
        public JsonResult Edit(Int32 id, String Title)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                return Context.Lists.Update(DB, Context.Lists.Title.Name, $"'{Title}'", $"{Context.Lists.ID.Name} = {id} and {Context.Lists.Author.Name} = {Global.GetUserID(this)}")
                    ? this.Json(Error.Accept)
                    : this.Json(Error.Unknown);
            }
        }
       
        [HttpPost]
        public JsonResult Remove(Int32 id)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (Context.Lists.Remove(DB, $"{Context.Lists.ID.Name} = {id} and {Context.Lists.Author.Name} = {Global.GetUserID(this)}")){
                    Context.WorksInList.Remove(DB, $"{Context.WorksInList.List.Name} = {id}");
                    return this.Json(Error.Accept);
                } else
                    return this.Json(Error.Unknown);
            }
        }
        
        [HttpPost]
        public JsonResult AddWork(Int32 id, Int32 WorkID, Int32 Author)
        {
            if (Author == Global.GetUserID(this))
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    if (Context.WorksInList.Count(DB, $"{Context.WorksInList.Work.Name} = {WorkID} and {Context.WorksInList.List.Name} = {id}") < 1)
                    {
                        return Context.WorksInList.Add(DB, new string[] { $"'{WorkID}'", id.ToString() })
                            ? this.Json(Error.Accept)
                            : this.Json(Error.Unknown);
                    } else
                        return this.Json(Error.Exists);
                }
            }
            return this.Json(Error.NotAccess);
        }
        
        [HttpPost]
        public JsonResult RemoveWork(Int32 id, Int32 WorkID, Int32 Author)
        {
            if (Author == Global.GetUserID(this))
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    return Context.WorksInList.Remove(DB, $"{Context.WorksInList.Work.Name} = {WorkID} and {Context.WorksInList.List.Name} = {id}")
                        ? this.Json(Error.Accept)
                        : this.Json(Error.Unknown);
                }
            }
            return this.Json(Error.NotAccess);
        }

    }
}