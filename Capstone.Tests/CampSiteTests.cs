using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Sql;
using System.Collections.Generic;
using System.Configuration;
using Capstone.DAL;
using Capstone.Menus;
using Capstone.Models;
using System.Transactions;

namespace Capstone.Tests
{
    [TestClass]
    public class CampSiteTests
    {
        // database connection for entire test class
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        // DAL objects for the entire test class
        static ViewParksDAL v = new ViewParksDAL();
        static CampgroundDAL c = new CampgroundDAL();
        static CampsiteDAL s = new CampsiteDAL();

        // park objects for the entire test class
        static Park p = v.GetParkByName("Acadia", connectionString);
        static Park p2 = v.GetParkByName("Arches", connectionString);
        static Park p3 = v.GetParkByName("Cuyahoga Valley", connectionString);

        // lists of campgrounds for the entire test class
        static List<Campground> cglist = c.GetCampgrounds(p, connectionString);// list of campgrounds for Acadia Park
        static List<Campground> cglist2 = c.GetCampgrounds(p2, connectionString);// list of campgrounds for Arches Park 
        static List<Campground> cglist3 = c.GetCampgrounds(p3, connectionString);// list of campgrounds for Cuyahoga Valley Park

        // specific campground objects for the entire test class
        static Campground cg = cglist[0];// Blackwoods Campground
        static Campground cg2 = cglist2[0];// Devil's Garden Campground
        static Campground cg3 = cglist3[0];// The Un-named Primitive Campground

        // a sample of dates to use across entire test class
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

            Assert.AreEqual(12, result);
            Assert.AreEqual(8, result2);
            Assert.AreEqual(5, result3);
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

            Assert.AreEqual(12, result);
            Assert.AreEqual(8, result2);
            Assert.AreEqual(5, result3);
        }

        [TestMethod]
        public void GetGetCampSiteAvailability_TotalCost_Test()
        {
            List<Campsite> output = s.GetCampsitesByAvailability(connectionString, cg, fromDate, toDate);
            List<Campsite> output2 = s.GetCampsitesByAvailability(connectionString, cg2, fromDate, toDate);

            Campsite cs = output[0];
            Campsite cs2 = output2[0];

            decimal result = s.CalculateCostOfReservation(cs, fromDate, toDate, connectionString);
            decimal result2 = s.CalculateCostOfReservation(cs2, fromDate, toDate, connectionString);

            Assert.AreEqual(140.00m, Decimal.Round(result, 2));
            Assert.AreEqual(100.00m, Decimal.Round(result2, 2));
        }

        [TestMethod]
        public void CreateReservation_Test()
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                s.CreateReservation(10, fromDate, toDate, "Jimmy Vanetta", connectionString);
                s.CreateReservation(1, fromDate, toDate, "Jimmy Vanetta", connectionString);

                int result = s.GetReservationID(10, connectionString);
                int result2 = s.GetReservationID(1, connectionString);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result2);
            }
        }

        [TestMethod]
        public void GetReservationID_Test()
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                s.CreateReservation(10, fromDate, toDate, "Jimmy Vanetta", connectionString);

                int result = s.GetReservationID(10, connectionString);

                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void FindReservationByName()
        {
            Reservation r = s.FindReservationByID(2, "Lockhart Family Reservation", connectionString);
            
            string dateRange = r.FromDate.ToString() + ", " + r.ToDate.ToString();
            string resName = r.Name;
            int siteId = r.SiteID;

            Assert.IsNotNull(r);
            Assert.AreEqual("Lockhart Family Reservation", resName);
            Assert.AreEqual(1, siteId);
            Assert.AreEqual("2/17/2018 12:00:00 AM, 2/20/2018 12:00:00 AM", dateRange);
        }
    }
}
