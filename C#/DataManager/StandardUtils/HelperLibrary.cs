using System;
using System.Configuration;
using System.Net;

using DataManager;

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

        public static UpdatePeriod.Period getUpdatePeriodEnumByID(int id)
        {
            var idAsEnum = (UpdatePeriod.Period)id;

            if (!Enum.IsDefined(typeof(UpdatePeriod.Period), idAsEnum))
                throw new ArgumentOutOfRangeException("id");

            return idAsEnum;
        }

        public static DateTime getNextUpdateTime(DateTime lastUpdate, float frequency, UpdatePeriod.Period updatePeriod)
        {
            switch(updatePeriod)
            {
                case UpdatePeriod.Period.Second:
                    return lastUpdate.AddSeconds(frequency);

                case UpdatePeriod.Period.Minute:
                    return lastUpdate.AddMinutes(frequency);

                case UpdatePeriod.Period.Hour:
                    return lastUpdate.AddHours(frequency);

                case UpdatePeriod.Period.Day:
                    return lastUpdate.AddDays(frequency);

                case UpdatePeriod.Period.Week:
                    return lastUpdate.AddDays(7 * frequency);

                case UpdatePeriod.Period.Month:
                    var months = (int)Math.Round(frequency);
                    return lastUpdate.AddMonths(months);

                case UpdatePeriod.Period.Year:
                    var years = (int)Math.Round(frequency);
                    return lastUpdate.AddYears(years);

                default:
                    throw new Exception($"You got here via black magic. Value = {updatePeriod}");
            }
        }
    }
}