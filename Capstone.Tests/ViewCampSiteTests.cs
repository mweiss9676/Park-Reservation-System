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
    public class ViewCampSiteTests
    {
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        [TestMethod]
        public void GetCampSites_CountList_Test()
        {
            ViewParksDAL v = new ViewParksDAL();
            CampgroundDAL c = new CampgroundDAL();
            CampsiteDAL s = new CampsiteDAL();

            Park p = v.GetParkByName("Acadia", connectionString);
            Park p2 = v.GetParkByName("Arches", connectionString);
            Park p3 = v.GetParkByName("Cuyahoga Valley", connectionString);

            List<Campground> cglist = c.GetCampgrounds(p, connectionString);
            List<Campground> cglist2 = c.GetCampgrounds(p2, connectionString);
            List<Campground> cglist3 = c.GetCampgrounds(p3, connectionString);


            Campground cg = cglist[0];
            Campground cg2 = cglist2[0];
            Campground cg3 = cglist3[0];


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
    }
}
