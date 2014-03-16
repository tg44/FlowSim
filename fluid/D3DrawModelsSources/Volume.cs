using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX;
using System.IO;

namespace fluid.D3DrawModelsSources
{
    class Volume
    {
        //private float[,,] Data;
        //private Texture3D VolumeTexture;

        public void createTest(Device device)
        {
            /*
            Data = new float[1000, 1000, 1000];
            //byte[] ddd = (byte[])Data;
            DataBox dbox;
            Texture3DDescription desc = new Texture3DDescription();
            desc.Width = 1000;
            desc.Height = 1000;
            desc.Depth = 1000;
            desc.MipLevels = 0;
            VolumeTexture = device.CreateTexture3D();
            VolumeTexture = new Texture3D(device, desc);
            //var asd = Texture3D.FromMemory(device, Data);
            //VolumeTexture.
            var byteArray = new byte[Data.Length * 4];
            System.Buffer.BlockCopy(Data, 0, byteArray, 0, byteArray.Length);
            Stream stream = new MemoryStream(byteArray);

            var asd = Texture3D.FromStream(device,stream,byteArray.Length);
            */

            Texture t=new Texture();
            if(!t.Initialize(device, "teapot.raw"))
                Console.Write("3d objectum lad hiba");





        }
    }
}
