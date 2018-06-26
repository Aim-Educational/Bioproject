/// Generated by ExtensionGenerator
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Model
{
    public partial class bbc_rss_visibility : IDataModel
    {
        public bool isDeletable(PlanningContext db)
        {
                        return true;
        }

        public bool isOutOfDate(PlanningContext db)
        {
            var obj = db.bbc_rss_visibility.SingleOrDefault(d => d.bbc_rss_visibility_id == this.bbc_rss_visibility_id);

            var dbTimestamp = BitConverter.ToInt64(obj.timestamp, 0);
            var localTimestamp = BitConverter.ToInt64(this.timestamp, 0);

            return (dbTimestamp >= localTimestamp);
        }

        public bool isValidForUpdate(IncrementVersion shouldIncrement = IncrementVersion.no)
        {
            using (var db = new PlanningContext())
            {
                var obj = db.bbc_rss_visibility.SingleOrDefault(d => d.bbc_rss_visibility_id == this.bbc_rss_visibility_id);
                
                if (this.isOutOfDate(db) && obj.version <= this.version)
                {
                    if(shouldIncrement == IncrementVersion.yes)
                        this.version += 1;

                    return true;
                }

                return false;
            }
        }
    }
}
