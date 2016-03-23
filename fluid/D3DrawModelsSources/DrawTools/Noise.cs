using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fluid.D3DrawModelsSources.DrawTools
{
    class Noise
    {
        private static Vector4 add(Vector4 v, float s)
        {
            return new Vector4(v.X + s, v.Y + s, v.Z + s, v.W + s);
        }

        private static Vector4 saturate(Vector4 v)
        {
            return new Vector4(v.X > 1 ? 1 : v.X < 0 ? 0 : v.X, v.Y > 1 ? 1 : v.Y < 0 ? 0 : v.Y, v.Z > 1 ? 1 : v.Z < 0 ? 0 : v.Z, v.W > 1 ? 1 : v.W < 0 ? 0 : v.W);
        }
        private static Vector4 grad4(float j, Vector4 ip)
        {
            Vector4 ones = new Vector4(1.0f, 1.0f, 1.0f, -1.0f);
            Vector4 p, s;
            p.X = (float)Math.Floor((j * ip.X - Math.Floor(j * ip.X)) * 7.0f) * ip.Z - 1.0f;
            p.Y = (float)Math.Floor((j * ip.Y - Math.Floor(j * ip.Y)) * 7.0f) * ip.Z - 1.0f;
            p.Z = (float)Math.Floor((j * ip.Z - Math.Floor(j * ip.Z)) * 7.0f) * ip.Z - 1.0f;
            //p.xyz = floor(frac(j * ip.xyz) * 7.0) * ip.z - 1.0;
            p.W = 1.5f - Vector3.Dot(new Vector3((float)Math.Abs(p.X), (float)Math.Abs(p.Y), (float)Math.Abs(p.Z)), new Vector3(ones.X, ones.Y, ones.Z));

            // GLSL: lessThan(x, y) = x < y
            // HLSL: 1 - step(y, x) = x < y
            s =
                add(-1 * new Vector4(0 >= p.X ? 1 : 0, 0 >= p.Y ? 1 : 0, 0 >= p.Z ? 1 : 0, 0 >= p.W ? 1 : 0), 1);
            ;
            p.X = p.X + (s.X * 2 - 1) * s.W;
            p.Y = p.Y + (s.Y * 2 - 1) * s.W;
            p.Z = p.Z + (s.Z * 2 - 1) * s.W;


            return p;
        }
        private static float taylorInvSqrt(float r)
        {
            return 1.79284291400159f - 0.85373472095314f * r;
        }
        private static Vector4 mod289(Vector4 x)
        {
            return new Vector4(x.X - (float)Math.Floor(x.X * 0.00346020761245674740484429065744f) * 289.0f,
                x.Y - (float)Math.Floor(x.Y * 0.00346020761245674740484429065744f) * 289.0f,
                x.Z - (float)Math.Floor(x.Z * 0.00346020761245674740484429065744f) * 289.0f,
                x.W - (float)Math.Floor(x.W * 0.00346020761245674740484429065744f) * 289.0f);
        }
        private static float mod289(float x)
        {
            return x - (float)Math.Floor(x * 0.00346020761245674740484429065744f) * 289.0f;
        }
        private static float permute(float x)
        {
            return mod289(
                x * x * 34.0f + x
            );
        }
        private static Vector4 permute(Vector4 x)
        {
            return mod289(
                x * x * 34.0f + x
            );
        }
        public static float noise(Vector4 v)
        {
            Vector4 C = new Vector4(
                0.138196601125011f, // (5 - sqrt(5))/20 G4
                0.276393202250021f, // 2 * G4
                0.414589803375032f, // 3 * G4
             -0.447213595499958f  // -1 + 4 * G4
            );

            // First corner
            Vector4 i = (
                v +
                (

                    0.309016994374947451f // (sqrt(5) - 1) / 4
                    * v
                )
            );
            i = new Vector4((float)Math.Floor(i.X), (float)Math.Floor(i.Y), (float)Math.Floor(i.Z), (float)Math.Floor(i.W));
            Vector4 x0 = v - add(i, Vector4.Dot(i, new Vector4(C.X, C.X, C.X, C.X)));

            // Other corners

            // Rank sorting originally contributed by Bill Licea-Kane, AMD (formerly ATI)
            Vector4 i0;
            Vector4 isX = new Vector4(x0.Y >= x0.X ? 1 : 0, x0.Z >= x0.X ? 1 : 0, x0.W >= x0.X ? 1 : 0, 0.0f);
            Vector4 isYZ = new Vector4(x0.Z >= x0.Y ? 1 : 0, x0.W >= x0.Y ? 1 : 0, x0.W >= x0.Z ? 1 : 0, 0.0f);
            i0.X = isX.X + isX.Y + isX.Z;
            i0.Y = 1.0f - isX.Y;
            i0.Z = 1.0f - isX.Z;
            i0.W = 1.0f - isX.W;
            i0.Y += isYZ.X + isYZ.Y;
            i0.Z += 1.0f - isYZ.X;
            i0.W += 1.0f - isYZ.Y;
            i0.Z += isYZ.Z;
            i0.W += 1.0f - isYZ.Z;

            // i0 now contains the unique values 0,1,2,3 in each channel
            Vector4 i3 = saturate(i0);
            Vector4 i2 = saturate(add(i0, -1.0f));
            Vector4 i1 = saturate(add(i0, -2.0f));

            //	x0 = x0 - 0.0 + 0.0 * C.xxxx
            //	x1 = x0 - i1  + 1.0 * C.xxxx
            //	x2 = x0 - i2  + 2.0 * C.xxxx
            //	x3 = x0 - i3  + 3.0 * C.xxxx
            //	x4 = x0 - 1.0 + 4.0 * C.xxxx
            Vector4 x1 = x0 - i1 + new Vector4(C.X, C.X, C.X, C.X);
            Vector4 x2 = x0 - i2 + new Vector4(C.Y, C.Y, C.Y, C.Y);
            Vector4 x3 = x0 - i3 + new Vector4(C.Z, C.Z, C.Z, C.Z);
            Vector4 x4 = x0 + new Vector4(C.W, C.W, C.W, C.W);

            // Permutations
            i = mod289(i);
            float j0 = permute(
                permute(
                    permute(
                        permute(i.W) + i.Z
                    ) + i.Y
                ) + i.X
            );
            Vector4 j1 = permute(
                permute(
                    permute(
                        permute(
                            add(new Vector4(i1.W, i2.W, i3.W, 1.0f), i.W)
                        ) + add(new Vector4(i1.Z, i2.Z, i3.Z, 1.0f), i.Z)
                    ) + add(new Vector4(i1.Y, i2.Y, i3.Y, 1.0f), i.Y)
                ) + add(new Vector4(i1.X, i2.X, i3.X, 1.0f), i.X)
            );

            // Gradients: 7x7x6 points over a cube, mapped onto a 4-cross polytope
            // 7*7*6 = 294, which is close to the ring size 17*17 = 289.
            Vector4 ip = new Vector4(
                0.003401360544217687075f, // 1/294
                0.020408163265306122449f, // 1/49
                0.142857142857142857143f, // 1/7
                0.0f
            );

            Vector4 p0 = grad4(j0, ip);
            Vector4 p1 = grad4(j1.X, ip);
            Vector4 p2 = grad4(j1.Y, ip);
            Vector4 p3 = grad4(j1.Z, ip);
            Vector4 p4 = grad4(j1.W, ip);

            // Normalise gradients
            Vector4 norm = new Vector4(taylorInvSqrt(
                Vector4.Dot(p0, p0)),
                taylorInvSqrt(Vector4.Dot(p1, p1)),
                taylorInvSqrt(Vector4.Dot(p2, p2)),
                taylorInvSqrt(Vector4.Dot(p3, p3)
            ));
            p0 *= norm.X;
            p1 *= norm.Y;
            p2 *= norm.Z;
            p3 *= norm.W;
            p4 *= taylorInvSqrt(Vector4.Dot(p4, p4));

            // Mix contributions from the five corners
            Vector3 m0 = new Vector3(Math.Max(
                add(-1 * new Vector4(
                    Vector4.Dot(x0, x0),
                    Vector4.Dot(x1, x1),
                    Vector4.Dot(x2, x2), 0
                ), 0.6f).X,
                0.0f), Math.Max(
                add(-1 * new Vector4(
                    Vector4.Dot(x0, x0),
                    Vector4.Dot(x1, x1),
                    Vector4.Dot(x2, x2), 0
                ), 0.6f).Y,
                0.0f), Math.Max(
                add(-1 * new Vector4(
                    Vector4.Dot(x0, x0),
                    Vector4.Dot(x1, x1),
                    Vector4.Dot(x2, x2), 0
                ), 0.6f).Z,
                0.0f)
            );
            Vector2 m1 = new Vector2(Math.Max(
                add(-1 * new Vector4(
                    Vector4.Dot(x3, x3),
                    Vector4.Dot(x4, x4), 0, 0
                ), 0.6f).X,
                0.0f),
                Math.Max(
                add(-1 * new Vector4(
                    Vector4.Dot(x3, x3),
                    Vector4.Dot(x4, x4), 0, 0
                ), 0.6f).Y,
                0.0f)
            );
            m0 = m0 * m0;
            m1 = m1 * m1;

            return 49.0f * (
                Vector3.Dot(
                    m0 * m0,
                    new Vector3(
                        Vector4.Dot(p0, x0),
                        Vector4.Dot(p1, x1),
                        Vector4.Dot(p2, x2)
                    )
                ) + Vector2.Dot(
                    m1 * m1,
                    new Vector2(
                        Vector4.Dot(p3, x3),
                        Vector4.Dot(p4, x4)
                    )
                )
            );
        }

        static float interpolate(float t, float a, float b)
        {
            return a + t * (b - a);
        }
        static float smoothInterpolant(float t)
        {
            return t * t * t * (6 * t * t - 15 * t + 10);
        }

        static int hash(Vector4 v)
        {
            /*
            A 256 element table of random numbers from 0-255.

            This table is from Malcolm Kesson's reference implementation
            of Perlin noise: http://www.fundza.com/c4serious/noise/perlin/perlin.html.
         */
            int[] p = {
            151,
            160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,
            21,10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,
            35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,
            74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,
            230,220,105,92,41,55,46,245,40,244,102,143,54,65,25,63,161,1,216,
            80,73,209,76,132,187,208,89,18,169,200,196,135,130,116,188,159,86,
            164,100,109,198,173,186,3,64,52,217,226,250,124,123,5,202,38,147,
            118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,223,
            183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,
            172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,
            218,246,97,228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,
            145,235,249,14,239,107,49,192,214,31,181,199,106,157,184,84,204,176,
            115,121,50,45,127,4,150,254,138,236,205,93,222,114,67,29,24,72,243,
            141,128,195,78,66,215,61,156,180
                  };


            // The pattern tiles every 256 samples in any direction
            int h = p[(int)v.X & 255];
            h = p[(int)(h + v.Y) & 255];
            h = p[(int)(h + v.Z) & 255];
            h = p[(int)(h + v.W) & 255];
            return h;

        }
        static float dot_gradient(int hash, float x, float y, float z, float w)
        {
            /*var grad = gradient(hash);
            var dot = x * grad[0] + y * grad[1] + z * grad[2] + w * grad[3];*/

            // Hard-code the dot product since it's much faster...
            switch (hash % 32)
            {
                case 0: return -y - z - w;
                case 1: return -y - z + w;
                case 2: return -y + z - w;
                case 3: return -y + z + w;
                case 4: return y - z - w;
                case 5: return y - z + w;
                case 6: return y + z - w;
                case 7: return y + z + w;
                case 8: return -x - y - w;
                case 9: return -x + y - w;

                case 10: return x - y - w;
                case 11: return x + y - w;
                case 12: return -x - y + w;
                case 13: return -x + y + w;
                case 14: return x - y + w;
                case 15: return x + y + w;
                case 16: return -x - z - w;
                case 17: return x - z - w;
                case 18: return -x - z + w;
                case 19: return x - z + w;

                case 20: return -x + z - w;
                case 21: return x + z - w;
                case 22: return -x + z + w;
                case 23: return x + z + w;
                case 24: return -y - z;
                case 25: return -y - z;
                case 26: return -y + z;
                case 27: return -y + z;
                case 28: return y - z;
                case 29: return y - z;

                case 30: return y + z;
                case 31: return y + z;
            }
            return 0;
        }
        static float noise2(float x, float y, float z, float w)
        {
            // Get the cell and position within the cell in the 4d grid
            float[] cell = {
            (float)Math.Floor(x),
            (float)Math.Floor(y),
            (float)Math.Floor(z),
            (float)Math.Floor(w)
     };
            float[] f = {
            x - cell[0],
            y - cell[1],
            z - cell[2],
            w - cell[3]
                    };

            // Compute the gradient at each corner of the hypercube
            float[] g = new float[16];
            for (var i = 0; i < 16; ++i)
            {
                var cx = (i & 1) > 0 ? 1 : 0;
                var cy = (i & 2) > 0 ? 1 : 0;
                var cz = (i & 4) > 0 ? 1 : 0;
                var cw = (i & 8) > 0 ? 1 : 0;

                int h = hash(new Vector4(cell[0] + cx, cell[1] + cy, cell[2] + cz, cell[3] + cw));
                g[i] = dot_gradient(h, f[0] - cx, f[1] - cy, f[2] - cz, f[3] - cw);
            }

            float[] t = 
        {
            smoothInterpolant(f[0]),
            smoothInterpolant(f[1]),
            smoothInterpolant(f[2]),
            smoothInterpolant(f[3])
        };

            float[] b3 = new float[8];
            float[] b2 = new float[4];
            float[] b1 = new float[2];

            for (int i = 0; i < 8; ++i)
                b3[i] = interpolate(t[0], g[i * 2], g[i * 2 + 1]);
            for (int i = 0; i < 4; ++i)
                b2[i] = interpolate(t[1], b3[i * 2], b3[i * 2 + 1]);
            for (int i = 0; i < 2; ++i)
                b1[i] = interpolate(t[2], b2[i * 2], b2[i * 2 + 1]);

            float b = interpolate(t[3], b1[0], b1[1]);

            return (b + 1.0f) / 2.0f;
        }
        public static float noise4D(Vector4 v)
        { return noise2(v.X, v.Y, v.Z, v.W); }
    }
}
