/// Generated by ExtensionGenerator
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Model
{
    public partial class device_history_action : IDataModel
    {
        public bool isOutOfDate(PlanningContext db)
        {
            var obj = db.device_history_action.SingleOrDefault(d => d.device_history_action1 == this.device_history_action1);

            var dbTimestamp = BitConverter.ToInt64(obj.timestamp, 0);
            var localTimestamp = BitConverter.ToInt64(this.timestamp, 0);

            return (dbTimestamp > localTimestamp);
        }

        public bool isValidForUpdate(IncrementVersion shouldIncrement = IncrementVersion.no)
        {
            using (var db = new PlanningContext())
            {
                var obj = db.device_history_action.SingleOrDefault(d => d.device_history_action1 == this.device_history_action1);
                
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