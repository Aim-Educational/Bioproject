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
        public const string RSSDeviceName = "BBC Hourly Observation";

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

        public static device GetRSSDevice()
        {
            using (var db = new PlanningContext())
            {
                var query = from db_device in db.devices
                            where db_device.name == RSSDeviceName
                            select db_device;

                var result = query.FirstOrDefault();

                if (result == null)
                    throw new Exception($"Unable to find RSS device, name '{RSSDeviceName}'");

                return result;
            }
        }
    }
}
