using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class ViewWork : Basic<ViewWork>
    {
        public override string Table => "[ViewWork]";

        public override string[] Fields => new string[] { "[work]", "[user]", "[date]" };

        public override string[] Types => new string[] { "int", "int", "date" };

        public ViewWork()
        {
            this.ID = new Typle<int>("[id]");
            this.Work = new Typle<int>(this.Fields[0]);
            this.User = new Typle<int>(this.Fields[1]);
            this.Day = new Typle<string>(this.Fields[2]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<Int32> Work { get; set; }
        public Typle<Int32> User { get; set; }
        public Typle<String> Day { get; set; }

        protected override ViewWork FromObject(object[] line)
            => new ViewWork()
            {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Work = new Typle<int>(this.Fields[0], To.Int(line[1])),
                User = new Typle<int>(this.Fields[1], To.Int(line[2])),
                Day = new Typle<string>(this.Fields[2], To.Date(line[3]).ToShortDateString())
            };

    }
}