using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fluid.HMDP.Preset
{
    class PresetItem
    {

        public string File
        {
            get { return file; }
            set { file = value; Loader = new HMDPLoader() { FileName = value }; }
        }
        private string file;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Rotation { get; set; }

        public HMDPLoader Loader { get; set; }

        public bool Processed { get; set; }


    }
}
