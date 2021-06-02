using System;
using System.ComponentModel.DataAnnotations;

namespace FunCloud.Comment
{
    public class CommentPublicModel
    {
        public CommentPublicModel() { }

        public CommentPublicModel(Models.DataBase.Comment Item)
        {
            this.ID = Item.ID.Value;
            this.Work = Item.Work.Value;
            this.Author = Item.Author.Value;
            this.Answer = Item.Answer.Value;
            this.Text = Item.Text.Value;
            this.Pub_date = Item.Pub_date.Value;
        }

        public Int32 ID { get; set; }
        public Int32 Work { get; set; }
        public Int32 Author { get; set; }
        public Int32 Answer { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public String Text { get; set; }
        public String Pub_date { get; set; }
        
        public String[] ToAttributes()
        {
            return new string[] {
                this.Work.ToString(),
                this.Author.ToString(),
                this.Answer.ToString(),
                $"'{DateTime.Now.ToShortDateString()}'",
                $"'{this.Text}'"
            };
        }

    }
}