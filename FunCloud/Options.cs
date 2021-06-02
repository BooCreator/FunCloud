using System;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataBaseConnector;
using DataBaseConnector.Ext;
using System.Security.Principal;

namespace FunCloud
{
    public class UserData
    {
        public UserData(Int32 ID, String Name, Int32 Role)
        {
            this.UserID = ID;
            this.UserName = Name;
            this.UserRole = Role;
        }
        public Int32 UserID { get; set; } = -1;
        public Int32 UserRole { get; set; } = -1;
        public String UserName { get; set; }
    }

    public static class Global
    {
        public static String ConnectionString
            => WebConfigurationManager.AppSettings["DataBase"];

        public static List<UserData> LoginedUser = new List<UserData>();
        public static Int32 GetUserID(Controller Controller)
        {
            int result = -1;
            if (Controller.User.Identity.IsAuthenticated)
                result = Global.getLoginedUserID(Controller.User.Identity.Name);
            return result;
        }
        public static Int32 GetUserRole(Controller Controller)
        {
            int result = -1;
            if (Controller.User.Identity.IsAuthenticated)
                result = Global.getLoginedUserRole(Controller.User.Identity.Name);
            return result;
        }
        private static Int32 getLoginedUserID(String UserName)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Models.DataBase.User user = Context.Users.Find(DB, $"{Context.Users.Login.Name} like '{UserName}'", out _);
                return (user != null)
                    ? findInLoginedUser(user) 
                    : -1;
            }
        }
        private static Int32 getLoginedUserRole(String UserName)
        {
            using (var DB = new DataBaseExtended(Global.ConnectionString))
            {
                Models.DataBase.User user = Context.Users.Find(DB, $"{Context.Users.Login.Name} like '{UserName}'", out _);
                return (user != null)
                    ? findInLoginedUserRole(user)
                    : -1;
            }
        }
        private static Int32 findInLoginedUser(Models.DataBase.User User)
        {
            int result = User.ID.Value;
            if (LoginedUser.Find(x => x.UserID == result) == null)
                LoginedUser.Add(
                    new UserData(
                        User.ID.Value,
                        User.Login.Value,
                        User.Role.Value));
            return result;
        }
        private static Int32 findInLoginedUserRole(Models.DataBase.User User)
        {
            if (LoginedUser.Find(x => x.UserID == User.ID.Value) == null)
                LoginedUser.Add(
                    new UserData(
                        User.ID.Value,
                        User.Login.Value,
                        User.Role.Value));
            return User.Role.Value;
        }
        public static Int32 AdminRoleID { get; } = 0;

    }
}