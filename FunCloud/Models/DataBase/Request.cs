using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Request : Basic<Request>
    {
        public override String Table => "[Request]";
        public override String[] Fields => new String[] { "[title]", "[category]", "[fandome]", "[description]", "[author]", "[pub_date]" };
        public override String[] Types => new String[] { "varchar(32)", "int", "int", "varchar(256)", "int", "varchar(21)" };

        public Request()
        {
            this.ID = new Typle<int>("[id]");
            this.Title = new Typle<string>(this.Fields[0]);
            this.Category = new Typle<int>(this.Fields[1]);
            this.Fandome = new Typle<int>(this.Fields[2]);
            this.Description = new Typle<string>(this.Fields[3]);
            this.Author = new Typle<int>(this.Fields[4]);
            this.Pub_date = new Typle<string>(this.Fields[5]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<String> Title { get; set; }
        public Typle<Int32> Category { get; set; }
        public Typle<Int32> Fandome { get; set; }
        public Typle<String> Description { get; set; }
        public Typle<Int32> Author { get; set; }
        public Typle<String> Pub_date { get; set; }

        protected override Request FromObject(object[] line) =>
            new Request() {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Title = new Typle<string>(this.Fields[0], To.String(line[1])),
                Category = new Typle<int>(this.Fields[1], To.Int(line[2])),
                Fandome = new Typle<int>(this.Fields[2], To.Int(line[3])),
                Description = new Typle<string>(this.Fields[3], To.String(line[4])),
                Author = new Typle<int>(this.Fields[4], To.Int(line[5])),
                Pub_date = new Typle<string>(this.Fields[5], To.String(line[6])),
            };

    }
}