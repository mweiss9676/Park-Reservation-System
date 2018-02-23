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
    public class CampSiteTests
    {
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        static ViewParksDAL v = new ViewParksDAL();
        static CampgroundDAL c = new CampgroundDAL();
        static CampsiteDAL s = new CampsiteDAL();

        static Park p = v.GetParkByName("Acadia", connectionString);
        static Park p2 = v.GetParkByName("Arches", connectionString);
        static Park p3 = v.GetParkByName("Cuyahoga Valley", connectionString);

        static List<Campground> cglist = c.GetCampgrounds(p, connectionString);
        static List<Campground> cglist2 = c.GetCampgrounds(p2, connectionString);
        static List<Campground> cglist3 = c.GetCampgrounds(p3, connectionString);

        static Campground cg = cglist[0];
        static Campground cg2 = cglist2[0];
        static Campground cg3 = cglist3[0];

        static DateTime fromDate = new DateTime(2018, 4, 24);
        static DateTime toDate = new DateTime(2018, 4, 28);

        [TestMethod]
        public void GetCampSites_CountList_Test()
        {
            List<Campsite> output = s.GetCampsites(cg, connectionString);
            List<Campsite> output2 = s.GetCampsites(cg2, connectionString);
            List<Campsite> output3 = s.GetCampsites(cg3, connectionString);

            int result = output.Count;
            int result2 = output2.Count;
            int result3 = output3.Count;

            Assert.AreEqual(result, 12);
            Assert.AreEqual(result2, 8);
            Assert.AreEqual(result3, 5);
        }

        [TestMethod]
        public void GetCampSiteAvailablity_Test()
        {
            List<Campsite> output = s.GetCampsitesByAvailability(connectionString, cg, fromDate, toDate);
            List<Campsite> output2 = s.GetCampsitesByAvailability(connectionString, cg2, fromDate, toDate);
            List<Campsite> output3 = s.GetCampsitesByAvailability(connectionString, cg3, fromDate, toDate);

            int result = output.Count;
            int result2 = output2.Count;
            int result3 = output3.Count;

            Assert.AreEqual(result, 14);
            Assert.AreEqual(result2, 10);
            Assert.AreEqual(result3, 5);
        }

        [TestMethod]
        public void GetGetCampSiteAvailability_TotalCost_Test()
        {
            List<Campsite> output = s.GetCampsitesByAvailability(connectionString, cg, fromDate, toDate);

            Campsite cs = output[0];

            decimal result = s.CalculateCostOfReservation(cs, fromDate, toDate, connectionString);

            Assert.AreEqual(result, 140.00m);
        }
    }
}
