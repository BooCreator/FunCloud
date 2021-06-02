using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Fandome : Basic<Fandome>
    {
        public override String Table => "[Fandome]";
        public override String[] Fields => new String[] { "[title]" };
        public override String[] Types => new String[] { "varchar(32)" };

        public Fandome()
        {
            this.ID = new Typle<int>("[id]");
            this.Title = new Typle<string>(this.Fields[0]);
        }

        public Typle<Int32> ID { get; set; }
        public Typle<String> Title { get; set; }

        public Boolean Add(DataBaseExtended DB, String Title)
        {
            DB.Table = this.Table;
            DB.Fields = this.Fields;
            return DB.Insert($"'{Title}'") > 0;
        }

        protected override Fandome FromObject(Object[] line)
        {
            return new Fandome() {
                ID = new Typle<int>("[id]", To.Int(line[0])),
                Title = new Typle<string>(this.Fields[0], To.String(line[1])),
            };
        }

    }
}