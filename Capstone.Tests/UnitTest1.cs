using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Sql;
using System.Collections.Generic;
using System.Configuration;
using Capstone.DAL;
using Capstone.Menus;
using Capstone.Models;

namespace Capstone.Tests
{
    [TestClass]
    public class UnitTest1
    {
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        [TestMethod]
        public void GetCampsite_Test()
        {
            CampgroundDAL cdal = new CampgroundDAL();
            CampsiteDAL csdal = new CampsiteDAL();

            
        }
    }
}
