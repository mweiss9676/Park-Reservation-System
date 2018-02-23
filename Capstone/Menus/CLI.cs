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

            bool parkExists = viewParkDAL.DoesParkExist(result, connectionString);
            while(!parkExists)
            {
                Console.WriteLine("That is not a valid option, please enter one of the choices below");
                ChooseParkMenu(parks);
            }

            ParkInformationScreen(result);
        }

        public static void ParkInformationScreen(string park)
        {
            Park p = viewParkDAL.GetParkByName(park, connectionString);
            List<string> parkInfo = viewParkDAL.ViewParkInformation(park, connectionString);
            Console.WriteLine($"{parkInfo[0]} National Park");
            Console.WriteLine();
            Console.WriteLine("{0, -20}{1, 0}", $"Location:", $"{parkInfo[1]}");
            Console.WriteLine("{0, -20}{1, 0}", $"Established:", $"{parkInfo[2]}");
            Console.WriteLine("{0, -20}{1, 0}", $"Area:", $"{parkInfo[3]}");
            Console.WriteLine("{0, -20}{1, 0}", $"Annual Visitors:", $"{parkInfo[4]}");
            PrintMenuDoubleSpaced(new[] { "1) View Campgrounds", "2) Search For Reservation", "3) Return to Previous Screen"});
            string input = CLIHelper.GetString("Select a Command");

            if (input == "1")
            {
                Console.Clear();
                ParkCampgroundScreen(p);
            }
            if (input == "2")
            {

            }
            if (input == "3")
            {
                Console.Clear();
                ChooseParkMenu(viewParkDAL.ViewParks(connectionString));
            }

        }
        
        public static void ParkCampgroundScreen(Park park)
        {
            List<Campground> campgrounds = campgroundDAL.GetCampgrounds(park, connectionString);

            Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}", $"", $"Name", $"Open", $"Close", $"Daily Fee");
            for(int i = 0; i < campgrounds.Count; i++)
            {
                Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}", $"{i + 1}", $"{campgrounds[i].Name}", $"{Months[campgrounds[i].OpenFromDate]}", $"{Months[campgrounds[i].OpenToDate]}", $"{campgrounds[i].DailyFee.ToString("c")}");
            }

            PrintMenuDoubleSpaced(new[] {"1) Search For Available Reservation", "2) Return to Previous Screen"});
            string result = CLIHelper.GetString("Select an Option");

            //List<Campground> camp = campgroundDAL.GetCampgrounds(park, connectionString);

            if (result == "1")
            {
                SearchForReservation(campgrounds);  // <-- why does this just use the 0th campground?
            }
            if (result == "2")
            {
                
            }
        }

        private static void SearchForReservation(List<Campground> campgrounds) 
        {
            Campsite reservationSite = new Campsite();

            string selectedCampground = CLIHelper.GetString("Which campground (i.e. 'Blackwoods)");
            DateTime arrival = CLIHelper.GetDateTime("What is the arrival date? (Month/Day/Year)");
            DateTime departure = CLIHelper.GetDateTime("What is the departure date? (Month/Day/Year)");
            Console.Clear();

            //right here we need to narrow down the list of campgrounds based on what the user selected
            Campground campground = campgroundDAL.GetCampgroundByName(selectedCampground, connectionString);
            if (!campgroundDAL.IsTheCampgroundOpen(campground, arrival, departure, connectionString))
            {
                Console.Clear();
                Console.WriteLine($"{campground.Name} campground is only open from {Months[campground.OpenFromDate]} to {Months[campground.OpenToDate]}, please choose another date range:");
                SearchForReservation(campgrounds);
            }

            Console.WriteLine(campground.Name + " Campground");
            Console.WriteLine();
            List<Campsite> sites = campsiteDAL.GetCampsitesByAvailability(connectionString, campground, arrival, departure);
            Console.WriteLine("{0, -15}{1, -15}{2, -15}{3, -15}{4, -15}{5, -15}", $"Site No.", $"Max Occup.", $"Accessible?", $"Max RV Length", $"Utility", $"Cost");
            Console.WriteLine();

            foreach (var site in sites)
            {
                decimal totalCost = campsiteDAL.CalculateCostOfReservation(site, arrival, departure, connectionString);
                Console.WriteLine("{0, -15}{1, -15}{2, -15}{3, -15}{4, -15}{5, -15}", $"{site.SiteID}", $"{site.MaxOccupancy}", $"{site.Accessible}", $"{site.MaxRvLength}", $"{site.Utilities}", $"{totalCost.ToString("c")}");
            }
            int reserveSite = CLIHelper.GetInteger("What site should be reserved?(enter 0 to cancel)");
            if (reserveSite == 0)
            {
                Console.Clear();
                SearchForReservation(campgrounds);
            }
            else
            {
                //first verify that the site_id entered exists from the list provided to the user only!
                bool exists = sites.Any(x => x.SiteID == reserveSite);
                if (exists)
                {
                    reservationSite.SiteID = reserveSite;
                    //book a reservation based on the site_id
                }
            }
            string nameOfReservation = CLIHelper.GetString("What name should the reservation be placed under?");
            // make the reservation here


            
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
