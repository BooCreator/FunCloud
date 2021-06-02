using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.View
{
    public class CommentView
    {

        public CommentView(DataBaseExtended DB, Models.DataBase.Comment Comment)
        {
            this.ID = Comment.ID.Value;
            this.Work = Comment.Work.Value;

            List<Models.DataBase.Comment> Comments = Context.Comments.FromEntity(
                DB.Select(
                    $"select * from {Context.Comments.Table} where {Context.Comments.Work.Name} = {Comment.Work.Value} and {Context.Comments.Answer.Name} = {Comment.ID.Value}")
                );
            foreach (Models.DataBase.Comment Item in Comments)
                this.Answer.Add(new FunCloud.View.CommentView(DB, Item));

            Entity temp = DB.Select(
                $"select {Context.Users.Login.Name} from {Context.Users.Table} where id = {Comment.Author.Value}");
            if (temp.Lines.Count > 0)
            {
                this.Author.Name = To.String(temp.Lines[0][0]);
                this.Author.Value = Comment.Author.Value;
            }

            this.Pub_date = Comment.Pub_date.Value;
            this.Text = Comment.Text.Value;
        }

        public Int32 ID { get; set; }
        public Int32 Work { get; set; }
        public List<CommentView> Answer { get; set; } = new List<CommentView>();
        public Typle<Int32> Author { get; set; } = new Typle<int>("");
        public String Pub_date { get; set; }
        public String Text { get; set; }

    }
}