using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.View
{
    public class RequestView
    {
        public RequestView(DataBaseExtended DB, Models.DataBase.Request Request)
        {
            this.ID = Request.ID.Value;
            this.Title = Request.Title.Value;

            Entity temp = DB.Select(
                $"select {Context.Categories.Title.Name} from {Context.Categories.Table} where id = {Request.Category.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Category.Name = To.String(temp.Lines[0][0]);
                this.Category.Value = Request.Category.Value;
            }

            temp = DB.Select(
                $"select {Context.Fandomes.Title.Name} from {Context.Fandomes.Table} where id = {Request.Fandome.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Fandome.Name = To.String(temp.Lines[0][0]);
                this.Fandome.Value = Request.Fandome.Value;
            }

            this.Description = Request.Description.Value;

            temp = DB.Select(
                $"select {Context.Users.Login.Name} from {Context.Users.Table} where id = {Request.Author.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Author.Name = To.String(temp.Lines[0][0]);
                this.Author.Value = Request.Author.Value;
            }

        }

        public Int32 ID { get; set; }
        public String Title { get; set; }
        public Typle<Int32> Category { get; set; } = new Typle<int>("", -1);
        public Typle<Int32> Fandome { get; set; } = new Typle<int>("", -1);
        public String Description { get; set; }
        public Typle<Int32> Author { get; set; } = new Typle<int>("", -1);

    }

}