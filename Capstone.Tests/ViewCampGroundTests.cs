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
    public class ViewCampGroundTests
    {
        static string connectionString = @"Server=.\SQLEXPRESS;Database=NationalParks;Trusted_Connection=True";

        [TestMethod]
        public void GetCampGrounds_CountList_Test()
        {
            ViewParksDAL v = new ViewParksDAL();
            CampgroundDAL c = new CampgroundDAL();

            Park p = v.GetParkByName("Acadia", connectionString);
            Park p2 = v.GetParkByName("Arches", connectionString);
            Park p3 = v.GetParkByName("Cuyahoga Valley", connectionString);

            List<Campground>cg = c.GetCampgrounds(p, connectionString);
            List<Campground> cg2 = c.GetCampgrounds(p2, connectionString);
            List<Campground> cg3 = c.GetCampgrounds(p3, connectionString);

            int result = cg.Count;
            int result2 = cg2.Count;
            int result3 = cg3.Count;

            Assert.AreEqual(result, 3);
            Assert.AreEqual(result2, 3);
            Assert.AreEqual(result3, 1);
        }
    }
}
