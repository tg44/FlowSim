using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace fluid.D3DrawModelsSources
{
    class Light
    {
        public Vector4 DiffuseColor { get; private set; }
        public Vector3 Direction { get; private set; }

        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColor = new Vector4(red, green, blue, alpha);
        }

        public void SetDirection(float x, float y, float z)
        {
            Direction = new Vector3(x, y, z);
        }
    }
}
