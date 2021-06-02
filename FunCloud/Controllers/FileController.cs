using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DataBaseConnector;
using DataBaseConnector.Ext;
using System.IO.Compression;

namespace FunCloud.Controllers
{
    public class FileController : Controller
    {

        public static String Temp_Dir = "~/App_Data/Temp/";

        public static String SetMIMEType(String FileName)
        {
            switch (FileName.Substring(FileName.LastIndexOf(".")))
            {
                case "pdf": return "application/pdf";
                case "doc": return "application/msword";
                case "docx": return "application/msword";
                case "ogg": return "audio/ogg";
                case "zip": return "application/zip";
                case "rar": return "application/zip";
                case "mp3": return "audio/mpeg";
                case "mp4": return "video/mp4";
                case "avi": return "video/mp4";
                case "jpg": return "image/jpeg";
                case "png": return "image/png";
                case "gif": return "image/gif";
                default: 
                    return "multipart/form-data";
            }
        }

        public FileResult Download(Int32 WorkID, String FileName)
        {
            string file_path = this.Server.MapPath(Context.Works.FilesPath + WorkID.ToString() + "/" + FileName);
            if (System.IO.File.Exists(file_path))
            {
                string file_type = SetMIMEType(FileName);
                string file_name = FileName;
                return this.File(file_path, file_type, file_name);
            }
            else return null;
        }
        public FileResult DownloadAll(Int32 WorkID)
        {
            
            string file_path = this.Server.MapPath(Context.Works.FilesPath + WorkID.ToString());

            string temp_path = this.Server.MapPath(Temp_Dir + WorkID.ToString());
            string zip_path = temp_path + "\\archive.zip";

            if (!Directory.Exists(temp_path))
                Directory.CreateDirectory(temp_path);

            if (System.IO.File.Exists(zip_path))
                System.IO.File.Delete(zip_path);

            while (System.IO.File.Exists(zip_path)){ /* -- wait deleting file -- */ }

            ZipFile.CreateFromDirectory(file_path, zip_path);

            if (System.IO.File.Exists(zip_path))
            {
                string file_type = SetMIMEType("archive.zip");
                string file_name = "archive.zip";
                return this.File(zip_path, file_type, file_name);
            }
            return null;
        }

        [HttpPost]
        public JsonResult Edit(Int32 WorkID, HttpPostedFileBase basefile)
        {
            if (basefile != null)
            {
                basefile.SaveAs(this.Server.MapPath(Context.Works.FilesPath + WorkID + "/" + Context.Works.MainFileName));
                this.ViewBag.isFileLoad = true;
                return this.Json(Error.Accept);
            }
            return this.Json(Error.IsEmpty);
        }
        [HttpPost]
        public JsonResult Add(Int32 WorkID, HttpPostedFileBase file)
        {
            if (file != null)
            {
                using (var DB = new DataBaseExtended(Global.ConnectionString))
                {
                    Models.DataBase.Work work = Context.Works.Find(DB, $"{Context.Works.ID.Name} = {WorkID}", out _);

                    if (work.Author.Value == Global.GetUserID(this))
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string path = Context.Works.FilesPath + WorkID + "/" + fileName;
                        if (!System.IO.File.Exists(this.Server.MapPath(path)))
                        {
                            // сохраняем файл в папку Files в проекте
                            file.SaveAs(this.Server.MapPath(path));
                            Context.Works.Update(DB, Context.Works.Files.Name, $"{Context.Works.Files.Name} + '{fileName},'", $"{Context.Works.ID.Name} = {WorkID}");
                        } else
                            return new JsonResult() { Data = Error.Exists };
                    }
                    else
                        return new JsonResult() { Data = Error.NotAccess };
                }
                return new JsonResult() { Data = Error.Accept };
            }
            return new JsonResult() { Data = Error.IsEmpty };
        }
        [HttpPost]
        public JsonResult Remove(Int32 WorkID, String FileName)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Models.DataBase.Work work = Context.Works.Find(DB, $"{Context.Works.ID.Name} = {WorkID}", out _);

                if (work.Author.Value == Global.GetUserID(this))
                {
                    work.Files.Value = work.Files.Value.Replace(FileName, "").Replace(",,", ",").Trim(new char[] { ',' });
                    Context.Works.Update(DB, Context.Works.Files.Name, $"'{work.Files.Value}'", $"{Context.Works.ID.Name} = {WorkID}");

                    string filepath = this.Server.MapPath(work.FilesPath + work.ID.Value + "/" + FileName);
                    if (System.IO.File.Exists(filepath))
                        System.IO.File.Delete(filepath);
                }
                else 
                    return this.Json(Error.NotAccess);
            }
            return this.Json(Error.Accept);
        }


    }
}