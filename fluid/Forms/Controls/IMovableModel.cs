using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fluid.Forms
{
    public interface IMovableModel
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }

        double R { get; set; }

        string Name { get; set; }

        bool is2D();

    }
}
