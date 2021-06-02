using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnector.Ext
{

    public class Typle<T>
    {
        public String Name { get; set; }
        public T Value { get; set; }

        public Typle(String Name) 
            => this.Name = Name;
        public Typle(String Name, T Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        public override string ToString()
            => this.Value.ToString();

    }

    public class Typle : Typle<String>
    {
        public Typle(String Name, String Value) : base(Name, Value) { }

    }

    public static class To
    {
        public static String String(String[] Array, String Devider = "")
        {
            StringBuilder temp = new StringBuilder();
            if (Array.Length > 0)
                temp.Append(Array[0]);
            for (int i = 1; i < Array.Length; i++)
                temp.Append(Devider + Array[i]);
            return temp.ToString();
        }

        public static DateTime Date(Object Value)
        {
            DateTime.TryParse(To.String(Value), out DateTime result);
            return result;
        }
        public static Int32 Int(Object Value)
        {
            if (Value != null)
            {
                Int32.TryParse(Value.ToString(), out int result);
                return result;
            }
            else return -1;
        }

        public static Double Float(Object Value)
            => Double.Parse(Value.ToString());

        public static String String(Object Value)
            => Value.ToString();

        public static List<String> StringArray(Entity Table)
        {
            List<string> result = new List<string>();
            StringBuilder temp = new StringBuilder();
            foreach (object[] Line in Table.Lines)
            {
                temp.Clear();
                foreach(object Item in Line)
                {
                    temp.Append(To.String(Item) + " ");
                }
                result.Add(temp.ToString());
            }
            return result;
        }

        public static List<String> StringArrayTransponire(Entity Table)
        {
            List<string> result = new List<string>();
            StringBuilder temp = new StringBuilder();

            for (int i = 0; i < Table.Cols.Count; i++)
            {
                temp.Clear();
                for (int j = 0; j < Table.Lines.Count; j++)
                {
                    temp.Append(To.String(Table.Lines[j][i]) + " ");
                }
                result.Add(temp.ToString());
            }
            return result;
        }

        /// <summary>
        /// Ограничение в 2 столбца (1 - Значение, 2 - Текст)!
        /// </summary>
        /// <param name="Table">Ограничение в 2 столбца (1 - Значение, 2 - Текст)!</param>
        public static List<Typle> Typle(Entity Table)
        {
            List<Typle> result = new List<Typle>();

            for (int i = 0; i < Table.Lines.Count; i++)
            {
                result.Add(new Ext.Typle(To.String(Table.Lines[i][0]),To.String(Table.Lines[i][1])));
            }
            return result;
        }

    }

    public static class From
    {
        public static List<String> String(String Value, String Devider)
        {
            var result = new List<string>();
            int pos = Value.IndexOf(Devider);
            while(pos > -1)
            {
                string tmp = Value.Substring(0, pos);
                result.Add(tmp);
                Value = Value.Replace(tmp + Devider, "");
                pos = Value.IndexOf(Devider);
            }
            if(Value.Length > 0)
                result.Add(Value);
            return result;
        }

        public static List<String> String(String Value, Char Devider)
        {
            var result = new List<string>();
            int pos = Value.IndexOf(Devider);
            while (pos > -1)
            {
                string tmp = Value.Substring(0, pos);
                result.Add(tmp.Replace(Devider.ToString(), ""));
                Value = Value.Substring(pos + 1);
                pos = Value.IndexOf(Devider);
            }
            if (Value.Length > 0)
                result.Add(Value);
            return result;
        }

    }

    public enum DBType
    {
        MS_ACCESS,
        MS_SQLSERVER
    }

    public static class HSQLStrings
    {
        public static string Server = "#SERVERNAME";
        public static string DataBase = "#DATABASENAME";
        public static string Login = "#LOGIN";
        public static string Password = "#PASSWORD";

        public static string SQL_SERVER_LOCAL = @"Data Source=#SERVERNAME;Initial Catalog=#DATABASENAME;Integrated Security=True";
        public static string SQL_SERVER_USER = @"Server=#SERVERNAME; Database=#DATABASENAME; User Id=#LOGIN; Password=#PASSWORD;";
    }

}
