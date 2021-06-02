using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.View
{
    public class ListView
    {
        public ListView(DataBaseExtended DB, Models.DataBase.List List)
        {
            this.ID = List.ID.Value;
            this.Title = List.Title.Value;

            Entity table = 
                DB.Select(
                    $"select {Context.Works.Table}.{Context.Works.Title.Name}, {Context.WorksInList.Table}.{Context.WorksInList.Work.Name} " +
                    $"from {Context.Works.Table} left join {Context.WorksInList.Table} on {Context.Works.Table}.{Context.Works.ID.Name} = {Context.WorksInList.Table}.{Context.WorksInList.Work.Name} " +
                    $"where {Context.WorksInList.Table}.{Context.WorksInList.List.Name} = {List.ID.Value}"
                );
            
            foreach(object[] line in table.Lines)
                this.Works.Add(new Typle<int>(To.String(line[0]), To.Int(line[1])));
        }

        public Int32 ID { get; set; }
        public String Title { get; set; }
        //public Int32 Author { get; set; }
        public List<Typle<Int32>> Works { get; set; } = new List<Typle<int>>();

    }
}