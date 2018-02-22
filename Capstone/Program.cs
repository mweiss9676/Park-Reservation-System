using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.CLI;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
            // Use this so that you don't need to copy your connection string all over your code!            
            // ConfigurationManager opens up the App.config file and looks for an entry called "CapstoneDatabase".
            //     <add name="CapstoneDatabase" connectionString=""/>
            // The actual connection string for the database is found connectionString attribute.            
            string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        }
    }
}
