using System;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class User : Basic<User>
    {
        public override String Table => "[User]";
        public override String[] Fields => new String[] { "[login]", "[password]", "[role]", "[reg_date]", "[login_date]" };
        public override String[] Types => new String[] { "varchar(25)", "varchar(40)", "int", "varchar(12)", "varchar(12)" };

        public Typle<Int32> ID { get; set; }
        public Typle<String> Login { get; set; }
        public Typle<String> Password { get; set; }
        public Typle<Int32> Role { get; set; }
        public Typle<String> Reg_date { get; set; }
        public Typle<String> Log_date { get; set; }

        public User()
        {
            this.ID = new Typle<int>("[id]", -1);
            this.Login = new Typle<string>(this.Fields[0]);
            this.Password = new Typle<string>(this.Fields[1]);
            this.Role = new Typle<int>(this.Fields[2]);
            this.Reg_date = new Typle<string>(this.Fields[3]);
            this.Log_date = new Typle<string>(this.Fields[4]);
        }

        public Boolean Add(DataBaseExtended DB, String Login, String Password, Int32 Role)
        {
            DB.Table = this.Table;
            DB.Fields = this.Fields;
            return DB.Insert($"'{Login}', '{Password}', {Role}, '{DateTime.Now.ToShortDateString()}', '{DateTime.Now.ToShortDateString()}'") > 0;
        }

        protected override User FromObject(object[] line) =>
            new User() {
                ID = new Typle<int>("[id]" ,To.Int(line[0])),
                Login = new Typle<string>(this.Fields[0], To.String(line[1])),
                Password = new Typle<string>(this.Fields[1], To.String(line[2])),
                Role = new Typle<int>(this.Fields[2], To.Int(line[3])),
                Reg_date = new Typle<string>(this.Fields[3], To.String(line[4])),
                Log_date = new Typle<string>(this.Fields[4], To.String(line[5]))
            };
    }

}