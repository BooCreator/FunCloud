using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class List : Basic<List>
    {
        public override string Table => "[List]";

        public override string[] Fields => new string[] { "[title]", "[author]" };

        public override string[] Types => new string[] { "varchar(255)", "int" };

        public List()
        {
            this.ID = new Typle<int>("[id]", -1);
            this.Title = new Typle<string>(this.Fields[0]);
            this.Author = new Typle<int>(this.Fields[1]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<String> Title { get; set; }
        public Typle<Int32> Author { get; set; }

        protected override List FromObject(object[] line)
            => new List()
            {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Title = new Typle<string>(this.Fields[0], To.String(line[1])),
                Author = new Typle<int>(this.Fields[1], To.Int(line[2])),
            };
    }
}