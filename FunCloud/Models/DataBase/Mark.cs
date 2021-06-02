using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Mark : Basic<Mark>
    {
        public override string Table => "[Mark]";

        public override string[] Fields => new string[] { "[title]"};

        public override string[] Types => new string[] { "varchar(255)" };

        public Mark()
        {
            this.ID = new Typle<int>("[id]", -1);
            this.Title = new Typle<string>(this.Fields[0]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<String> Title { get; set; }

        protected override Mark FromObject(object[] line)
            => new Mark()
            {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Title = new Typle<string>(this.Fields[0], To.String(line[1])),
            };

    }
}