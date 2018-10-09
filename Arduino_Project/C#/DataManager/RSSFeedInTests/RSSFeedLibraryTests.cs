using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSSFeedIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedIn.Tests
{
    [TestClass()]
    public class RSSFeedLibraryTests
    {
        [TestMethod()]
        public void DirectionFromStringTest()
        {
            Assert.AreEqual(WindDirection.WestNorthWest, RSSFeedLibrary.DirectionFromString("West North Westerly"));

            try
            {
                RSSFeedLibrary.DirectionFromString("asfasd");
                Assert.Fail("No exception was thrown.");
            }
            catch(NotSupportedException ex)
            {
                Assert.IsTrue(ex.Message.Contains("asfasd"));
            }
        }
    }
}