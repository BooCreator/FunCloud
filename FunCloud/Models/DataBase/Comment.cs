using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Comment : Basic<Comment>
    {
        public override string Table => "[Comment]";

        public override string[] Fields => new string[] { "[work]", "[author]", "[answer]", "[pub_date]", "[text]" };

        public override string[] Types => new string[] { "int", "int", "int", "varchar(21)", "varchar(255)" };

        public Comment()
        {
            this.ID = new Typle<int>("[id]", -1);
            this.Work = new Typle<int>(this.Fields[0]);
            this.Author = new Typle<int>(this.Fields[1]);
            this.Answer = new Typle<int>(this.Fields[2]);
            this.Pub_date = new Typle<string>(this.Fields[3]);
            this.Text = new Typle<string>(this.Fields[4]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<Int32> Work { get; set; }
        public Typle<Int32> Author { get; set; }
        public Typle<Int32> Answer { get; set; }
        public Typle<String> Pub_date { get; set; }
        public Typle<String> Text { get; set; }

        protected override Comment FromObject(object[] line)
            => new Comment() {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Work = new Typle<int>(this.Fields[0], To.Int(line[1])),
                Author = new Typle<int>(this.Fields[1], To.Int(line[2])),
                Answer = new Typle<int>(this.Fields[2], To.Int(line[3])),
                Pub_date = new Typle<string>(this.Fields[3], To.String(line[4])),
                Text = new Typle<string>(this.Fields[4], To.String(line[5])),
            };
    }
}