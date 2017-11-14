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
        public Matrix worldMatrix, projectionMatrix, viewMatrix; // Matrizes reponsáveis pelo render
        Matrix rotationMatrix;
        private Vector3 direction;
        public Vector3 cameraPosition; // Vector que representa a posição da camera no espaço
        private Vector3 cameraLookAt; // Vector que representa a posição para que a camera está apontar
        static private Vector3 directionBase; // Vector de direção para calcular as rotações e translações
        private Vector2 center; // Vector para guardar a posição central do device
        private float cameraVelocity; // Escalar para a velocidade de movimento da camera
        float yaw, pitch,roll; // para a rotação e inclinação da camera
        MouseState previousMS; // MouseState para guardar a ultima posição do rato
        private int cameraTag;
        private bool camChange = false;
        const int SurfaceFollow = 1,
            FreeCam = 2,
            TankFollow = 3;


        public ClsCamara(Game game, GraphicsDevice device) : base(game)
        {
            // camPositon = Tpos - Tdir*d + Tnormal*height
            // camTarget = cPos + Tdir;
            //            position += tankDirection;

            #region Set Camera
            cameraPosition = new Vector3(64f, 30.0f, 64.0f); // posição inicial da camera
            directionBase = new Vector3(0.0f, 0.0f, 1.0f); // vector a direcionar para x=z
            cameraLookAt = cameraPosition + directionBase; // posição para qual a camera está apontar
            cameraVelocity = 0.1f; // definir a velocidade de movimento da camera
            cameraTag = FreeCam;

            yaw = MathHelper.ToRadians(0);
            pitch = MathHelper.ToRadians(0);
            roll = MathHelper.ToRadians(0);
            center = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);//definir o centro do ecrã
            #endregion
            #region Set Matrices
            float aspectRatio = device.Viewport.AspectRatio;
            worldMatrix = Matrix.Identity;
            viewMatrix = Matrix.CreateLookAt(cameraPosition,
                cameraLookAt,
                Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                aspectRatio,
                0.01f,
                1000.0f);
            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            direction = Vector3.Transform(directionBase, rotationMatrix);
            #endregion
        }
        public void Update(KeyboardState kb, MouseState ms, ClsTerreno terreno)
        {
            if (kb.IsKeyDown(Keys.F1))
            {
                cameraTag = SurfaceFollow;
                camChange = true;
            }
            else if (kb.IsKeyDown(Keys.F2))
            {
                cameraTag = FreeCam;
                camChange = true;
            }

            else if (kb.IsKeyDown(Keys.F3))
            {
                cameraTag = TankFollow;
                camChange = true;
            }

            switch (cameraTag)
            {
                case SurfaceFollow:
                    #region Mouse Input
                    if (ms.Position.X > previousMS.Position.X)
                        yaw -= MathHelper.ToRadians(1.0f);
                    if (ms.Position.X < previousMS.Position.X)
                        yaw += MathHelper.ToRadians(1.0f);
                    if (ms.Position.Y > previousMS.Position.Y)
                        pitch += MathHelper.ToRadians(1.0f);
                    if (ms.Position.Y < previousMS.Position.Y)
                        pitch -= MathHelper.ToRadians(1.0f);

                    previousMS = ms;
                    #endregion
                    #region Keyboard Input
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
                    #endregion
                    cameraPosition.Y = terreno.Bilinear(cameraPosition).Y + 5;
                    camChange = false;
                    break;
                case FreeCam:
                    #region Mouse Input
                    if (ms.Position.X > previousMS.Position.X)
                        yaw -= MathHelper.ToRadians(1.0f);
                    if (ms.Position.X < previousMS.Position.X)
                        yaw += MathHelper.ToRadians(1.0f);
                    if (ms.Position.Y > previousMS.Position.Y)
                        pitch += MathHelper.ToRadians(1.0f);
                    if (ms.Position.Y < previousMS.Position.Y)
                        pitch -= MathHelper.ToRadians(1.0f);


                    previousMS = ms;
                    #endregion
                    #region Keyboard Input
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
                    if (kb.IsKeyDown(Keys.Right))
                    {
                        this.roll += MathHelper.ToRadians(1.0f);
                    }
                    if (kb.IsKeyDown(Keys.Left))
                    {
                        this.roll -= MathHelper.ToRadians(1.0f);
                    }
                    #endregion
                    if (camChange==true)
                    cameraPosition = new Vector3(64, 30, 64);
                    camChange = false;
                    break;
                case 3:
                    break;
            }
            //cameraPosition.Y = terreno.Bilinear(cameraPosition).Y +5;
            Debug.WriteLine("CamX:" + cameraPosition.X + "CamZ:" + cameraPosition.Z);


            #region Movement
            if (kb.IsKeyDown(Keys.W))
            {
                this.cameraPosition += this.cameraVelocity * direction;
            }
            if (kb.IsKeyDown(Keys.S))
            {
                this.cameraPosition -= this.cameraVelocity * direction;
            }
            // calcula a matrix de rotação e direção
            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            direction = Vector3.Transform(directionBase, rotationMatrix);
            this.cameraLookAt = cameraPosition + direction;
            this.viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
            #endregion
            // Colission detection v0.000000001
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
