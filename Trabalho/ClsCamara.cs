using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public Vector3 cameraPosition, cameraDirection;
        float width, height;


        public ClsCamara(GraphicsDevice device)
        {
            width = device.Viewport.Width;
            height = device.Viewport.Height;
            this.cameraPosition = new Vector3(60.0f, 0.0f, 60.0f);
            this.cameraDirection = Vector3.Zero;

            worldMatrix = Matrix.Identity;
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = width / height;
            // Matrix de view
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraDirection, Vector3.Up);
            // Matrix de perspective
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.0001f, 20.0f);
        }
    }
}
