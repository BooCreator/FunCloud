using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class WorkOnRequest : Basic<WorkOnRequest>
    {
        public override string Table => "[WorkOnRequest]";

        public override string[] Fields => new string[] { "[work]", "[request]" };

        public override string[] Types => new string[] { "int", "int" };

        public WorkOnRequest()
        {
            this.ID = new Typle<int>("[id]");
            this.Work = new Typle<int>(this.Fields[0]);
            this.Request = new Typle<int>(this.Fields[1]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<Int32> Work { get; set; }
        public Typle<Int32> Request { get; set; }

        protected override WorkOnRequest FromObject(object[] line)
            => new WorkOnRequest()
            {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Work = new Typle<int>(this.Fields[0], To.Int(line[1])),
                Request = new Typle<int>(this.Fields[1], To.Int(line[2])),
            };
    }
}