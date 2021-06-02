using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Role : Basic<Role>
    {

        public override String Table => "[Role]";
        public override String[] Fields => new String[] { "[title]" };
        public override String[] Types => new String[] { "varchar(32)" };

        public Role()
        {
            this.ID = new Typle<int>("[id]");
            this.Title = new Typle<string>(this.Fields[0]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<String> Title { get; set; }

        protected override Role FromObject(object[] line) =>
            new Role() {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Title = new Typle<string>(this.Fields[0], To.String(line[1])),
            };
    }
}