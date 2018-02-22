using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone;

namespace Capstone.CLI
{
    public class CLI
    {
        public static void MainMenu()
        {
            string[] menu = { "Main Menu", "1) View Parks" };
            PrintMenu(menu);
        }

        private static void PrintMenu(string[] menu)
        {
            foreach(string s in menu)
            {
                Console.WriteLine();
                Console.WriteLine(s);
                Console.WriteLine();
            }
        }
    }
}
