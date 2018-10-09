using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataManager;
using DataManager.Model;
using BusinessObjects;
using RSSFeedIn;
using StandardUtils;

namespace RSSUserInterface
{
    class RSSFeed
    {
        public string description;
        public DateTime lastUpdateTime;
        public DateTime nextUpdateTime;
        public float frequency;
        public UpdatePeriod.Period updatePeriod;
        public string rssURL;
    }

    public partial class Form1 : Form
    {
        List<RSSFeed> feeds;

        public Form1()
        {
            InitializeComponent();

            setupFeeds();
        }

        private void setupFeeds()
        {
            feeds = new List<RSSFeed>();

            using (var context = new PlanningContext())
            {
                var query = from config in context.rss_configuration
                            where config.is_active
                            select config;

                foreach(var configuration in query)
                {
                    var feed = new RSSFeed
                    {
                        description = configuration.description,
                        lastUpdateTime = configuration.last_update,
                        frequency = (float)configuration.update_frequency,
                        updatePeriod = RSSHelper.getUpdatePeriodEnumByID(configuration.update_period_id),
                        rssURL = configuration.rss_url
                    };
                    feed.nextUpdateTime = RSSHelper.getNextUpdateTime(feed.lastUpdateTime, feed.frequency, feed.updatePeriod);
                    feeds.Add(feed);
                }
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonProcessNow_Click(object sender, EventArgs e)
        {
            // TODO: Make this work, might work now
            checkFeeds();
        }

        private void timerDoProcess_Tick(object sender, EventArgs e)
        {
            checkFeeds();
        }

        private void checkFeeds()
        {
            timerDoProcess.Stop();
            foreach (var feed in feeds)
                processFeed(feed);

            timerDoProcess.Start();
        }

        private void processFeed(RSSFeed feed)
        {
            if (DateTime.Now >= feed.nextUpdateTime)
            {
                // TODO: Get URL from database? (Or are we just using the one from the textbox)
                // Get the current RSS data.
                BBCWeatherData rssData = null;
                try
                {
                    rssData = BBCHourlyObservation.Get(feed.rssURL);
                }
                catch (Exception ex)
                {
                    // TODO: Log to file/Windows error reporter
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // If rssData is still null, then an exception has been thrown.
                // Regardless of whether the data was retrieved or not, the code at the bottom needs to be ran.
                // Hence this if statement.
                if (rssData != null)
                {
                    // TODO: Process
                    MessageBox.Show($"Feed Description: {feed.description} | Wind Direction: {rssData.windDirection} | Wind Speed: {rssData.windSpeed} | Visibility: {rssData.visibility.description}", "Debug");
                } // What next?

                // Update the times
                feed.lastUpdateTime = feed.nextUpdateTime;
                feed.nextUpdateTime = RSSHelper.getNextUpdateTime(feed.lastUpdateTime, feed.frequency, feed.updatePeriod);
            }
            // TODO: May have to think about asynchronous processing.
        }
    }
}
