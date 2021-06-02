using System;
using System.Web.Configuration;
using DataBaseConnector;
using DataBaseConnector.Ext;

namespace FunCloud.Models.DataBase
{
    public class Work: Basic<Work>
    {
        public override String Table => "[Work]";
        public override String[] Fields => new String[] { "[title]", "[category]", "[fandome]", "[description]", "[author]", "[state]", "[pub_date]", "[upd_date]","[files]", "[like]", "[dislike]", "[marks]" };
        public override String[] Types => new String[] { "varchar(32)", "int", "int", "varchar(256)", "int", "int", "varchar(21)", "varchar(21)", "varchar(255)", "int", "int", "varchar(255)" };

        public string FilesPath { get; private set; }
        public string MainFileName => "text";

        public Work() {
            this.FilesPath = WebConfigurationManager.AppSettings["FilesPath"];

            this.ID = new Typle<int>("[id]", -1);
            this.Title = new Typle<string>(this.Fields[0]);
            this.Category = new Typle<int>(this.Fields[1]);
            this.Fandome = new Typle<int>(this.Fields[2]);
            this.Description = new Typle<string>(this.Fields[3]);
            this.Author = new Typle<int>(this.Fields[4]);
            this.State = new Typle<int>(this.Fields[5]);
            this.Pub_date = new Typle<string>(this.Fields[6]);
            this.Upd_date = new Typle<string>(this.Fields[7]);
            this.Files = new Typle<string>(this.Fields[8]);
            this.Like = new Typle<int>(this.Fields[9]);
            this.Dislike = new Typle<int>(this.Fields[10]);
        }

        public Work(DataBaseExtended DB, Int32 ID)
        {
            Entity temp = this.Get(DB, $"id = {ID}");
            if (temp.Lines.Count > 0) {
                object[] line = temp.Lines[0];
                this.ID = new Typle<int>("[id]", To.Int(line[0]));
                this.Title = new Typle<string>(this.Fields[0], To.String(line[1]));
                this.Category = new Typle<int>(this.Fields[1], To.Int(line[2]));
                this.Fandome = new Typle<int>(this.Fields[2], To.Int(line[3]));
                this.Description = new Typle<string>(this.Fields[3], To.String(line[4]));
                this.Author = new Typle<int>(this.Fields[4], To.Int(line[5]));
                this.State = new Typle<int>(this.Fields[5], To.Int(line[6]));
                this.Pub_date = new Typle<string>(this.Fields[6], To.String(line[7]));
                this.Upd_date = new Typle<string>(this.Fields[7], To.String(line[8]));
                this.Files = new Typle<string>(this.Fields[8], To.String(line[9]));
                this.Like = new Typle<int>(this.Fields[9], To.Int(line[10]));
                this.Dislike = new Typle<int>(this.Fields[10], To.Int(line[11]));
                this.Marks = new Typle<string>(this.Fields[11], To.String(line[12]));
            }
        }

        public Boolean AddFiles(DataBaseExtended DB, Int32 WorkID, String FileNames)
        {
            return Context.Works.Update(DB, Context.Works.Files.Name, $"'{FileNames}'", $"{Context.Works.ID.Name} = {WorkID}");
        }

        public Typle<Int32> ID { get; set; }
        public Typle<String> Title { get; set; }
        public Typle<Int32> Category { get; set; }
        public Typle<Int32> Fandome { get; set; }
        public Typle<String> Description { get; set; }
        public Typle<Int32> Author { get; set; }
        public Typle<Int32> State { get; set; }
        public Typle<String> Pub_date { get; set; }
        public Typle<String> Upd_date { get; set; }
        public Typle<String> Files { get; set; }
        public Typle<Int32> Like { get; set; }
        public Typle<Int32> Dislike { get; set; }
        public Typle<String> Marks { get; set; }

        protected override Work FromObject(object[] line) =>
            new Work() {
                ID = new Typle<int>("[id]",To.Int(line[0])),
                Title = new Typle<string>(this.Fields[0], To.String(line[1])),
                Category = new Typle<int>(this.Fields[1], To.Int(line[2])),
                Fandome = new Typle<int>(this.Fields[2], To.Int(line[3])),
                Description = new Typle<string>(this.Fields[3], To.String(line[4])),
                Author = new Typle<int>(this.Fields[4], To.Int(line[5])),
                State = new Typle<int>(this.Fields[5], To.Int(line[6])),
                Pub_date = new Typle<string>(this.Fields[6], To.String(line[7])),
                Upd_date = new Typle<string>(this.Fields[7], To.String(line[8])),
                Files = new Typle<string>(this.Fields[8], To.String(line[9])),
                Like = new Typle<int>(this.Fields[9], To.Int(line[10])),
                Dislike = new Typle<int>(this.Fields[10], To.Int(line[11])),
                Marks = new Typle<string>(this.Fields[11], To.String(line[12])),
            };

    }
}