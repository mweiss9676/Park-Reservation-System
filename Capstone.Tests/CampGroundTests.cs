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
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        static ViewParksDAL v = new ViewParksDAL();
        static CampgroundDAL c = new CampgroundDAL();

        static Park p = v.GetParkByName("Acadia", connectionString);
        static Park p2 = v.GetParkByName("Arches", connectionString);
        static Park p3 = v.GetParkByName("Cuyahoga Valley", connectionString);

        static List<Campground> cg = c.GetCampgrounds(p, connectionString);
        static List<Campground> cg2 = c.GetCampgrounds(p2, connectionString);
        static List<Campground> cg3 = c.GetCampgrounds(p3, connectionString);

        static DateTime fromDate = new DateTime(2018, 4, 24);
        static DateTime toDate = new DateTime(2018, 4, 28);

        [TestMethod]
        public void GetCampGrounds_CountList_Test()
        {
            int result = cg.Count;
            int result2 = cg2.Count;
            int result3 = cg3.Count;

            Assert.AreEqual(result, 3);
            Assert.AreEqual(result2, 3);
            Assert.AreEqual(result3, 1);
        }

        [TestMethod]
        public void VerifyAcadiaCampGroundNames_Test()
        {
            string result = cg[0].Name;
            string result2 = cg[1].Name;
            string result3 = cg[2].Name;

            Assert.AreEqual(result, "Blackwoods");
            Assert.AreEqual(result2, "Seawall");
            Assert.AreEqual(result3, "Schoodic Woods");
        }

        [TestMethod]
        public void VerifyArchesCampGroundNames_Test()
        {
            string result = cg2[0].Name;
            string result2 = cg2[1].Name;
            string result3 = cg2[2].Name;

            Assert.AreEqual(result, "Devil's Garden");
            Assert.AreEqual(result2, "Canyon Wren Group Site");
            Assert.AreEqual(result3, "Juniper Group Site");
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

            // asserting first campground
            Assert.AreEqual(campground.CampgroundID, 1);
            Assert.AreEqual(campground.DailyFee, 35m);
            Assert.AreEqual(campground.OpenFromDate, 1);
            Assert.AreEqual(campground.OpenToDate, 12);

            // asserting campground2
            Assert.AreEqual(campground2.CampgroundID, 6);
            Assert.AreEqual(campground2.DailyFee, 250m);
            Assert.AreEqual(campground2.OpenFromDate, 1);
            Assert.AreEqual(campground2.OpenToDate, 12);

            // asserting campground3
            Assert.AreEqual(campground3.CampgroundID, 7);
            Assert.AreEqual(campground3.DailyFee, 20m);
            Assert.AreEqual(campground3.OpenFromDate, 5);
            Assert.AreEqual(campground3.OpenToDate, 11);
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

            Assert.AreEqual(result, true);
            Assert.AreEqual(result2, false);
            Assert.AreEqual(result3, false);
            Assert.AreEqual(result4, true);
        }
    }
}
