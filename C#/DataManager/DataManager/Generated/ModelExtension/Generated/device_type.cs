/// Generated by ExtensionGenerator
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Model
{
    public partial class device_type : IDataModel
    {
        public bool isDeletable(PlanningContext db)
        {
                        var query0 = from obj in db.devices
                         where obj.device_type_id == this.device_type_id
                         select obj;

            return query0.Count() == 0;
        }

        public bool isOutOfDate(PlanningContext db)
        {
            var obj = db.device_type.SingleOrDefault(d => d.device_type_id == this.device_type_id);

            var dbTimestamp = BitConverter.ToInt64(obj.timestamp, 0);
            var localTimestamp = BitConverter.ToInt64(this.timestamp, 0);

            return (dbTimestamp >= localTimestamp);
        }

        public bool isValidForUpdate(IncrementVersion shouldIncrement = IncrementVersion.no)
        {
            using (var db = new PlanningContext())
            {
                var obj = db.device_type.SingleOrDefault(d => d.device_type_id == this.device_type_id);
                
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

