using System;
using DataBaseConnector;
using DataBaseConnector.Ext;
using System.Collections.Generic;

namespace FunCloud.Models.DataBase
{
    public abstract class Basic<T>
    {
        public abstract String Table { get; }
        public abstract String[] Fields { get; }
        public abstract String[] Types { get; }

        public virtual Entity Get(DataBaseExtended DB, String Where = "")
            => DB.Select($"select * from {this.Table}" + ((Where.Length > 0) ? $" where {Where}" : ""));
        public virtual Entity Get(DataBaseExtended DB, Int32 Count, Int32 Start = 0, String Where = "1 = 1")
        {
            Entity data = DB.Select($"select top {Start} id from {this.Table} where {Where}");
            int lastID = (data.Lines.Count > 0) ? To.Int(data.Lines[data.Lines.Count - 1][0]) : -1;
            return DB.Select($"select top {Count} * from {this.Table} where id > {lastID} and {Where}");
        }

        public T Find(DataBaseExtended DB, String Where, out Boolean Multiple)
        {
            Entity temp = DB.Select($"select * from {this.Table} where {Where}");
            if (temp.Lines.Count > 0)
            {
                Multiple = (temp.Lines.Count > 1);
                return this.FromObject(temp.Lines[0]);
            }
            Multiple = false;
            return default;
        }
        public Int32 FindIndex(DataBaseExtended DB, String Where, out Boolean Multiple)
        {
            Entity temp = DB.Select($"select id from {this.Table} where {Where}");
            if (temp.Lines.Count > 0)
            {
                Multiple = (temp.Lines.Count > 1);
                return To.Int(temp.Lines[0][0]);
            }
            Multiple = false;
            return -1;
        }

        public Boolean Add(DataBaseExtended DB, String[] Values)
        {
            DB.Table = this.Table;
            DB.Fields = this.Fields;
            return DB.Insert(Values) > 0;
        }
        public Boolean Update(DataBaseExtended DB, String[] Fields, String[] Values, String Where)
        {
            DB.Table = this.Table;
            DB.Fields = Fields;
            return DB.Update(Values, Where) > 0;
        }
        public Boolean Update(DataBaseExtended DB, String Field, String Value, String Where)
        {
            DB.Table = this.Table;
            DB.Fields = new string[] { Field };
            return DB.Update(new string[] { Value }, Where) > 0;
        }
        public Boolean Remove(DataBaseExtended DB, String Where)
            => DB.Delete(this.Table, Where) > 0;
        public Int32 Count(DataBaseExtended DB, String Where = "1 = 1")
            => DB.Scalar($"select COUNT(id) from {this.Table} where {Where}");

        public List<T> FromEntity(Entity Data)
        {
            var result = new List<T>();

            foreach (object[] line in Data.Lines)
            {
                result.Add(this.FromObject(line));
            }

            return result;
        }

        protected abstract T FromObject(Object[] line);

    }
}