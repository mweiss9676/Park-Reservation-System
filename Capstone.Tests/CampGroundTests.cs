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
    public class CampGroundTests
    {
        // database connection for entire test class
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        // DAL objects for the entire test class
        static ParkDAL v = new ParkDAL();
        static CampgroundDAL c = new CampgroundDAL();

        // park objects for the entire test class
        static Park p = v.GetParkByName("Acadia", connectionString);
        static Park p2 = v.GetParkByName("Arches", connectionString);
        static Park p3 = v.GetParkByName("Cuyahoga Valley", connectionString);

        // lists of campgrounds for the entire test class
        static List<Campground> cg = c.GetCampgrounds(p, connectionString);// list of campgrounds for Acadia Park
        static List<Campground> cg2 = c.GetCampgrounds(p2, connectionString);// list of campgrounds for Arches Park
        static List<Campground> cg3 = c.GetCampgrounds(p3, connectionString);// list of campgrounds for Cuyahoga Valley Park

        // a sample of dates to use across entire test class
        static DateTime fromDate = new DateTime(2018, 4, 24);// start date
        static DateTime toDate = new DateTime(2018, 4, 28);// end date

        [TestMethod]
        public void GetCampGrounds_CountList_Test()
        {
            int result = cg.Count;
            int result2 = cg2.Count;
            int result3 = cg3.Count;

            Assert.AreEqual(3, result);
            Assert.AreEqual(3, result2);
            Assert.AreEqual(1, result3);
        }

        [TestMethod]
        public void VerifyAcadiaCampGroundNames_Test()
        {
            string result = cg[0].Name;
            string result2 = cg[1].Name;
            string result3 = cg[2].Name;

            Assert.AreEqual("Blackwoods", result);
            Assert.AreEqual("Seawall", result2);
            Assert.AreEqual("Schoodic Woods", result3);
        }

        [TestMethod]
        public void VerifyArchesCampGroundNames_Test()
        {
            string result = cg2[0].Name;
            string result2 = cg2[1].Name;
            string result3 = cg2[2].Name;

            Assert.AreEqual("Devil's Garden", result);
            Assert.AreEqual("Canyon Wren Group Site", result2);
            Assert.AreEqual("Juniper Group Site", result3);
        }

        [TestMethod]
        public void VerifyCuyahogaValleyCampGroundName_Test()
        {
            string result = cg3[0].Name;

            Assert.AreEqual(result, "The Unnamed Primitive Campsites");
        }
        [TestMethod]
        public void GetCampGroundByName_Test()
        {
            Campground campground = c.GetCampgroundByName("Blackwoods", connectionString);
            Campground campground2 = c.GetCampgroundByName("Juniper Group Site", connectionString);
            Campground campground3 = c.GetCampgroundByName("The Unnamed Primitive Campsites", connectionString);

            // asserting campground
            Assert.AreEqual(1, campground.CampgroundID);
            Assert.AreEqual(35m, campground.DailyFee);
            Assert.AreEqual(1, campground.OpenFromDate);
            Assert.AreEqual(12, campground.OpenToDate);

            // asserting campground2
            Assert.AreEqual(6, campground2.CampgroundID);
            Assert.AreEqual(250m, campground2.DailyFee);
            Assert.AreEqual(1, campground2.OpenFromDate);
            Assert.AreEqual(12, campground2.OpenToDate);

            // asserting campground3
            Assert.AreEqual(7, campground3.CampgroundID);
            Assert.AreEqual(20m, campground3.DailyFee);
            Assert.AreEqual(5, campground3.OpenFromDate);
            Assert.AreEqual(11, campground3.OpenToDate);
        }
        
        [TestMethod]
        public void IsCampGroundOpen_Tests()
        {
            Campground campground = c.GetCampgroundByName("Blackwoods", connectionString);
            Campground campground2 = c.GetCampgroundByName("Schoodic Woods", connectionString);
            Campground campground3 = c.GetCampgroundByName("Seawall", connectionString);
            Campground campground4 = c.GetCampgroundByName("Canyon Wren Group Site", connectionString);

            bool result = c.IsTheCampgroundOpen(campground, fromDate, toDate, connectionString);
            bool result2 = c.IsTheCampgroundOpen(campground2, fromDate, toDate, connectionString);
            bool result3 = c.IsTheCampgroundOpen(campground3, fromDate, toDate, connectionString);
            bool result4 = c.IsTheCampgroundOpen(campground4, fromDate, toDate, connectionString);

            Assert.AreEqual(true, result);
            Assert.AreEqual(false, result2);
            Assert.AreEqual(false, result3);
            Assert.AreEqual(true, result4);
        }
    }
}
