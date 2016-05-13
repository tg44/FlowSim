using System.Drawing;

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

        public bool Dust { get; set; }
    }
}
