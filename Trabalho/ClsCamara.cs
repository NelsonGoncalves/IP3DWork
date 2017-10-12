using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Trabalho
{
    class ClsCamara : GameComponent
    {
        //Attributes
        public Matrix worldMatrix, projectionMatrix, viewMatrix;
        public Vector3 cameraPosition;
        private Vector3 cameraLookAt;
        private Vector3 directionBase;
        Vector2 center;
        private float cameraVelocity;
        float yaw, pitch;
        MouseState preMS;
        float height, width;

        public ClsCamara(Game game, GraphicsDevice device) : base(game)
        {
            cameraPosition = new Vector3(60f, 30.0f, 60.0f);
            directionBase = new Vector3(1.0f, 0.0f, 1.0f);
            cameraLookAt = cameraPosition + directionBase;

            center = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);
            height = device.Viewport.Height;
            cameraVelocity = 0.1f;

            yaw = MathHelper.ToRadians(45f);

            pitch = MathHelper.ToRadians(45f);

            worldMatrix = Matrix.Identity;
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = device.Viewport.AspectRatio;
            // Matrix de view
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
            // Matrix de perspective
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.0001f, 1000.0f);
        }

        public void Update(KeyboardState kb, MouseState ms)
        {
            
            Debug.WriteLine("CamX:" + cameraPosition.X + "CamZ:" + cameraPosition.Z);
            //rato
            if (ms.Position.X > preMS.Position.X)
                yaw -= MathHelper.ToRadians(1.0f);
            if (ms.Position.X < preMS.Position.X)
                yaw += MathHelper.ToRadians(1.0f);
            if (ms.Position.Y > preMS.Position.Y)
                pitch += MathHelper.ToRadians(1.0f);
            if (ms.Position.Y < preMS.Position.Y)
                pitch -= MathHelper.ToRadians(1.0f);

            preMS = ms;
            
            // teclado
            if (kb.IsKeyDown(Keys.D))
            {
                this.yaw -= MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.A))
            {
                this.yaw += MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.Up))
            {
                this.pitch += MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.Down))
            {
                this.pitch -= MathHelper.ToRadians(1.0f);
            }

            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 direction = Vector3.Transform(this.directionBase, rotation);
            Debug.WriteLine("direction:" + direction);

            if (kb.IsKeyDown(Keys.W))
            {
                this.cameraPosition += this.cameraVelocity * direction;
            }
            if (kb.IsKeyDown(Keys.S))
            {
                this.cameraPosition -= this.cameraVelocity * direction;
            }
            //
            cameraLookAt = cameraPosition + direction;
            this.worldMatrix = rotation * Matrix.CreateTranslation(cameraPosition);
            this.viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
            
            if (cameraPosition.X > 127)
                cameraPosition.X = 126;
            if (cameraPosition.X < 1)
                cameraPosition.X = 1;
            if (cameraPosition.Z > 127)
                cameraPosition.Z = 126;
            if (cameraPosition.Z < 1)
                cameraPosition.Z = 1;
        }
    }
}
