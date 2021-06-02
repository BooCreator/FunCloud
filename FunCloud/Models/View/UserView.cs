using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.View
{
    public class UserView
    {
        public UserView(DataBaseExtended DB, Models.DataBase.User User)
        {
            this.ID = User.ID.Value;
            this.Role = User.Role.Value;
            this.Login = User.Login.Value;
            this.Reg_date = User.Reg_date.Value;
            this.Log_date = User.Log_date.Value;
            this.PopularWorks = Context.Works.FromEntity(
                DB.Select(
                    $"select top 10 * from {Context.Works.Table} where {Context.Works.Author.Name} = {User.ID.Value} order by {Context.Works.Like.Name} desc"
                )
            );
            this.Serials = Context.Serials.FromEntity(
                DB.Select(
                    $"select * from {Context.Serials.Table} where {Context.Serials.Author.Name} = {User.ID.Value} order by {Context.Serials.Title.Name} desc"
                )
            );

            List<Models.DataBase.List> lists = Context.Lists.FromEntity(
                DB.Select(
                    $"select * from {Context.Lists.Table} where {Context.Lists.Author.Name} = { User.ID.Value}"
                )
            );

            foreach(Models.DataBase.List list in lists)
            {
                this.Lists.Add(new ListView(DB, list));
            }

            Entity table =
                DB.Select(
                    $"select {Context.Users.Table}.{Context.Users.Login.Name}, {Context.Subscribe.Table}.{Context.Subscribe.Follower.Name} " +
                    $"from {Context.Users.Table} left join {Context.Subscribe.Table} on {Context.Users.Table}.{Context.Users.ID.Name} = {Context.Subscribe.Table}.{Context.Subscribe.Follower.Name}" +
                    $"where {Context.Subscribe.User.Name} = {User.ID.Value}"
                );
            foreach (object[] line in table.Lines)
                this.Followers.Add(new Typle<int>(To.String(line[0]), To.Int(line[1])));

            table =
                DB.Select(
                    $"select {Context.Users.Table}.{Context.Users.Login.Name}, {Context.Subscribe.Table}.{Context.Subscribe.User.Name} " +
                    $"from {Context.Users.Table} left join {Context.Subscribe.Table} on {Context.Users.Table}.{Context.Users.ID.Name} = {Context.Subscribe.Table}.{Context.Subscribe.User.Name}" +
                    $"where {Context.Subscribe.Follower.Name} = {User.ID.Value}"
                );
            foreach (object[] line in table.Lines)
                this.Users.Add(new Typle<int>(To.String(line[0]), To.Int(line[1])));
        }

        public Int32 ID { get; set; }
        public Int32 Role { get; set; }
        public String Login { get; set; }
        public String Reg_date { get; set; }
        public String Log_date { get; set; }
        public List<Models.DataBase.Work> PopularWorks { get; set; }
        public List<Models.DataBase.Serial> Serials { get; set; }
        public List<ListView> Lists { get; set; } = new List<ListView>();
        public List<Typle<Int32>> Followers { get; set; } = new List<Typle<Int32>>();
        public List<Typle<Int32>> Users { get; set; } = new List<Typle<Int32>>();

    }
}