using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FunCloud.Find
{
    public class FindPublicModel
    {
        public String Text { get; set; } = "";
        public Int32 Author { get; set; } = -1;
        public Int32 Category { get; set; } = -1;
        public Int32 Fandome { get; set; } = -1;
        public Int32 Request { get; set; } = -1;
        public Int32 Serial { get; set; } = -1;
        public Int32 Page { get; set; } = 0;

        public Boolean Popular { get; set; } = false;
        public String StartDate { get; set; } = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd");
        public String EndDate { get; set; } = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

    }
}