using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace fluid.HMDP
{
    class HMDP2D
    {
        public int x { get; set; }
        public int y { get; set; }

        public Image Icon { get; set; }
        public Image Heat { get; set; }
        public Image Throughput { get; set; }
        public Image Solid { get; set; }
    }
}
