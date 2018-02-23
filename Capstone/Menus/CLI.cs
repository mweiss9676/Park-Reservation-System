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
        static ViewParksDAL viewParkDAL = new ViewParksDAL();
        static CampgroundDAL campgroundDAL = new CampgroundDAL();
        static CampsiteDAL campsiteDAL = new CampsiteDAL();

        static Dictionary<int, string> Months = new Dictionary<int, string>
        {
            {1,"January"},
            {2,"February"},
            {3,"March"},
            {4,"April"},
            {5,"May"},
            {6,"June"},
            {7,"July"},
            {8,"August"},
            {9,"September"},
            {10,"October"},
            {11,"November"},
            {12,"December"},
        };

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

                    List<Park> parks = viewParkDAL.ViewParks(connectionString);

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
            Park p = viewParkDAL.GetParkByName(park, connectionString);
            List<string> parkInfo = viewParkDAL.ViewParkInformation(park, connectionString);
            PrintMenuSingleSpace(parkInfo.ToArray());
            PrintMenuDoubleSpaced(new[] { "1) View Campgrounds", "2) Search For Reservation", "3) Return to Previous Screen"});
            string input = CLIHelper.GetString("Select a Command");

            if (input == "1")
            {
                Console.Clear();
                ParkCampgroundScreen(p);
            }
        }
        
        public static void ParkCampgroundScreen(Park park)
        {
            List<Campground> camps = campgroundDAL.GetCampgrounds(park, connectionString);

            Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}", $"", $"Name", $"Open", $"Close", $"Daily Fee");
            for(int i = 0; i < camps.Count; i++)
            {
                Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}", $"{i + 1}", $"{camps[i].Name}", $"{Months[camps[i].OpenFromDate]}", $"{Months[camps[i].OpenToDate]}", $"{camps[i].DailyFee.ToString("c")}");
            }

            PrintMenuDoubleSpaced(new[] {"1) Search For Available Reservation", "2) Return to Previous Screen"});
            string result = CLIHelper.GetString("Select an Option");

            List<Campground> camp = campgroundDAL.GetCampgrounds(park, connectionString);

            DateTime dt = new DateTime(2018, 01, 01);
            DateTime dt2 = new DateTime(2018, 05, 01);
            if (result == "1")
            {
                List<Campsite> sites = campsiteDAL.GetCampsitesByAvailability(connectionString, camp[0], dt, dt2);
                foreach (var site in sites)
                {
                    Console.WriteLine(site.SiteID);
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
