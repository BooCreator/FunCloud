using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Subscribe : Basic<Subscribe>
    {

        public override string Table => "[Subscribe]";

        public override string[] Fields => new string[] { "[user1]", "[user2]" };

        public override string[] Types => new string[] { "int", "int" };

        public Subscribe()
        {
            this.ID = new Typle<int>("[id]", -1);
            this.Follower = new Typle<int>(this.Fields[0]);
            this.User = new Typle<int>(this.Fields[1]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<Int32> Follower { get; set; }
        public Typle<Int32> User { get; set; }

        protected override Subscribe FromObject(object[] line)
            => new Subscribe()
            {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Follower = new Typle<int>(this.Fields[0], To.Int(line[1])),
                User = new Typle<int>(this.Fields[1], To.Int(line[2])),
            };

    }
}