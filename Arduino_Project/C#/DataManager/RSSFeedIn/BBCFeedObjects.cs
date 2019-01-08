using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;
using BusinessObjects;
using DataManager.Model;
using System.IO;
using StandardUtils;

namespace RSSFeedIn
{
    // TODO: Find a more fitting place for this class to sit in
    public class Temperature
    {
        public enum Type
        {
            Celcius,
            Farenheit
        }

        private float _temperature;
        public Type measurement { get; private set; }
        
        public Temperature(float amount, Type measurement)
        {
            this._temperature = amount;
            this.measurement = measurement;
        }

        public override string ToString()
        {
            var unit = (this.measurement == Type.Celcius) ? "C" : "F";
            return $"{this._temperature}°{unit}";
        }

        public static Temperature fromCelcius(float celcius)
        {
            return new Temperature(celcius, Type.Celcius);
        }

        public static Temperature fromFarenheit(float farenheit)
        {
            return new Temperature(farenheit, Type.Farenheit);
        }

        public float asFarenheit()
        {
            if (this.measurement == Type.Farenheit)
                return this._temperature;
            else
                return (this._temperature * (9 / 5)) + 32;
        }

        public float asCelcius()
        {
            if (this.measurement == Type.Celcius)
                return this._temperature;
            else
                return (this._temperature - 32) * (5 / 9);
        }
    }

    public class BBCWeatherData
    {
        public rss_feed_result data;
        public bbc_rss_wind_direction windDirection;
        public bbc_rss_visibility visibility;
        public bbc_rss_barometric_change pressureChange;
        public bbc_rss_cloud_coverage cloudCoverage;
        public bbc_rss_pollution pollution;
    }

    class Common
    {
        public static SyndicationItem GetDataFromURL(string url, out string rawXML)
        {
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            var item = feed.Items.First();
            reader.Close();

            using (var writer = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(writer))
                {
                    feed.SaveAsRss20(xmlWriter);
                }

                rawXML = writer.ToString();
            }

