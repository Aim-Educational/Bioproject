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
