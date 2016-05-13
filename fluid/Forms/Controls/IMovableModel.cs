using System;

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

        Boolean Active { get; set; }

    }
}
