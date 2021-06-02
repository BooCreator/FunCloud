using System;
using System.ComponentModel.DataAnnotations;

namespace FunCloud.Request
{
    public class RequestPublicModel
    {
        public RequestPublicModel() { }

        public RequestPublicModel(Models.DataBase.Request Item)
        {
            this.ID = Item.ID.Value;
            this.Title = Item.Title.Value;
            this.Category = Item.Category.Value;
            this.Fandome = Item.Fandome.Value;
            this.Author = Item.Author.Value;
            this.Description = Item.Description.Value;
        }

        public Int32 ID { get; set; }
        [Required]
        public String Title { get; set; }
        public Int32 Category { get; set; }
        public Int32 Fandome { get; set; }
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }
        public Int32 Author { get; set; }
        public String[] ToAttributes()
        {
            return new string[] {
                $"'{this.Title.Replace("'", "''")}'",
                this.Category.ToString(),
                this.Fandome.ToString(),
                $"'{this.Description.Replace("'", "''")}'",
                $"{this.Author}",
                $"'{DateTime.Now.ToString("yyyy-MM-dd")}'"
            };
        }

    }
}