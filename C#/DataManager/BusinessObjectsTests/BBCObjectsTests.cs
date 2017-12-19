using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Tests
{
    [TestClass()]
    public class BBCObjectsTests
    {
        [TestMethod()]
        public void GetVisibilityTest()
        {
            var result = BBCObjects.GetVisibility("Good");
            Assert.AreEqual(result.description, "Good");
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void GetBarometricChangeTest()
        {
            var result = BBCObjects.GetBarometricChange("Falling");
            Assert.AreEqual(result.description, "Falling");
            Assert.Inconclusive();
        }
    }
}