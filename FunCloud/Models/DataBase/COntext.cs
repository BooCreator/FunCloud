using System;
using DataBaseConnector;
using DataBaseConnector.Ext;
using System.Web.Configuration;
using FunCloud.Models.DataBase;
using System.Collections.Generic;

namespace FunCloud
{
    public static class Context
    {
        private static String query = "create table #TABLE (ID int NOT NULL PRIMARY KEY, #FIELDS);";

        public static Models.DataBase.Work Works = new Models.DataBase.Work();
        public static Models.DataBase.ViewWork ViewWorks = new Models.DataBase.ViewWork();
        public static Models.DataBase.WorkOnRequest WorksOnRequest = new Models.DataBase.WorkOnRequest();
        public static Models.DataBase.Serial Serials = new Models.DataBase.Serial();
        public static Models.DataBase.WorksInSerial WorksInSerial = new Models.DataBase.WorksInSerial();
        public static Models.DataBase.User Users = new Models.DataBase.User();
        public static Models.DataBase.Role Roles = new Models.DataBase.Role();
        public static Models.DataBase.State States = new Models.DataBase.State();
        public static Models.DataBase.Category Categories = new Models.DataBase.Category();
        public static Models.DataBase.Request Requests = new Models.DataBase.Request();
        public static Models.DataBase.Fandome Fandomes = new Models.DataBase.Fandome();

        public static Models.DataBase.Comment Comments = new Models.DataBase.Comment();
        public static Models.DataBase.Like Likes = new Models.DataBase.Like();

        public static Models.DataBase.List Lists = new Models.DataBase.List();
        public static Models.DataBase.WorksInList WorksInList = new Models.DataBase.WorksInList();

        public static Models.DataBase.Subscribe Subscribe = new Models.DataBase.Subscribe();
        public static Models.DataBase.Mark Marks = new Models.DataBase.Mark();

        public static List<String> CheckTables()
        {
            var log = new List<string> { "--Start!" };

            String last_query = "";

            var temp = new DataBase(WebConfigurationManager.AppSettings["DataBase"]);

            try
            {
                temp.Open();

                log.Add(_check(temp, Works, ref last_query));
                log.Add(_check(temp, ViewWorks, ref last_query));
                log.Add(_check(temp, WorksOnRequest, ref last_query));
                log.Add(_check(temp, Serials, ref last_query));
                log.Add(_check(temp, WorksInSerial, ref last_query));
                log.Add(_check(temp, Users, ref last_query));
                log.Add(_check(temp, Roles, ref last_query));
                log.Add(_fill(temp, Roles, new string[] { "'Администратор'", "'Пользователь'" }, ref last_query));
                log.Add(_check(temp, States, ref last_query));
                log.Add(_fill(temp, States, new string[] { "'В работе'", "'Заморожен'", "'Завершен'" }, ref last_query));
                log.Add(_check(temp, Categories, ref last_query));
                log.Add(_check(temp, Requests, ref last_query));
                log.Add(_check(temp, Fandomes, ref last_query));

                log.Add(_check(temp, Comments, ref last_query));
                log.Add(_check(temp, Likes, ref last_query));

                log.Add(_check(temp, Lists, ref last_query));
                log.Add(_check(temp, WorksInList, ref last_query));

                log.Add(_check(temp, Subscribe, ref last_query));

                log.Add(_check(temp, Marks, ref last_query));
                log.Add(_fill(temp, Marks, new string[] { "'Романтика'", "'18+'", "'Драма'" }, ref last_query));
            }
            catch (Exception e)
            {
                log.Add(last_query);
                log.Add(e.ToString());
            }
            finally
            {
                temp.Dispose();
            }

            log.Add("--End!");

            return log;
        }

        private static String _check<T>(DataBase DB, Basic<T> Table, ref String last_query)
        {
            List<String> text = new List<String>();

            if (!DB.TrySelect($"select * from {Table.Table}"))
            {
                for (int i = 0; i < Table.Fields.Length; i++)
                    text.Add(Table.Fields[i] + " " + Table.Types[i]);

                last_query = query.Replace("#TABLE", Table.Table).Replace("#FIELDS", To.String(text.ToArray(), ","));

                DB.NonQuery(last_query);
                return $"-table {Table.Table} created!";
            }
            return $"{Table.Table} -- ok";
        }

        public static String _fill<T>(DataBase DB, Basic<T> Table, String[] Values, ref String last_query)
        {
            if (DB.Scalar($"select count(*) from {Table.Table}") == 0)
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    last_query = $"insert into {Table.Table}(id, {To.String(Table.Fields, ",")}) values({i}, {Values[i]})";
                    DB.NonQuery(last_query);
                }
                return $"{Table.Table} filled!";
            }
            return $"{Table.Table} -- ok fill";
        }
        public static String _fill<T>(DataBase DB, Basic<T> Table, List<String> Values, ref String last_query)
            => _fill<T>(DB, Table, Values.ToArray(), ref last_query);
        public static String _fill<T>(DataBase DB, Basic<T> Table, Typle<Int32>[] Values, ref String last_query)
        {
            if (DB.Scalar($"select count(*) from {Table.Table}") == 0)
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    last_query = $"insert into {Table.Table}(id, {To.String(Table.Fields, ",")}) values({Values[i].Value}, {Values[i].Name})";
                    DB.NonQuery(last_query);
                }
                return $"{Table.Table} filled!";
            }
            return $"{Table.Table} -- ok fill";
        }
        public static String _fill<T>(DataBase DB, Basic<T> Table, List<Typle<Int32>> Values, ref String last_query)
            => _fill<T>(DB, Table, Values.ToArray(), ref last_query);

    }
}