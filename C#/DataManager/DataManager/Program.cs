using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager.Model;

namespace DataManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // Should we add, then read in some test data, just to see if it works?
            using (var db = new PlanningContext())
            {
                var query = from device in db.devices select device;

                foreach(var device in query)
                {
                    Console.WriteLine(device.device_type.description);
                }

                Console.ReadKey();
            }
        }
    }
}
