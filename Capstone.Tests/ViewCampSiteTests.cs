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

            List<Campground> cglist = c.GetCampgrounds(p, connectionString);

            Campground cg = cglist[0];

            List<Campsite> output = s.GetCampsites(cg, connectionString);

            int result = output.Count;

            Assert.AreEqual(result, 12);
        }
    }
}
