using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho
{
    class ClsCamara
    {

        public Matrix worldMatrix, projectionMatrix, viewMatrix;
        public Vector3 position, direction, directionBase;
        float width, height, yaw, roll, velocity;


        public ClsCamara(GraphicsDevice device)
        {
            width = device.Viewport.Width;
            height = device.Viewport.Height;
            position = new Vector3(30.0f, 30.0f, 30.0f);
            direction = new Vector3(1.0f, 0, 1.0f);
            directionBase = new Vector3(1, 0, 1);
            velocity = 0.1f;

            worldMatrix = Matrix.Identity;
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = width / height;
            // Matrix de view
            viewMatrix = Matrix.CreateLookAt(position, position + direction, Vector3.Up);
            // Matrix de perspective
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.0001f, 1000.0f);
        }
        public void Update(KeyboardState kb)
        {
            if (kb.IsKeyDown(Keys.D))
            {
                yaw += MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.A))
            {
                this.yaw -= MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.Right))
            {
                this.roll += MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.Left))
            {
                this.roll -= MathHelper.ToRadians(1.0f);
            }

            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, roll, 0);
            Vector3 direction = Vector3.Transform(this.directionBase, rotation);

            if (kb.IsKeyDown(Keys.W))
            {
                this.position += this.velocity * direction;
            }
            if (kb.IsKeyDown(Keys.S))
            {
                this.position -= this.velocity * direction;
            }
            //
            this.worldMatrix = Matrix.CreateTranslation(position) * rotation;
            this.viewMatrix = Matrix.CreateLookAt(position, position + direction, Vector3.Up);
        }
    }
}
