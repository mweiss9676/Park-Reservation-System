using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Capstone.DAL;
using Capstone.Menus;
using Capstone.Models;


namespace Capstone.Tests
{
    [TestClass]
    public class ParksTests
    {
        // DAL objects for the entire test class
        static ParkDAL v = new ParkDAL();

        // database connection for entire test class
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        [TestMethod]
        public void DoesParkExist_Test()
        {
            bool result = v.DoesParkExist("Acadia", connectionString);
            bool result1 = v.DoesParkExist("Arches", connectionString);
            bool result2 = v.DoesParkExist("Dingle Berry", connectionString);

            Assert.AreEqual(true, result);
            Assert.AreEqual(true, result1);
            Assert.AreEqual(false, result2);
        }

        [TestMethod]
        public void GetParkByName_Test()
        {
            Park p = v.GetParkByName("Acadia", connectionString);
            Park a = v.GetParkByName("Arches", connectionString);

            Assert.AreEqual("Acadia", p.Name);
            Assert.AreEqual(76518, a.Area);
        }
        [TestMethod]
        public void ViewParkInformation_Test()
        {
            List<string> output = v.ViewParkInformation("Acadia", connectionString);

            string park = output[0];
            string location = output[1];
            string establishDate = output[2];
            string area = output[3];
            string annualVisitors = output[4];

            Assert.AreEqual("Acadia", park);
            Assert.AreEqual("Maine", location);
            Assert.AreEqual("2/26/1919 12:00:00 AM", establishDate);
            Assert.AreEqual("47389", area);
            Assert.AreEqual("2563129", annualVisitors);
        }
    }
}
