using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FunCloud
{
    public static class Error
    {
        public static String IsEmpty => "empty";
        public static String Exists => "exist";
        public static String NotAccess => "not_access";
        public static String Accept => "ok";
        public static String Unknown => "unknown_error";

    }
}