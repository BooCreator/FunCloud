using System;
using System.ComponentModel.DataAnnotations;

namespace FunCloud.Work
{

    public class WorkPublicModel
    {
        public WorkPublicModel() { }

        public WorkPublicModel(Models.DataBase.Work Item)
        {
            this.ID = Item.ID.Value;
            this.Title = Item.Title.Value;
            this.Category = Item.Category.Value;
            this.Fandome = Item.Fandome.Value;
            this.Description = Item.Description.Value;
            this.Author = Item.Author.Value;
            this.State = Item.State.Value;
            this.Files = Item.Files.Value;
            this.Marks = Item.Marks.Value;
        }

        public Int32 ID { get; set; }
        [Required]
        public String Title { get; set; }
        public Int32 Category { get; set; }
        public Int32 Fandome { get; set; }
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }
        public Int32 Author { get; set; }
        [Required]
        public Int32 State { get; set; }
        public String Files { get; set; }
        public Int32 Request { get; set; }
        public Int32 Serial { get; set; }
        public String Marks { get; set; }
        public String[] ToAttributes()
        {
            return new string[] { 
                $"'{this.Title.Replace("'", "''")}'", 
                this.Category.ToString(),
                this.Fandome.ToString(),
                $"'{this.Description.Replace("'", "''")}'", 
                this.Author.ToString(), 
                this.State.ToString(),
                $"'{DateTime.Now.ToString("yyyy-MM-dd")}'",
                $"'{DateTime.Now.ToString("yyyy-MM-dd")}'",
                $"'{this.Files}'",
                "0", "0",
                $"'{this.Marks?.Replace(", ", ",")}'"
            };
        }

    }

    

}