using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Capstone.Menus
{
    public class CLI
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        static ParkDAL viewParkDAL = new ParkDAL();
        static CampgroundDAL campgroundDAL = new CampgroundDAL();
        static CampsiteDAL campsiteDAL = new CampsiteDAL();
        static ConsoleColor foregroundColor = ConsoleColor.White;
        static ConsoleColor foregroundColorForText = ConsoleColor.Green;
        static ConsoleColor backgroundColor = ConsoleColor.Black;
        static ConsoleColor backgroundColorForText = ConsoleColor.Black;


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
            Console.Clear();
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.Clear();
            Console.SetWindowSize(Console.LargestWindowWidth, 41);
            Console.SetBufferSize(Console.LargestWindowWidth * 2, 100);
            Console.SetWindowPosition(0, 0);

            while (true)
            {
                PrintTrees();

                Console.WriteLine();
                PrintWelcome();

                Console.WriteLine();
                Console.WriteLine();
                string[] menu = { "1) View Parks", "2) Quit" };
                PrintMenuDoubleSpaced(menu);
                PrintCampers();

                string input = Console.ReadLine();

                if (input == "1" || input.ToLower() == "view parks" || input.ToLower() == "view")
                {
                    Console.Clear();

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
                    PrintMenuSingleSpace(new[] { "That is not a valid option, please select from one of the following choices: " });
                    Console.WriteLine();
                    MainMenu();
                }
            }
        }

        public static void ChooseParkMenu(List<Park> parks)
        {
            PrintTrees();
            Console.WriteLine();
            PrintMenuSingleSpace(new[] { "Viewing All Parks..." });
            Console.WriteLine();

            List<string> menu = new List<string>();

            for (int i = 1; i <= parks.Count; i++)
            {
                menu.Add(i + ") " + parks[i - 1].Name);
            }
            PrintMenuDoubleSpaced(menu.ToArray());


            PrintMenuSingleSpace(new[] { "Please Select a Park for More Information" });

            PrintTreesBottom();
            string userParkName = Console.ReadLine();

            Console.Clear();
            
            bool parkExists = viewParkDAL.DoesParkExist(userParkName, connectionString);

            if (parkExists)
            {
                ParkInformationScreen(userParkName);
            }
            if (userParkName == "1")
            {
                ParkInformationScreen("acadia");
            }
            if (userParkName == "2")
            {
                ParkInformationScreen("arches");
            }
            if (userParkName == "3" || userParkName.ToLower() == "cuyahoga")
            {
                ParkInformationScreen("cuyahoga valley");
            }
            else
            {
                Console.Clear();
                PrintMenuSingleSpace(new[] { $"{userParkName} is not a valid option, please enter one of the choices below..." });
                Console.WriteLine();
                ChooseParkMenu(parks);
            }          
        }

        public static void ParkInformationScreen(string parkName)
        {

            PrintTrees();
            Console.WriteLine();
            Park p = viewParkDAL.GetParkByName(parkName, connectionString);

            List<string> parkInfo = viewParkDAL.ViewParkInformation(parkName, connectionString);

            PrintMenuSingleSpace(new[] { $"{parkInfo[0]} National Park", "", "Location:".PadRight(20) + $"{parkInfo[1]}",
                                        $"Established:".PadRight(20) + $"{parkInfo[2].Substring(0, 10)}",
                                        $"Area:".PadRight(20) + $"{parkInfo[3]}", $"Annual Visitors:".PadRight(20) + $"{parkInfo[4]}"});
            Console.WriteLine();

            string[] result = SpliceText(parkInfo[5], 80);
            PrintMenuSingleSpace(result ); 
           

            Console.WriteLine();
            Console.WriteLine();
            PrintMenuDoubleSpaced(new[] { "1) View Campgrounds", "2) Search For Reservation", "3) Return to Park Selection Menu"});

            PrintTreesBottom();

            string input = CLIHelper.GetString("Select a Command");

            if (input == "1")
            {
                Console.Clear();
                ParkCampgroundScreen(p);
            }
            if (input == "2")
            {
                Console.Clear();
                SearchForReservationByID();
            }
            if (input == "3")
            {
                Console.Clear();
                ChooseParkMenu(viewParkDAL.ViewParks(connectionString));
            }
            else
            {
                Console.Clear();
                PrintMenuDoubleSpaced(new[] { "That is not a valid choice, please select from one of the below options: " });
                ParkInformationScreen(parkName);
            }

        }

        public static void ParkCampgroundScreen(Park park)
        {
            PrintTrees();
            Console.WriteLine();
            List<Campground> campgrounds = campgroundDAL.GetCampgrounds(park, connectionString);

            Console.SetCursorPosition((Console.WindowWidth - 70) / 2, Console.CursorTop);
            Console.WriteLine("{0, -40}{1, -20}{2, -20}{3, -20}", $"Name", $"Open", $"Close", $"Daily Fee");
            Console.WriteLine();
            for (int i = 0; i < campgrounds.Count; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth - 70) / 2, Console.CursorTop);
                Console.WriteLine("{0, -40}{1, -20}{2, -20}{3, -20}",  $"{campgrounds[i].Name}", $"{Months[campgrounds[i].OpenFromDate]}", $"{Months[campgrounds[i].OpenToDate]}", $"{campgrounds[i].DailyFee.ToString("c")}");
            }
            Console.WriteLine();
            Console.WriteLine();
            PrintMenuDoubleSpaced(new[] { "1) Search For Available Reservation", "2) Return to Previous Screen" });
            PrintTrees();
            string result = CLIHelper.GetString("Select an Option");

            if(campgrounds.Any(x => x.Name == result))
            {
                CheckReservationAvailabilityMenu(campgrounds, name: result);
            }
            if (result == "1")
            {
                CheckReservationAvailabilityMenu(campgrounds);  // <-- why does this just use the 0th campground?
            }
            if (result == "2")
            {
                Console.Clear();
                ParkInformationScreen(park.Name);
            }
            else
            {
                Console.Clear();
                PrintMenuDoubleSpaced(new[] { "That is not a valid option, please select from the choices below..." });
                ParkCampgroundScreen(park);
            }
        }

        private static void CheckReservationAvailabilityMenu(List<Campground> campgrounds, string name = "") 
        {
            Console.WriteLine();
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
            DateTime minDate = DateTime.Today;


            Park p = campgroundDAL.GetParkByCampgroundName(selectedCampground, connectionString);
            if (arrival <= minDate)
            {
                PrintMenuDoubleSpaced(new[] { "That is an unacceptable date, please re-enter your information" });
                ParkCampgroundScreen(p);
            }
            if (arrival == departure)
            {
                PrintMenuDoubleSpaced(new[] { "You have selected the same arrival and departure day, minimum stay is 1 day..." });
                ParkCampgroundScreen(p);
            }
            if (departure < arrival)
            {
                PrintMenuDoubleSpaced(new[] { "You have selected a departure date that is earlier than your arrival date, have you made a mistake...?" });
                ParkCampgroundScreen(p);
            }
            if (!campgroundDAL.IsTheCampgroundOpen(campground, arrival, departure, connectionString))
            {
                Console.Clear();
                PrintMenuDoubleSpaced(new[] { $"{campground.Name} campground is only open from {Months[campground.OpenFromDate]} to {Months[campground.OpenToDate]}, please choose another date range:" });
                ParkCampgroundScreen(p);
            }

            CreateReservationMenu(selectedCampground, campgrounds, campground, arrival, departure, connectionString);
        }

        private static void CreateReservationMenu(string selectedCampground, List<Campground> campgrounds, 
                                                  Campground campground, DateTime arrival, DateTime departure, 
                                                  string connectionString)
        {
            PrintMenuDoubleSpaced(new[] { campground.Name + " Campground" });

            PrintTrees();
            Console.WriteLine();
            List<Campsite> availableCampsites = campsiteDAL.GetCampsitesByAvailability(connectionString, campground, arrival, departure);

            if (availableCampsites.Count == 0)
            {
                Console.Clear();
                PrintMenuDoubleSpaced(new[] { "I'm sorry, there are no available dates for your selected visit, please select again..." });
                CheckReservationAvailabilityMenu(campgrounds, selectedCampground);
            }
            Console.SetCursorPosition((Console.WindowWidth - 80) / 2, Console.CursorTop);
            Console.WriteLine("{0, -15}{1, -15}{2, -15}{3, -15}{4, -15}{5, -15}", $"Site No.", $"Max Occup.", $"Accessible?", $"Max RV Length", $"Utility", $"Cost");
            Console.WriteLine();

            foreach (var site in availableCampsites)
            {
                decimal totalCost = campsiteDAL.CalculateCostOfReservation(site, arrival, departure, connectionString);
                Console.SetCursorPosition((Console.WindowWidth - 80) / 2, Console.CursorTop);
                Console.WriteLine("{0, -15}{1, -15}{2, -15}{3, -15}{4, -15}{5, -15}", $"{site.SiteID}", $"{site.MaxOccupancy}", $"{site.Accessible}", $"{site.MaxRvLength}", $"{site.Utilities}", $"{totalCost.ToString("c")}");
            }
            Console.WriteLine();
            
            Campsite reservationSite = new Campsite();//create a new campsite so that the user can book their stay

            PrintTreesBottom();
            int userSelectedSiteID = CLIHelper.GetInteger("What site should be reserved?(To Return to the Main Menu press (0))");

            //first verify that the site_id entered exists from the list provided to the user only!
            bool exists = availableCampsites.Any(x => x.SiteID == userSelectedSiteID);

            if (userSelectedSiteID == 0)
            {
                Console.Clear();
                MainMenu();
            }
            else if (exists)
            {
                reservationSite.SiteID = userSelectedSiteID;
                //book a reservation based on the site_id
            }
            else
            {
                Console.Clear();
                Console.WriteLine("That is not a valid option, please select from the choices below...");
                CreateReservationMenu(selectedCampground, campgrounds, campground, arrival, departure, connectionString);
              
            }

            string nameOfReservation = CLIHelper.GetString("What name should the reservation be placed under?");

            // ADDED BY JIMMY V 2-25 @ 9:45 AM // 
            
            // I added the IsSiteReserved method I created to the logic below //
            // I also refactored the method's name to be shorter //
            // There seems to be a weird loop going on in this menu //
            if (availableCampsites.Any(x => x.SiteID == userSelectedSiteID))
            {
                campsiteDAL.CreateReservation(reservationSite.SiteID, arrival, departure, nameOfReservation, connectionString);

                Console.WriteLine($"The reservation has been created and the reservation id is " +
                                  $"{campsiteDAL.GetReservationID(reservationSite.SiteID, connectionString)}");
                Console.WriteLine("Press Enter to Return to the Main Menu");
                Console.ReadLine();
                MainMenu();
            }
            // ^^ END ADDITIONS BY JIMMY V ^^ //
            else
            {
                // if the site has not been reserved on the provided dates the reservation will be created
                Console.WriteLine("This site is already reserved during the dates provided");
                CheckReservationAvailabilityMenu(campgrounds, selectedCampground);
            }
        }

        public static void SearchForReservationByID()
        {
            while (true)
            {
                int reservationID = CLIHelper.GetInteger("What is your reservation id? (0 to exit)");
                if (reservationID == 0)
                {
                    Console.Clear();
                    MainMenu();
                }
                string customerName = CLIHelper.GetString("What is your name?");
                Console.WriteLine();

                if (campsiteDAL.FindReservationByID(reservationID, customerName, connectionString) != null)
                {
                    Console.WriteLine("Thank you! We found your reservation: ");
                    Console.WriteLine();
                    PrintReservationInformation(campsiteDAL.FindReservationByID(reservationID, customerName, connectionString));
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"{customerName} with a reservation id of {reservationID} is not in our system, please re-enter your information: ");
                    SearchForReservationByID();
                }
            }
        }

        private static void PrintReservationInformation(Reservation reservation)
        {
            Console.WriteLine($"We have you staying from {reservation.FromDate} through {reservation.ToDate}.");
            Console.WriteLine($"Your stay is under the name {reservation.Name} at site: {reservation.SiteID}");
            Console.WriteLine("Enjoy your stay!");
            PrintMenuDoubleSpaced(new[] { "1) Return to the Main Menu", "2) Quit" });
            string option = CLIHelper.GetString("");
            if (option == "1")
            {
                Console.Clear();
                MainMenu();
            }
            if (option == "2")
            {
                Console.Clear();
                Environment.Exit(0);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("That is not a valid option, please select from the choices below...");
                PrintReservationInformation(reservation);
            }
        }

        public static string[] SpliceText(string text, int lineLength)
        {
            var charCount = 0;
            var lines = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .GroupBy(w => (charCount += w.Length + 1) / lineLength)
                            .Select(g => string.Join(" ", g));

            string[] result = lines.ToArray();
            return result;
        }

        private static void PrintMenuDoubleSpaced(string[] menu)
        {
            int longest = menu.Max(x => x.Length);

            foreach (string s in menu)
            {
                Console.SetCursorPosition((Console.WindowWidth - longest) / 2, Console.CursorTop);

                Console.WriteLine(s);
                Console.WriteLine();
            }
        }
        private static void PrintMenuSingleSpace(string[] menu)
        {
            int longest = menu.Max(x => x.Length);

            foreach (string s in menu)
            {
                Console.SetCursorPosition((Console.WindowWidth - longest) / 2, Console.CursorTop);

                Console.WriteLine(s);
            }
        }

        private static void PrintWelcome()
        {

            Console.BackgroundColor = backgroundColorForText;
            Console.ForegroundColor = foregroundColorForText;

            List<string> welcome = new List<string>();

            welcome.Add(":::       ::: :::::::::: :::        ::::::::   ::::::::  ::::    ::::  ::::::::::");
            welcome.Add(":+:       :+: :+:        :+:       :+:    :+: :+:    :+: +:+:+: :+:+:+ :+:       ");
            welcome.Add("+:+ +:+ +:+ + :+         +:+       +:+        +:+    +:+ +:+ :++:  +:+ +:+       ");
            welcome.Add("+#+  +:+  +#+ +#++:++#   +#+       +#+        +#+    +:+ +#+  +:+  +#+ +#++:++#  ");
            welcome.Add("+#+ +#+#+ +#+ +#+        +#+       +#+        +#+    +#+ +#+       +#+ +#+       ");
            welcome.Add(" #+#+# #+#+#  #+#        #+#       #+#    #+# #+#    #+# #+#       #+# #+#       ");
            welcome.Add("  ###   ###   ########## ########## ########   ########  ###       ### ##########");

            PrintMenuSingleSpace(welcome.ToArray());
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
        }
         
        private static void PrintCampers()
        {
            Console.BackgroundColor = backgroundColorForText;
            Console.ForegroundColor = foregroundColorForText;
            Console.SetCursorPosition(0, Console.WindowHeight - Console.WindowHeight / 3);

            List<string> campers = new List<string>();
            campers.Add(" ::::::::      :::     ::::    ::::  :::::::::  :::::::::: :::::::::   ::::::::  ");
            campers.Add(":+:    :+:   :+: :+:   +:+:+: :+:+:+ :+:    :+: :+:        :+:    :+: :+:    :+: ");
            campers.Add("+:+         +:+   +:+  +:+ +:+:+ +:+ +:+    +:+ +:+        +:+    +:+ +:+        ");
            campers.Add("+#+        +#++:++#++: +#+  +:+  +#+ +#++:++#+  +#++:++#   +#++:++#:  +#++:++#++ ");
            campers.Add("+#+        +#+     +#+ +#+       +#+ +#+        +#+        +#+    +#+        +#+ ");
            campers.Add("#+#    #+# #+#     #+# #+#       #+# #+#        #+#        #+#    #+# #+#    #+# ");
            campers.Add(" ########  ###     ### ###       ### ###        ########## ###    ###  ########  ");

            PrintMenuSingleSpace(campers.ToArray());
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
        }

        private static void PrintTrees()
        {
            Console.ForegroundColor = foregroundColorForText;
            Console.WriteLine(@"            ,@@@@@@@,                                ,@@@@@@@,                             ,@@@@@@@,                             ,@@@@@@@,                              ,@@@@@@@,                               ,@@@@@@@,                     ");
            Console.WriteLine(@"    ,,,.   ,@@@@@@/@@,  .oo8888o.           ,,,.   ,@@@@@@/@@,  .oo8888o.          ,,,.   ,@@@@@@/@@,  .oo8888o.         ,,,.   ,@@@@@@/@@,  .oo8888o.          ,,,.   ,@@@@@@/@@,  .oo8888o.           ,,,.   ,@@@@@@/@@,  .oo8888o.         ");
            Console.WriteLine(@" ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o       ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o      ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o     ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o      ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o       ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o        ");
            Console.WriteLine(@"%&&%&%&/%&&%@@\@@/ /@@@88888\88888'     %&&%&%&/%&&%@@\@@/ /@@@88888\88888'    %&&%&%&/%&&%@@\@@/ /@@@88888\88888'   %&&%&%&/%&&%@@\@@/ /@@@88888\88888'    %&&%&%&/%&&%@@\@@/ /@@@88888\88888'     %&&%&%&/%&&%@@\@@/ /@@@88888\88888'       ");
            Console.WriteLine(@"%&&%/ %&%%&&@@\ V /@@' `88\8 `/88'      %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'     %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'    %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'     %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'      %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'        ");
            Console.WriteLine(@"`&%\ ` /%&'    |.|        \ '|8'        `&%\ ` /%&'    |.|        \ '|8'       `&%\ ` /%&'    |.|        \ '|8'      `&%\ ` /%&'    |.|        \ '|8'       `&%\ ` /%&'    |.|        \ '|8'        `&%\ ` /%&'    |.|        \ '|8'          ");
            Console.WriteLine(@"    |o|        | |         | |              |o|        | |         | |             |o|        | |         | |            |o|        | |         | |             |o|        | |         | |              |o|        | |         | |            ");
            Console.WriteLine(@"    |.|        | |         | |              |.|        | |         | |             |.|        | |         | |            |.|        | |         | |             |.|        | |         | |              |.|        | |         | |            ");
            Console.WriteLine(@" \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_   \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_  \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_ \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_  \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_   \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_    ");
            Console.ForegroundColor = foregroundColor;
        }

        private static void PrintTreesBottom()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - Console.WindowHeight / 3);
            Console.ForegroundColor = foregroundColorForText;
            Console.WriteLine(@"            ,@@@@@@@,                                ,@@@@@@@,                             ,@@@@@@@,                             ,@@@@@@@,                              ,@@@@@@@,                               ,@@@@@@@,                     ");
            Console.WriteLine(@"    ,,,.   ,@@@@@@/@@,  .oo8888o.           ,,,.   ,@@@@@@/@@,  .oo8888o.          ,,,.   ,@@@@@@/@@,  .oo8888o.         ,,,.   ,@@@@@@/@@,  .oo8888o.          ,,,.   ,@@@@@@/@@,  .oo8888o.           ,,,.   ,@@@@@@/@@,  .oo8888o.         ");
            Console.WriteLine(@" ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o       ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o      ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o     ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o      ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o       ,&%%&%&&%,@@@@@/@@@@@@,8888\88/8o        ");
            Console.WriteLine(@"%&&%&%&/%&&%@@\@@/ /@@@88888\88888'     %&&%&%&/%&&%@@\@@/ /@@@88888\88888'    %&&%&%&/%&&%@@\@@/ /@@@88888\88888'   %&&%&%&/%&&%@@\@@/ /@@@88888\88888'    %&&%&%&/%&&%@@\@@/ /@@@88888\88888'     %&&%&%&/%&&%@@\@@/ /@@@88888\88888'       ");
            Console.WriteLine(@"%&&%/ %&%%&&@@\ V /@@' `88\8 `/88'      %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'     %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'    %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'     %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'      %&&%/ %&%%&&@@\ V /@@' `88\8 `/88'        ");
            Console.WriteLine(@"`&%\ ` /%&'    |.|        \ '|8'        `&%\ ` /%&'    |.|        \ '|8'       `&%\ ` /%&'    |.|        \ '|8'      `&%\ ` /%&'    |.|        \ '|8'       `&%\ ` /%&'    |.|        \ '|8'        `&%\ ` /%&'    |.|        \ '|8'          ");
            Console.WriteLine(@"    |o|        | |         | |              |o|        | |         | |             |o|        | |         | |            |o|        | |         | |             |o|        | |         | |              |o|        | |         | |            ");
            Console.WriteLine(@"    |.|        | |         | |              |.|        | |         | |             |.|        | |         | |            |.|        | |         | |             |.|        | |         | |              |.|        | |         | |            ");
            Console.WriteLine(@" \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_   \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_  \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_ \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_  \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_   \\/ ._\//_/__/  ,\_//__\\/.  \_//__/_    ");
            Console.ForegroundColor = foregroundColor;
        }
    }
}
