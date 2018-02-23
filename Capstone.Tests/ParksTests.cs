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
        static ViewParksDAL v = new ViewParksDAL();

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
    }
}
