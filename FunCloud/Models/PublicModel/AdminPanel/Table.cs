using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FunCloud.Models.PublicModel.AdminPanel
{
    public class Table<T>
    {
        public Table(List<T> Items) 
            => this.Items = Items;
        public List<T> Items { get; } = new List<T>();

        public void Add(T Item) 
            => this.Items.Add(Item);

    }
}