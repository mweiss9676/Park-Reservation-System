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
    }
}
