using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Kp
{
    class ContactData
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public Geopoint location { get; set; }
        public string Pic { get; set; }
    }
}
