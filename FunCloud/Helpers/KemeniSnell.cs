using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataBaseConnector.Ext;

namespace FunCloud.Helpers
{
    //"universe", "fandome", "serial", "author", "marks"
    public static class Saatti
    {
        public const Int32 Count = 5;

        //"universe", "fandome", "serial", "author", "marks"
        public static Double[,] Array = new Double[,] { 
            { 1.0,    3.0,    1.0,    1/2.0,  1/3.0 },  // "universe"
            { 1/3.0,  1.0,    1/3.0,  1.0,    1/4.0 },  // "fandome"
            { 1.0,    3.0,    1.0,    1.0,    1/2.0 },  // "serial"
            { 2.0,    1.0,    1.0,    1.0,    1/2.0 },  // "author"
            { 3.0,    4.0,    2.0,    2.0,    1.0   },  // "marks"
        };

        public static Double[] Weights()
        {
            double[] line = new double[Count];
            double sum = 0;
            for (int i = 0; i < Count; i++)
            {
                double tmp = 0;
                for(int j = 0; j < Count; j++)
                    tmp += Array[i, j];
                line[i] = tmp / Count;
                sum += line[i];
            }
            for (int i = 0; i < Count; i++)
                line[i] /= sum;
            return line;
        }


    }

    public class Work
    {
        public Work(Models.DataBase.Work Work, List<String> BasicMarks)
        {
            using(var DB = new DataBaseConnector.DataBaseExtended(Global.ConnectionString))
            {
                this.ID = Work.ID.Value;
                this.Universe = Work.Fandome.Value;
                this.Fandome = Work.Category.Value;
                this.Serial = To.Int(DB.SelectOne(Context.WorksInSerial.Table, Context.WorksInSerial.ID.Name, $"{Context.WorksInSerial.Work.Name} = {Work.ID.Value}"));
                this.Author = Work.Author.Value;
                List<string> marks = From.String(Work.Marks.Value, ",");
                foreach (string mark in marks)
                    this.Marks += (BasicMarks.FindIndex(x => x.CompareTo(mark) == 0) > -1) ? 1 : 0;
            }
            
        }

        public Int32 ID { get; set; }
        public Int32 Universe { get; set; }
        public Int32 Fandome { get; set; }
        public Int32 Serial { get; set; } = -1;
        public Int32 Author { get; set; }
        public Int32 Marks { get; set; }
        public Boolean Used { get; set; } = false;
    }

    public static class KemeniSnell
    {
        
        public static Int32 Min(Double[] array)
        {
            int min = 0;
            for (int i = 0; i < array.Length; i++)
                if ((array[i] > 0 && array[i] < array[min]) || array[min] == 0) min = i;
            return min;
        }

        public static void FlushLine(ref Double[,] Matrix, Int32 Line)
        {
            for(int i = 0; i < Matrix.GetLength(0); i++)
                Matrix[i, Line] = 0;
            for (int i = 0; i < Matrix.GetLength(1); i++)
                Matrix[Line, i] = 0;
        }
        public static List<Work> GetAnalog(Work Basic, List<Work> Items)
        {
            int count = Items.Count;
            var result = new List<Work>();

            // - ранжирование

            int[,] randged = new int[Saatti.Count, count];
            for(int j = 0; j < count; j++)
            {
                randged[0, j] = (Basic.Universe == Items[j].Universe) ? 1 : 2;
                randged[1, j] = (Basic.Fandome == Items[j].Fandome) ? 1 : 2;
                randged[2, j] = (Basic.Serial == Items[j].Serial) ? 1 : 2;
                randged[3, j] = (Basic.Author == Items[j].Author) ? 1 : 2;
                randged[4, j] = Basic.Marks + 1 - Items[j].Marks;
            }

            //Console.WriteLine("ранжирование");
            //print(randged);

            int[][,] ranged_k = new int[Saatti.Count][,];
            for(int k = 0; k < Saatti.Count; k++)
            {
                ranged_k[k] = new int[count, count];
                for(int i = 0; i < count; i++)
                    for (int j = 0; j < count; j++)
                    {
                        if (i == j || randged[k, i] == randged[k, j])
                            ranged_k[k][i, j] = 0;
                        else if (randged[k, i] < randged[k, j])
                            ranged_k[k][i, j] = 1;
                        else
                            ranged_k[k][i, j] = -1;
                    }
            }

            /*for(int i = 0; i < Saatti.Count; i++)
            {
                Console.WriteLine("k-" + i.ToString());
                print(ranged_k[i]);
            }*/

            // - потери

            double[,] matrix = new double[count, count];
            int lines = count;
            double[] v = Saatti.Weights();

            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                {
                    if (i == j)
                        matrix[i, j] = 0;
                    else
                    {
                        double sum = 0;
                        for (int k = 0; k < Saatti.Count; k++)
                            sum += v[k] * Math.Abs(ranged_k[k][i, j] - 1);
                        matrix[i, j] = sum;
                    }
                }

            double[] sums = new double[count];

            do
            {
                //Console.WriteLine("потери");
                //Printer.print(matrix);
                //Console.WriteLine();

                for (int i = 0; i < count; i++) 
                {
                    sums[i] = 0;
                    for (int j = 0; j < count; j++)
                        sums[i] += matrix[i, j];
                }

                //Console.WriteLine("Суммы");
                //Printer.print(sums);

                int min = Min(sums);

                Work item = Items[min];
                if (item != null)
                {
                    result.Add(item);
                    Items[min].Used = true;
                }

                FlushLine(ref matrix, min);

                lines--;

            } while (lines > 0);

            foreach (Work item in Items)
            {
                if(!item.Used)
                    result.Add(item);
            }

            return result;
        }

    }
}