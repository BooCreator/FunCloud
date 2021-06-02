using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Саатти");
            //Printer.print(Saatti.Array);

            //Console.WriteLine("Веса");
            //Printer.print(Saatti.Weights());

            int[,] array = new int[,] {
                { 1, 1, 0, 1 },
                { 1, 0, 0, 1 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 0 },
            };
            Printer.print(array);

            var Basic = new Work()
            {
                ID = -1,
                Universe = 1,
                Fandome = 1,
                Serial = 0,
                Author = 1,
            };
            var Items = new List<Work>();
            for (int i = 0; i < array.GetLength(1); i++)
            {
                Items.Add(new Work() {
                    ID = i,
                    Universe = array[0, i],
                    Fandome = array[1, i],
                    Serial = array[2, i],
                    Author = array[3, i],
                });
            }

            var items = KemeniSnell.GetAnalog(Basic, Items);

            Console.WriteLine();
            foreach (Work item in items)
                Console.Write(item.ID + " ");
            Console.WriteLine();

            Console.WriteLine("---");
            Console.ReadLine();
        }

        

    }

    public static class Printer
    {
        public static void print(double [] array)
        {
            for (int i = 0; i < Saatti.Count; i++)
                Console.Write(Math.Round(array[i], 2) + " ");
            Console.WriteLine();
        }

        public static void print(int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(array[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public static void print(double[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(Math.Round(array[i, j], 2) + " ");
                }
                Console.WriteLine();
            }
        }

    }

}
