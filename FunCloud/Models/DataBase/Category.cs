using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Category : Basic<Category>
    {
        public override String Table => "[Category]";
        public override String[] Fields => new String[] { "[title]" };
        public override String[] Types => new String[] { "varchar(32)" };

        public Category()
        {
            this.ID = new Typle<int>("[id]");
            this.Title = new Typle<string>(this.Fields[0]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<String> Title { get; set; }

        protected override Category FromObject(object[] line) =>
            new Category() {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Title = new Typle<string>(this.Fields[0], To.String(line[1])),
            };

    }
}