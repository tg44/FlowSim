using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace fluid.D3DrawModelsSources
{
    class FPS
    {
        long count;
        Stopwatch watch;
        public float Value { get; private set; }

        public bool Initialize(){
            count = 0;
            Value = 0;
            watch = Stopwatch.StartNew();
            return true;
        }

        public void Frame()
        {
            count++;
            if (watch.ElapsedMilliseconds >= 1000)
            {
                Value = count;
                count = 0;
                watch.Restart();
            }
        }

        public void Dispose(){
            watch.Stop();
        }
    }
}
