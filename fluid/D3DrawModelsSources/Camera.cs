using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace fluid.D3DrawModelsSources
{
    class Camera
    {
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }

        public Matrix ViewMatrix { get; private set; }

        public void SetPosition(float x, float y, float z)
        {
            PositionX = x;
            PositionY = y;
            PositionZ = z;
        }

        public void ModPositionInSphere(float x, float y, float z)
        {
            PositionX += x;
            PositionY += y;
            PositionZ += z;

        }
        public Vector3 GetPosition()
        {
            return new Vector3(PositionX, PositionY, PositionZ);
        }
       
        public void Render()
        {
            /*
            // Setup the position of the camera in the world.
            var position = new Vector3(PositionX, PositionY, PositionZ);

            // Setup where the camera is looking by default.
            var lookAt = new Vector3(0, 0, 1);
            
            // Set the yaw (Y axis), pitch (X axis), and roll (Z axis) rotations in radians.
            var pitch = RotationX * 0.0174532925f;
            var yaw = RotationY * 0.0174532925f;
            var roll = RotationZ * 0.0174532925f;

            // Create the rotation matrix from the yaw, pitch, and roll values.
            var rotationMatrix = Matrix.RotationYawPitchRoll(yaw, pitch, roll);
            
            // Transform the lookAt and up vector by the rotation matrix so the view is correctly rotated at the origin.
            lookAt = Vector3.TransformCoordinate(lookAt, rotationMatrix);
            var up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

            // Translate the rotated camera position to the location of the viewer.
            lookAt = position + lookAt;

            // Finally create the view matrix from the three updated vectors.
            ViewMatrix = Matrix.LookAtLH(position, lookAt, up);
             */
            var position = new Vector3(PositionZ * (float)Math.Sin(PositionY) * (float)Math.Cos(PositionX),
                                        PositionZ * (float)Math.Cos(PositionY) + PositionZ,
                                        PositionZ * (float)Math.Sin(PositionY) * (float)Math.Sin(PositionX));
            var lookAt = new Vector3(0, 0, 0);
            var up = new Vector3(0, 1, 0);
            ViewMatrix = Matrix.LookAtLH(position, lookAt, up);

        }
    }
}
