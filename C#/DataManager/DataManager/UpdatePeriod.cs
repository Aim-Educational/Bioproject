using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataManager.Model;

namespace DataManager
{
    public class UpdatePeriod : update_period
    {
        public enum Period
        {
            Second = 1,
            Minute = 2,
            Week = 3,
            Month = 4,
            Year = 5,
            Hour = 7,
            Day = 8
        }
    }
}
