using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fluid.HMDP
{
    class HMDPInformation
    {
        public string UID { get; set; }
        public bool is2D { get; set; }
        public bool is3D { get; set; }
        public string CreatorUID { get; set; }
        public string CreatorName { get; set; }
        public string CreatorDescription { get; set; }
        public string CreatorEmail { get; set; }
        public string CreatorUrl { get; set; }
        public string HardwareName { get; set; }
        public string HardwareType { get; set; }
        public string HardwareManufacture { get; set; }
        public string HardwareDescription { get; set; }
        public string HardwareUrl { get; set; }
    }
}
