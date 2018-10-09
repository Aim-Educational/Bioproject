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
    public class BBCHourlyObvsTests
    {
        [TestMethod()]
        public void GetTest()
        {
            BBCHourlyObservation.Get("http://open.live.bbc.co.uk/weather/feeds/en/2656848/observations.rss");
            Assert.Inconclusive("This isn't an actual test... yet");
        }
    }
}