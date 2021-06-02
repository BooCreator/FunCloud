using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class WorksInSerial : Basic<WorksInSerial>
    {
        public override string Table => "[WorksInSerial]";

        public override string[] Fields => new string[] { "[work]", "[serial]" };

        public override string[] Types => new string[] { "int", "int" };

        public WorksInSerial()
        {
            this.ID = new Typle<int>("[id]", -1);
            this.Work = new Typle<int>(this.Fields[0]);
            this.Serial = new Typle<int>(this.Fields[1]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<Int32> Work { get; set; }
        public Typle<Int32> Serial { get; set; }

        protected override WorksInSerial FromObject(object[] line)
            => new WorksInSerial()
            {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Work = new Typle<int>(this.Fields[0], To.Int(line[1])),
                Serial = new Typle<int>(this.Fields[1], To.Int(line[2])),
            };
    }
}