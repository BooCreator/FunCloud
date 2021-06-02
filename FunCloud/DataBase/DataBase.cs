using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Common;
using System.Data;
using DataBaseConnector.Ext;
using System.Text;

namespace DataBaseConnector
{
    public class DataBase : IDisposable
    {
        #region Переменные

        private DbConnection conn = null;

        private DBType type = DBType.MS_SQLSERVER;

        #endregion

        #region Конструкторы

        public DataBase(String ConnectionString, DBType Type = DBType.MS_SQLSERVER)
        {
            this.type = Type;
            this.conn = this._connection(ConnectionString);
            this.Open();
        }

        #endregion

        public void Open()
        {
            if (!(this.conn.State == ConnectionState.Open))
                this.conn.Open();
        }
        public void Close()
            => this.conn.Close();
        public void Dispose()
            => this.conn.Dispose();

        public Entity Select(String Query)
        {
            DbDataReader reader = this._command(Query).ExecuteReader();
            var result = new Entity();

            for (int i = 0; i < reader.FieldCount; i++)
                result.Cols.Add(reader.GetName(i));

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    object[] line = new object[result.Cols.Count];
                    reader.GetValues(line);
                    result.Lines.Add(line);
                }
            }
            reader.Close();

            return result;
        }
        public Boolean TrySelect(String Query, out Entity res)
        {
            res = null;
            try
            {
                res = this.Select(Query);
                return true;
            } catch { return false; }
        }
        public Boolean TrySelect(String Query) 
            => this.TrySelect(Query, out Entity _);

        public Int32 Scalar(String Query)
            => To.Int(this._command(Query).ExecuteScalar());
        
        public Int32 NonQuery(String Query)
            => To.Int(this._command(Query).ExecuteNonQuery());
        public Int32 NonQuery(String Query, String Field, Object Data)
        {
            DbCommand command = this._command(Query);
            DbParameter parameter = this._parameter(Field);
            parameter.Value = Data;
            command.Parameters.Add(parameter);
            return command.ExecuteNonQuery();
        }

        #region Методы создания классов БД

        private DbConnection _connection(String conn)
          =>
          (this.type == DBType.MS_ACCESS) ? new OleDbConnection(conn) :
          (this.type == DBType.MS_SQLSERVER) ? (DbConnection)new SqlConnection(conn) : null;

        private DbCommand _command(String query)
          =>
          (this.type == DBType.MS_ACCESS) ? new OleDbCommand(query, (OleDbConnection)this.conn) :
          (this.type == DBType.MS_SQLSERVER) ? (DbCommand)new SqlCommand(query, (SqlConnection)this.conn) : null;

        private DbParameter _parameter(String field)
          =>
          (this.type == DBType.MS_ACCESS) ? new OleDbParameter(field, OleDbType.VarBinary) :
          (this.type == DBType.MS_SQLSERVER) ? (DbParameter)new SqlParameter(field, SqlDbType.VarBinary) : null;

        #endregion

    }

    public class DataBaseExtended : DataBase, IDisposable
    {
        public String Table { get; set; }
        public String[] Fields { get; set; }

        public DataBaseExtended(String ConnectionString, DBType Type = DBType.MS_SQLSERVER) : base(ConnectionString, Type) { }

        public Entity Select(String Table, String Fields, String Where = "")
            => base.Select($"select {Fields} from {Table}" + ((Where.Length > 0) ? $" where {Where}" : ""));

        public Int32 Insert(String Table, String Fields, String Values)
        {
            int maxID = base.Scalar($"select MAX(id) from {Table}") + 1;
            return base.NonQuery($"insert into {Table}(id,{Fields}) values({maxID},{Values})");
        }

        public Int32 Update(String Table, String[] Fields, String[] Values, String Where)
        {
            if (Fields.Length > 0 && Fields.Length == Values.Length) {
                var temp = new StringBuilder();
                temp.Append($"{Fields[0]} = {Values[0]}");
                for (int i = 1; i < Fields.Length; i++)
                    temp.Append($",{Fields[i]} = {Values[i]}");
                return base.NonQuery($"update {Table} set {temp.ToString()}" + ((Where.Length > 0) ? $" where {Where}" : ""));
            } else 
                throw new Exception("Количество полей не равно количеству значений, либо заданных полей 0!");
        }
        public Int32 Update(String[] Values, String Where = "")
            => (this.Table.Length > 0 && this.Fields.Length > 0)
            ? this.Update(this.Table, this.Fields, Values, Where)
            : throw new Exception("Имя таблицы и/или поля не заданы!");

        public Int32 Delete(String Table, String Where = "")
            =>base.NonQuery($"delete from {Table}" + ((Where.Length > 0) ? $" where {Where}" : ""));

        public Int32 Insert(String[] Values)
        {
            if (this.Table.Length > 0 && this.Fields.Length > 0) { 
                if (this.Fields.Length == Values.Length) {
                    return this.Insert(this.Table, To.String(this.Fields, ","), To.String(Values, ","));
                } else 
                    throw new Exception("Количество полей не равно количеству значений!");
            } else 
                throw new Exception("Имя таблицы и/или поля не заданы!");
        }
        public Int32 Insert(String Values) 
            => (this.Table.Length > 0 && this.Fields.Length > 0) 
            ? this.Insert(this.Table, To.String(this.Fields, ","), Values) 
            : throw new Exception("Имя таблицы и/или поля не заданы!");

        public Object[] SelectLine(String Query)
            => base.Select(Query).Lines?[0];
        public Object[] SelectLine(String Table, String Fields, String Where = "")
            => this.Select(Table, Fields, Where).Lines?[0];

        public Object SelectOne(String Query)
        {
            Entity table = base.Select(Query);
            return (table.Lines.Count > 0) ? table.Lines[0][0] : null;
        }
        public Object SelectOne(String Table, String Field, String Where = "")
        {
            Entity table = this.Select(Table, Field, Where);
            return (table.Lines.Count > 0) ? table.Lines[0][0] : null;
        }

    }

}
