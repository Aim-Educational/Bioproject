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
    public partial class Form1 : Form
    {
        const int ForecastKey     = 1;
        const int ObvservationKey = 2;

        class RSSFeed
        {
            public rss_configuration configuration;
            public DateTime nextUpdateTime;
        }

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
                    feeds.Add(new RSSFeed()
                    {
                        configuration = configuration,
                        nextUpdateTime = RSSHelper.getNextUpdateTime(configuration.last_update, 
                                                                    (float)configuration.update_frequency, 
                                                                    configuration.update_period)
                    });
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
#if DEBUG
                if(feed.configuration.rss_configuration_type_id == ObvservationKey)
                    rssData = BBCHourlyObservation.Get(feed.configuration.rss_url);
                //else if(feed.configuration.rss_configuration_type_id == ForecastKey)
                //    rssData = BBCThreeDayForecast.Get(feed.configuration.rss_url);
#else
                try
                {
                    if(feed.configuration.rss_configuration_type_id == ObvservationKey)
                        rssData = BBCHourlyObservation.Get(feed.configuration.rss_url);
                    else if(feed.configuration.rss_configuration_type_id == ForecastKey)
                        rssData = BBCThreeDayForecast.Get(feed.configuration.rss_url);
                }
                catch (Exception ex)
                {
                    // TODO: Log to file/Windows error reporter
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
#endif

                // Update the times
                feed.configuration.last_update = feed.nextUpdateTime;
                feed.nextUpdateTime = RSSHelper.getNextUpdateTime(feed.configuration.last_update,
                                                                  (float)feed.configuration.update_frequency,
                                                                  feed.configuration.update_period);

                // If rssData is still null, then an exception has been thrown.
                if (rssData == null)
                    return;

                rssData.data.source_id              = feed.configuration.rss_configuration_id;
                rssData.data.cloud_coverage_id      = rssData.cloudCoverage.bbc_rss_cloud_coverage_id;
                rssData.data.forecast_pollution_id  = (rssData.pollution == null) ? -1 
                                                                                  : rssData.pollution.bbc_rss_pollution_id;
                rssData.data.pressure_change_id     = rssData.pressureChange.bbc_rss_barometric_change_id;
                rssData.data.visibility_id          = rssData.visibility.bbc_rss_visibility_id;
                rssData.data.wind_direction_id      = rssData.windDirection.bbc_rss_wind_direction_id;

                using (var db = new PlanningContext())
                {
                    if(db.rss_feed_result.FirstOrDefault(f => f.date_and_time_data == rssData.data.date_and_time_data) == null)
                    {
                        db.rss_feed_result.Add(rssData.data);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
