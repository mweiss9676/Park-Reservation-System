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
                Console.WriteLine("Welcome To The Park Reservation System!!");
                string[] menu = { "1) View Parks", "2) Quit" };
                PrintMenuDoubleSpaced(menu);

                string input = Console.ReadLine();

                if (input == "1" || input.ToLower() == "view parks" || input.ToLower() == "view")
                {
                    Console.Clear();
                    Console.WriteLine("Viewing All Parks...");

                    List<Park> parks = viewParkDAL.ViewParks(connectionString);

                    ChooseParkMenu(parks);
                }
                if (input.ToLower() == "q" || input.ToLower() == "quit" || input.ToLower() == "2")
                {
                    Console.Clear();
                    Environment.Exit(0);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("That is not a valid option, please select from one of the following choices: ");
                    MainMenu();
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
                Console.WriteLine("That is not a valid option, please enter one of the choices below...");
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
            Console.WriteLine();
            Console.WriteLine($"{parkInfo[5]}");
            PrintMenuDoubleSpaced(new[] { "1) View Campgrounds", "2) Search For Reservation", "3) Return to Previous Screen"});
            string input = CLIHelper.GetString("Select a Command");

            if (input == "1")
            {
                Console.Clear();
                ParkCampgroundScreen(p);
            }
            if (input == "2")
            {
                Console.Clear();
                SearchForReservationByName();
            }
            if (input == "3")
            {
                Console.Clear();
                ChooseParkMenu(viewParkDAL.ViewParks(connectionString));
            }
            else
            {
                Console.Clear();
                Console.WriteLine("That is not a valid choice, please select from one of the below options: ");
                ParkInformationScreen(park);
            }

        }

        public static void ParkCampgroundScreen(Park park)
        {
            List<Campground> campgrounds = campgroundDAL.GetCampgrounds(park, connectionString);

            Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}", $"", $"Name", $"Open", $"Close", $"Daily Fee");
            for (int i = 0; i < campgrounds.Count; i++)
            {
                Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}", $"{i + 1}", $"{campgrounds[i].Name}", $"{Months[campgrounds[i].OpenFromDate]}", $"{Months[campgrounds[i].OpenToDate]}", $"{campgrounds[i].DailyFee.ToString("c")}");
            }

            PrintMenuDoubleSpaced(new[] { "1) Search For Available Reservation", "2) Return to Previous Screen" });
            string result = CLIHelper.GetString("Select an Option");

            if(campgrounds.Any(x => x.Name == result))
            {
                MakeNewReservation(campgrounds, name: result);
            }
            if (result == "1")
            {
                MakeNewReservation(campgrounds);  // <-- why does this just use the 0th campground?
            }
            if (result == "2")
            {
                Console.Clear();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("That is not a valid option, please select from the choices below...");
                ParkCampgroundScreen(park);
            }
        }

        private static void MakeNewReservation(List<Campground> campgrounds, string name = "") 
        {
            Campsite reservationSite = new Campsite();

            string selectedCampground = name;

            if (selectedCampground == "")
            {
                selectedCampground = CLIHelper.GetString("Which campground (i.e. 'Blackwoods)");
            }

            DateTime arrival = CLIHelper.GetDateTime("What is the arrival date? (Month/Day/Year)");
            DateTime departure = CLIHelper.GetDateTime("What is the departure date? (Month/Day/Year)");
            Console.Clear();

            //right here we need to narrow down the list of campgrounds based on what the user selected
            Campground campground = campgroundDAL.GetCampgroundByName(selectedCampground, connectionString);

            if (arrival == departure)
            {
                Console.WriteLine("You have selected the same arrival and departure day, minimum stay is 1 day...");
                MakeNewReservation(campgrounds, selectedCampground);
            }
            if (departure < arrival)
            {
                Console.WriteLine("You have selected a departure date that is earlier than your arrival date, have you made a mistake...?");
                MakeNewReservation(campgrounds, selectedCampground);
            }
            if (!campgroundDAL.IsTheCampgroundOpen(campground, arrival, departure, connectionString))
            {
                Console.Clear();
                Console.WriteLine($"{campground.Name} campground is only open from {Months[campground.OpenFromDate]} to {Months[campground.OpenToDate]}, please choose another date range:");
                MakeNewReservation(campgrounds);
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
            Console.WriteLine();

            int reserveSite = CLIHelper.GetInteger("What site should be reserved?(enter 0 to cancel)");
            if (reserveSite == 0)
            {
                Console.Clear();
                MakeNewReservation(campgrounds, selectedCampground);
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
                else
                {
                    Console.WriteLine("That is not a valid option, please select from the choices above...");
                    MakeNewReservation(campgrounds, selectedCampground);
                }
            }
            string nameOfReservation = CLIHelper.GetString("What name should the reservation be placed under?");
            // make the reservation here
            campsiteDAL.CreateReservation(reservationSite.SiteID, arrival, departure, nameOfReservation, connectionString);

            Console.WriteLine($"The reservation has been created and the confirmation id is {campsiteDAL.GetReservationID(reservationSite.SiteID, connectionString)}");                       
        }

        public static void SearchForReservationByName()
        {
            int reservationID = CLIHelper.GetInteger("What is your reservation id?");

            if (campsiteDAL.FindReservationByID(reservationID, connectionString) != null)
            {
                Console.WriteLine("Thank you! We found your reservation: ");
                PrintReservationInformation(campsiteDAL.FindReservationByID(reservationID, connectionString));
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"{reservationID} is not a valid choice, please select from one of the below options: ");
                SearchForReservationByName();
            }
        }

        private static void PrintReservationInformation(Reservation reservation)
        {
            Console.WriteLine($"We have you staying from {reservation.FromDate} through {reservation.ToDate}.");
            Console.WriteLine($"Your stay is under the name {reservation.Name} at site: {reservation.SiteID}");
            Console.WriteLine("Enjoy your stay!");
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
