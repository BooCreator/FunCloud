using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.View
{
    public class WorkView
    {
        public WorkView(DataBaseExtended DB, Models.DataBase.Work Work, String FilePath)
        {
            this.ID = Work.ID.Value;
            this.Title = Work.Title.Value;

            Entity temp = DB.Select(
                $"select {Context.Categories.Title.Name} from {Context.Categories.Table} where id = {Work.Category.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Category.Name = To.String(temp.Lines[0][0]);
                this.Category.Value = Work.Category.Value;
            }

            temp = DB.Select(
                $"select {Context.Fandomes.Title.Name} from {Context.Fandomes.Table} where id = {Work.Fandome.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Fandome.Name = To.String(temp.Lines[0][0]);
                this.Fandome.Value = Work.Fandome.Value;
            }

            this.Description = Work.Description.Value;

            temp = DB.Select(
                $"select {Context.Users.Login.Name} from {Context.Users.Table} where id = {Work.Author.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Author.Name = To.String(temp.Lines[0][0]);
                this.Author.Value = Work.Author.Value;
            }

            if (Work.State.Value == 2 && DateTime.TryParse(Work.Upd_date.Value, out DateTime result))
            {
                if (result.AddDays(4) < DateTime.Now)
                {
                    Context.Works.Update(DB, Context.Works.State.Name, "3", $"{Context.Works.ID.Name} = {Work.ID.Value}");
                    Work.State.Value = 3;
                }
            }

            temp = DB.Select(
                $"select {Context.States.Title.Name} from {Context.States.Table} where id = {Work.State.Value}");
            if (temp.Lines.Count > 0)
                this.State = To.String(temp.Lines[0][0]);

            this.Pub_date = Work.Pub_date.Value;

            this.Like = Work.Like.Value;
            this.Dislike = Work.Dislike.Value;

            this.FilePath = FilePath;

            this.Files = From.String(Work.Files.Value, ",");

            temp = DB.Select(
                $"select {Context.WorksOnRequest.Table}.{Context.WorksOnRequest.Request.Name}, {Context.Requests.Table}.{Context.Requests.Title.Name} from {Context.WorksOnRequest.Table} left join {Context.Requests.Table} on {Context.WorksOnRequest.Table}.{Context.WorksOnRequest.Request.Name} = {Context.Requests.Table}.{Context.Requests.ID.Name} where {Context.WorksOnRequest.Work.Name} = {Work.ID.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Request.Name = To.String(temp.Lines[0][1]);
                this.Request.Value = To.Int(temp.Lines[0][0]);
            }

            temp = DB.Select(
                $"select {Context.WorksInSerial.Table}.{Context.WorksInSerial.Serial.Name}, {Context.Serials.Table}.{Context.Serials.Title.Name} from {Context.WorksInSerial.Table} left join {Context.Serials.Table} on {Context.WorksInSerial.Table}.{Context.WorksInSerial.Serial.Name} = {Context.Serials.Table}.{Context.Serials.ID.Name} where {Context.WorksInSerial.Work.Name} = {Work.ID.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Serial.Value = To.Int(temp.Lines[0][0]);
                this.Serial.Name = (this.Serial.Value > -1) ? To.String(temp.Lines[0][1]) : "Без серии";
                
            }

            this.Marks.Clear();
            var marks = From.String(Work.Marks.Value, ",");
            foreach(string mark in marks)
            {
                Models.DataBase.Mark item = Context.Marks.Find(DB, $"{Context.Marks.Title.Name} like '{mark}'", out _);
                if(item != null)
                    this.Marks.Add(new Typle<int>(mark, item.ID.Value));
            }
        }

        public Int32 ID { get; set; }
        public String Title { get; set; }
        public Typle<Int32> Category { get; set; } = new Typle<Int32>("", -1);
        public Typle<Int32> Fandome { get; set; } = new Typle<Int32>("", -1);
        public String Description { get; set; }
        public Typle<Int32> Author { get; set; } = new Typle<Int32>("", -1);
        public String State { get; set; }
        public String Pub_date { get; set; }
        public Int32 Like { get; set; }
        public Int32 Dislike { get; set; }
        public String FilePath { get; set; }
        public List<String> Files { get; set; } = new List<String>();
        public Typle<Int32> Request { get; set; } = new Typle<int>("", -1);
        public Typle<Int32> Serial { get; set; } = new Typle<int>("Без серии", -1);
        public List<Typle<Int32>> Marks { get; set; } = new List<Typle<Int32>>();

    }

}