using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;
using System.Configuration;

namespace Capstone.Menus
{
    public class CLI
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;


        public static void MainMenu()
        {
            while (true)
            {


                string[] menu = { "Main Menu", "1) View Parks", "Q) Quit" };
                PrintMenuDoubleSpaced(menu);

                string input = Console.ReadLine();

                if (input == "1")
                {

                    Console.WriteLine("Viewing All Parks");
                    ViewParksDAL v = new ViewParksDAL();
                    v.ViewParks(connectionString);
                    List<string> output = new List<string>();
                    foreach (Park p in v.ViewParks(connectionString))
                    {
                        output.Add(p.Name);
                        output.Add(p.Location);
                        output.Add(p.EstablishDate.ToString());
                    }
                    PrintMenuSingleSpace(output.ToArray());
                }
                if (input.ToLower() == "q")
                {
                    Environment.Exit(0);
                }
            }
        }

        private static void PrintMenuDoubleSpaced(string[] menu)
        {
            foreach (string s in menu)
            {
                Console.WriteLine();
                Console.WriteLine(s);
                Console.WriteLine();
            }
        }
        private static void PrintMenuSingleSpace(string[] menu)
        {
            foreach (string s in menu)
            {
                Console.WriteLine(s);
            }
        }
    }
}
