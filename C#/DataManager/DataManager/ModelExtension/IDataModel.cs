using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Model
{
    public enum IncrementVersion
    {
        yes,
        no
    }

    public interface IDataModel
    {
        bool isValidForUpdate(IncrementVersion shouldIncrement = IncrementVersion.no);
        bool isOutOfDate(PlanningContext db);
    }
}
