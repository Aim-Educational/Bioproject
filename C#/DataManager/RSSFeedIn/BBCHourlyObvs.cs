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

namespace RSSFeedIn
{
    /// <summary>
    /// Describes the direction of the wind.
    /// </summary>
    public enum WindDirection : int
    {
        North = 0,

        NorthEast = 45,
        East = 90,
        SouthEast = 135,

        South = 180,

        NorthWest = 315,
        West = 270,
        SouthWest = 225,

        NorthNorthEast = 22,
        EastNorthEast = 67,
        WestNorthWest = 292,
        NorthNorthWest = 337,
        EastSouthEast = 112,
        SouthSouthEast = 157,
        SouthSouthWest = 202,
        WestSouthWest = 247
    }

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
        public DateTime dateTime;
        public WindDirection windDirection;
        public bbc_rss_visibility visibility;
        public bbc_rss_barometric_change pressureChange;
        public int windSpeed; // mph
        public Temperature temperature;
        /* TODO: Finish reading in these values
         * Humidity
         * Pressure
         * General Description
         * Geo RSS point
         * */
    }

    public class BBCHourlyObservation
    {
        /// <summary>
        /// Retrieves the weather RSS feed from a given url, parses it's data, and then returns a BBCWeatherData containing the information.
        /// </summary>
        /// <param name="url">The URL to the BBC weather RSS feed.</param>
        /// <returns>A BBCWeatherData instance, containing the information from the given URL</returns>
        public static BBCWeatherData Get(string url)
        {
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            var data = new BBCWeatherData();
            
            foreach(var item in feed.Items)
            {
                // Of interest:
                // item.Summary = Temperature: 10°C (50°F), Wind Direction: Westerly, Wind Speed: 6mph, 
                //                Humidity: 79%, Pressure: 1022mb, Rising, Visibility: Very Good
                // item.Title = Tuesday - 09:00 BST: Light Cloud, 10°C (50°F)
                var summaryComponents = item.Summary.Text.Split(',');

                foreach(var component in summaryComponents)
                {
                    var split = component.Split(':');
                    var key = split[0].Trim();

                    if (split.Length == 1)
                        data.pressureChange = BBCObjects.GetBarometricChange(key);
                    else
                    {
                        var value = split[1].Trim();
                        switch(key)
                        {
                            case "Wind Direction":
                                data.windDirection = RSSFeedLibrary.DirectionFromString(value);
                                break;

                            case "Visibility":
                                data.visibility = BBCObjects.GetVisibility(value);
                                break;

                            case "Wind Speed":
                                var regex = new Regex("(\\d+)mph");
                                var matches = regex.Match(value).Groups;
                                
                                data.windSpeed = Convert.ToInt32(matches[1].Value);
                                break; // How're we gonna store all of the other pieces of data.

                            case "Temperature":
                                regex = new Regex("(\\d+)°C"); // Somehow, somewhy, the variables of other cases pass down...
                                matches = regex.Match(value).Groups;

                                data.temperature = Temperature.fromCelcius(Convert.ToSingle(matches[1].Value));
                                break;

                            default:
                                // TODO: Make sure to uncomment this once all of the data has been added
                                //throw new NotSupportedException($"Unknown component '{key}'");
                                break;
                        }
                    }
                } // Next week.
            }

            return data;
        }
    }
}
