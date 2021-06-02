using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class WorksInList : Basic<WorksInList>
    {
        public override string Table => "[WorksInList]";

        public override string[] Fields => new string[] { "[work]", "[list]" };

        public override string[] Types => new string[] { "int", "int" };

        public WorksInList()
        {
            this.ID = new Typle<int>("[id]", -1);
            this.Work = new Typle<int>(this.Fields[0]);
            this.List = new Typle<int>(this.Fields[1]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<Int32> Work { get; set; }
        public Typle<Int32> List { get; set; }

        protected override WorksInList FromObject(object[] line)
            => new WorksInList()
            {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Work = new Typle<int>(this.Fields[0], To.Int(line[1])),
                List = new Typle<int>(this.Fields[1], To.Int(line[2])),
            };
    }
}