using System;
using System.Configuration;
using System.Net;
using System.Linq;

using DataManager;
using DataManager.Model;

namespace StandardUtils
{
    public static class RSSHelper
    {
        public static bool isConnectedToTheInternet()
        {
            // TODO: Cacheing?
            // TODO: Attempt multiple checks if one fails?

            var testURL = ConfigurationManager.AppSettings["connectionTestURL"];
            if (testURL == null)
                throw new Exception("No key called 'connectionTestURL' in App.config *(make sure it's in the right one)*");

            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead(testURL))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DateTime getNextUpdateTime(DateTime lastUpdate, float frequency, update_period updatePeriod)
        {
            switch(updatePeriod.description)
            {
                case "Second":
                    return lastUpdate.AddSeconds(frequency);

                case "Minute":
                    return lastUpdate.AddMinutes(frequency);

                case "Hour":
                    return lastUpdate.AddHours(frequency);

                case "Day":
                    return lastUpdate.AddDays(frequency);

                case "Week":
                    return lastUpdate.AddDays(7 * frequency);

                case "Month":
                    var months = (int)Math.Round(frequency);
                    return lastUpdate.AddMonths(months);

                case "Year":
                    var years = (int)Math.Round(frequency);
                    return lastUpdate.AddYears(years);

                default:
                    throw new Exception($"You got here via black magic. Value = {updatePeriod}");
            }
        }

        public static void createRSSError(string rawXML, string message)
        {
            using (var db = new PlanningContext())
            {
                var result = db.rss_error.FirstOrDefault(e => e.data == rawXML && e.message == message);
                if (result != null)
                    return;

                var error = new rss_error();
                error.date_and_time = DateTime.Now;
                error.data = rawXML;
                error.message = message;
                error.comment = "N/A";
                error.version = 1;
                error.is_active = true;

                db.rss_error.Add(error);
                db.SaveChanges();
            }
        }
    }
}