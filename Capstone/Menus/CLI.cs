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
                    Console.WriteLine("Viewing All Parks...");
                    Console.WriteLine();

                    ViewParksDAL v = new ViewParksDAL();
                    List<Park> parks = v.ViewParks(connectionString);

                    ChooseParkMenu(parks);
                }
                if (input.ToLower() == "q")
                {
                    Environment.Exit(0);
                }
            }
        }

        public static void ChooseParkMenu(List<Park> parks)
        {
            List<string> menu = new List<string>();
            for (int i = 1; i <= parks.Count; i++)
            {
                menu.Add(i + ") " + parks[i - 1].Name);
            }
            PrintMenuDoubleSpaced(menu.ToArray());
            string result = CLIHelper.GetString("Select A Park For Further Details");

        }

        public static void ParkInformationScreen()
        {

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
