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
        static ViewParksDAL v = new ViewParksDAL();

        public static void MainMenu()
        {
            while (true)
            {
                string[] menu = { "Main Menu", "1) View Parks", "Q) Quit" };
                PrintMenuDoubleSpaced(menu);

                string input = Console.ReadLine();

                if (input == "1")
                {
                    Console.Clear();
                    Console.WriteLine("Viewing All Parks...");
                    Console.WriteLine();

                    List<Park> parks = v.ViewParks(connectionString);

                    ChooseParkMenu(parks);
                }
                if (input.ToLower() == "q")
                {
                    Console.Clear();
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
            Console.Clear();
            ParkInformationScreen(result);
        }

        public static void ParkInformationScreen(string park)
        {
            List<string> parkInfo = v.ViewParkInformation(park, connectionString);
            PrintMenuSingleSpace(parkInfo.ToArray());
            PrintMenuDoubleSpaced(new[] { "1) View Campgrounds", "2) Search For Reservation", "3) Return to Previous Screen"});
            CLIHelper.GetString("Select a Command");
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
