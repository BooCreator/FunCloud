using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Like : Basic<Like>
    {
        public override string Table => "[Like]";

        public override string[] Fields => new string[] { "[work]", "[author]", "[is_like]" };

        public override string[] Types => new string[] { "int", "int", "int" };

        public Like()
        {
            this.ID = new Typle<int>("[id]");
            this.Work = new Typle<int>(this.Fields[0]);
            this.Author = new Typle<int>(this.Fields[1]);
            this.IsLike = new Typle<int>(this.Fields[2]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<Int32> Work { get; set; }
        public Typle<Int32> Author { get; set; }
        public Typle<Int32> IsLike { get; set; }

        protected override Like FromObject(object[] line)
            => new Like() {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Work = new Typle<int>(this.Fields[0], To.Int(line[1])),
                Author = new Typle<int>(this.Fields[1], To.Int(line[2])),
                IsLike = new Typle<int>(this.Fields[2], To.Int(line[3]))
            };

    }
}