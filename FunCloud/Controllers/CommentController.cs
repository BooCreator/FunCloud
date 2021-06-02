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
    public class CommentController : Controller
    {

        [HttpPost]
        public JsonResult Public(Comment.CommentPublicModel model)
        {
            if (model != null)
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    if (Global.GetUserID(this) > -1)
                    {
                        Context.Comments.Add(DB, model.ToAttributes());
                        return this.Json(Error.Accept);
                    }
                    else
                        return this.Json(Error.NotAccess);
                }
            return this.Json(Error.IsEmpty);
        }
        [HttpPost]
        public JsonResult Edit(Comment.CommentPublicModel model)
        {
            string result = Error.IsEmpty;
            if (model != null)
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    if (Global.GetUserID(this) > -1)
                    {
                        Context.Comments.Update(DB, Context.Comments.Text.Name, $"'{model.Text}'", $"{Context.Comments.ID.Name} = {model.ID}");
                        return this.Json(Error.Accept);
                    }
                    else 
                        return this.Json(Error.NotAccess);
                }
            return this.Json(Error.IsEmpty);
        }
        [HttpPost]
        public JsonResult Remove(Int32 id)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                if (Context.Comments.Find(DB, $"{Context.Comments.ID.Name} = {id}", out bool _)?.Author.Value == Global.GetUserID(this))
                {
                    return Context.Comments.Remove(DB, $"{Context.Comments.ID.Name} = {id} or {Context.Comments.Answer.Name} = {id}")
                        ? this.Json(Error.Accept)
                        : this.Json(Error.Unknown);
                }
                else
                    return this.Json(Error.IsEmpty);
            }
        }

    }
}