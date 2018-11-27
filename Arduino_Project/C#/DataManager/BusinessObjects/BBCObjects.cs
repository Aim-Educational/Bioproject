using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataManager.Model;

namespace BusinessObjects
{
    public static class BBCObjects
    {
        public const string RSSObservationDeviceName = "BBC Hourly Observation";
        public const string RSSForecastDeviceName    = "BBC Three-day forecast";
        public const int    RecordNotFoundID         = -1;

        public static bbc_rss_visibility GetVisibility(string visiblity, bool createIfMissing = true)
        {
            visiblity = visiblity.ToUpper().Trim();
            using (var db = new PlanningContext())
            {
                var query = from db_visiblity in db.bbc_rss_visibility
                            where db_visiblity.description.ToUpper() == visiblity
                            select db_visiblity;

                var result = query.FirstOrDefault();
              
                if (result == null && createIfMissing)
                {
                    var toAdd = new bbc_rss_visibility
                    {
                        description = visiblity,
                        version = 1,
                        is_active = true,
                        comment = visiblity
                    };

                    db.bbc_rss_visibility.Add(toAdd);
                    db.SaveChanges();

                    result = toAdd;
                }
                
                return result;
            }
        }

        public static bbc_rss_barometric_change GetBarometricChange(string change, bool createIfMissing = true)
        {
            change = change.ToUpper().Trim();
            using (var db = new PlanningContext())
            {
                var query = from db_change in db.bbc_rss_barometric_change
                            where db_change.description == change
                            select db_change;

                var result = query.FirstOrDefault();

                if (result == null && createIfMissing)
                {
                    var toAdd = new bbc_rss_barometric_change
                    {
                        description = change,
                        version = 1,
                        is_active = true,
                        comment = change
                    };

                    db.bbc_rss_barometric_change.Add(toAdd);
                    db.SaveChanges();

                    result = toAdd;
                }

                return result;
            }
        }

        public static bbc_rss_cloud_coverage GetCloudCoverage(string coverage, bool createIfMissing = true)
        {
            coverage = coverage.ToUpper().Trim();
            using (var db = new PlanningContext())
            {
                var query = from db_coverage in db.bbc_rss_cloud_coverage
                            where db_coverage.description == coverage
                            select db_coverage;

                var result = query.FirstOrDefault();
                if (result == null && createIfMissing)
                {
                    var toAdd = new bbc_rss_cloud_coverage
                    {
                        description = coverage,
                        version = 1,
                        is_active = true,
                        comment = coverage
                    };

                    db.bbc_rss_cloud_coverage.Add(toAdd);
                    db.SaveChanges();
                    result = toAdd;
                }

                return result;
            }
        }

        public static bbc_rss_pollution GetPollution(string coverage, bool createIfMissing = true)
        {
            coverage = coverage.ToUpper().Trim();
            using (var db = new PlanningContext())
            {
                var query = from db_coverage in db.bbc_rss_pollution
                            where db_coverage.description == coverage
                            select db_coverage;

                var result = query.FirstOrDefault();
                if (result == null && createIfMissing)
                {
                    var toAdd = new bbc_rss_pollution
                    {
                        description = coverage,
                        version = 1,
                        is_active = true,
                        comment = coverage
                    };

                    db.bbc_rss_pollution.Add(toAdd);
                    db.SaveChanges();
                    result = toAdd;
                }

                return result;
            }
        }

        public static bbc_rss_wind_direction DirectionFromString(string direction)
        {
            // South Easterly
            // SouthEast
            direction = direction.Replace("erly", "");
            direction = direction.Replace(" ", "");

            using(var db = new PlanningContext())
            {
                var result = db.bbc_rss_wind_direction.FirstOrDefault(dir => dir.description == direction);
                if (result != null)
                    return result;

                var notFound = db.bbc_rss_wind_direction.FirstOrDefault(dir => dir.bbc_rss_wind_direction_id == RecordNotFoundID);
                if (notFound != null)
                    return notFound;
            }

            throw new Exception("Direction not found, and default record not found.");
        }

        public static device GetRSSObservationDevice()
        {
            return GetDevice(RSSObservationDeviceName);
        }

        public static device GetRSSForecastDevice()
        {
            return GetDevice(RSSForecastDeviceName);
        }

        private static device GetDevice(string name)
        {
            using (var db = new PlanningContext())
            {
                var query = from db_device in db.devices
                            where db_device.name == name
                            select db_device;

                var result = query.FirstOrDefault();

                if (result == null)
                    throw new Exception($"Unable to find RSS device, name '{RSSObservationDeviceName}'");

                return result;
            }
        }
    }
}