            return item;
        }
    }

    public class BBCHourlyObservation
    {
        public const int InvalidNumberValue = -1000;

        /// <summary>
        /// Retrieves the weather RSS feed from a given url, parses it's data, and then returns a BBCWeatherData containing the information.
        /// </summary>
        /// <param name="url">The URL to the BBC weather RSS feed.</param>
        /// <returns>A BBCWeatherData instance, containing the information from the given URL</returns>
        public static BBCWeatherData Get(string url)
        {
            string rawXML;
            var item = Common.GetDataFromURL(url, out rawXML);

            var data                            = new BBCWeatherData();
            data.data                           = new rss_feed_result();
            data.data.date_and_time_request     = DateTime.Now;
            data.data.date_and_time_data        = item.PublishDate.DateTime;
            data.data.raw_data                  = rawXML;
            data.data.is_active                 = true;
            data.data.comment                   = "n/a";
            data.data.version                   = 1;
            data.data.forecast_max_temperature  = InvalidNumberValue;
            data.data.forecast_min_temperature  = InvalidNumberValue;
            data.data.forecast_uv_risk          = InvalidNumberValue;
            data.data.forecast_sunrise_time     = new TimeSpan(0, 0, 0);
            data.data.forecast_sunset_time      = new TimeSpan(0, 0, 0);
            data.data.forecast_pollution_id     = -1;

            // Of interest:
            // item.Summary = Temperature: 10°C (50°F), Wind Direction: Westerly, Wind Speed: 6mph, 
            //                Humidity: 79%, Pressure: 1022mb, Rising, Visibility: Very Good
            // item.Title = Tuesday - 09:00 BST: Light Cloud, 10°C (50°F)
            var titleSplit     = item.Title.Text.Split(new char[] { ':', ',' });
            data.cloudCoverage = BBCObjects.GetCloudCoverage(titleSplit[3]);

            var summaryComponents = item.Summary.Text.Split(',');
            for (int i = 0; i < summaryComponents.Length; i++)
            {
                var component = summaryComponents[i];
                var split     = component.Split(':');
                var key       = split[0].Trim();
                var value     = split[1].Trim();

                switch (key)
                {
                    case "Wind Direction":
                        data.windDirection = BBCObjects.DirectionFromString(value);
                        break;

                    case "Visibility":
                        data.visibility = BBCObjects.GetVisibility(value);
                        break;

                    case "Wind Speed":
                        var regex = new Regex("(.+)mph");
                        var matches = regex.Match(value).Groups;

                        int speed;
                        bool succeeded = Int32.TryParse(matches[1].Value, out speed);
                        if (!succeeded)
                        {
                            data.data.wind_speed = 0;
                            RSSHelper.createRSSError(rawXML, "Invalid wind speed");
                        }
                        else
                            data.data.wind_speed = speed;
                        break;

                    case "Temperature":
                        regex = new Regex("(.+)°C"); // Somehow, somewhy, the variables of other cases pass down...
                        matches = regex.Match(value).Groups;

                        if (matches[1].Value == "--")
                        {
                            data.data.observed_temperature = new Temperature(InvalidNumberValue, Temperature.Type.Celcius).asCelcius();
                            RSSHelper.createRSSError(rawXML, "No temperature was provided");
                        }
                        else
                            data.data.observed_temperature = Temperature.fromCelcius(Convert.ToSingle(matches[1].Value)).asCelcius();
                        break;

                    case "Pressure":
                        if (i == summaryComponents.Length - 1)
                            throw new Exception("Invalid data");

                        var changeString = summaryComponents[++i];
                        regex = new Regex("(.+)mb");
                        matches = regex.Match(value).Groups;

                        data.pressureChange = BBCObjects.GetBarometricChange(changeString);

                        if (matches[1].Value.Trim() == "--")
                        {
                            data.data.pressure_value = InvalidNumberValue;
                            RSSHelper.createRSSError(rawXML, "Invalid pressure value");
                        }
                        else
                            data.data.pressure_value = Convert.ToInt32(matches[1].Value);
                        break;

                    case "Humidity":
                        regex = new Regex("(\\d+)%");
                        matches = regex.Match(value).Groups;

                        data.data.humidity = Convert.ToInt32(matches[1].Value);
                        break;

                    default:
                        //throw new NotSupportedException($"Unknown component '{key}'");
                        break;
                }
            }

            return data;
        }
    }

    public class BBCThreeDayForecast
    {
        public static BBCWeatherData Get(string url)
        {
            string rawXML;
            var item = Common.GetDataFromURL(url, out rawXML);
            var data = BBCHourlyObservation.Get(url);

            var titleSplit = item.Title.Text.Split(new char[] { ':', ',' });
            var summaryComponents = item.Summary.Text.Split(',');
            for (int i = 0; i < summaryComponents.Length; i++)
            {
                var component = summaryComponents[i];
                var split = component.Split(':');
                var key = split[0].Trim();
                var value = split[1].Trim();

                switch(key)
                {
                    // TODO: Functionise this
                    case "Minimum Temperature":
                        var regex = new Regex("(.+)°C"); // Somehow, somewhy, the variables of other cases pass down...
                        var matches = regex.Match(value).Groups;

                        if (matches[1].Value == "--")
                        {
                            data.data.forecast_min_temperature = new Temperature(BBCHourlyObservation.InvalidNumberValue, Temperature.Type.Celcius).asCelcius();
                            RSSHelper.createRSSError(rawXML, "No temperature was provided");
                        }
                        else
                            data.data.forecast_min_temperature = Temperature.fromCelcius(Convert.ToSingle(matches[1].Value)).asCelcius();
                        break;

                    case "Maximum Temperature":
                        regex = new Regex("(.+)°C"); // Somehow, somewhy, the variables of other cases pass down...
                        matches = regex.Match(value).Groups;

                        if (matches[1].Value == "--")
                        {
                            data.data.forecast_max_temperature = new Temperature(BBCHourlyObservation.InvalidNumberValue, Temperature.Type.Celcius).asCelcius();
                            RSSHelper.createRSSError(rawXML, "No temperature was provided");
                        }
                        else
                            data.data.forecast_max_temperature = Temperature.fromCelcius(Convert.ToSingle(matches[1].Value)).asCelcius();
                        break;

                    case "UV Risk":
                        data.data.forecast_uv_risk = Convert.ToInt32(value.Trim());
                        break;

                    case "Pollution":
                        data.pollution = BBCObjects.GetPollution(value);
                        break;

                    case "Sunrise":
                        // TODO: Fix, the split is messing up the value.
                        data.data.forecast_sunrise_time = new TimeSpan(DateTimeOffset.Parse(value.Trim()).ToUniversalTime().UtcTicks);
                        break;

                    case "Sunset":
                        data.data.forecast_sunset_time = new TimeSpan(DateTimeOffset.Parse(value.Trim()).ToUniversalTime().UtcTicks);
                        break;
                }
            }

            return data;
        }
    }
}
