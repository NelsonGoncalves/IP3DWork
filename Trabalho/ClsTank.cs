using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Trabalho
{
    class ClsTank
    {
        #region Fields
        private Model model; // modelo 
        private Matrix tank; // world Matrix

        private Vector3 rotation;
        private Vector3 positionTank; // posição do tank
        private Vector3 relativeForward, relativeRight;
        private Vector3 Up, Forward, Right;

        #region Float Angles
        private float turretRotation,
            cannonRotation,
            hatchRotation,
            wheelRotation = 0,
            steerRotation = 0,
            scale = 0.01f,
            speed = 0.1f,
            yaw = 0,
            pitch = 0,
            roll = 0;
        #endregion
        #region ModelBones

        private ModelBone turretBone,
            cannonBone,
            leftBackWheel,
            rightBackWheel,
            leftFrontWheel,
            rightFrontWheel,
            leftSteer,
            rightSteer,
            hatch;
        #endregion
        #region Model Trasnform Matrixes
        private Matrix rotationMatrix,
            transformMatrix,
            turretTransform,
            cannonTransform,
            leftBackWheelTransform,
            rightBackWheelTransform,
            leftFrontWheelTransform,
            rightFrontWheelTransform,
            leftSteerTransform,
            rightSteerTrasnform;
        private Matrix[] boneTransforms;        // Keeps all transforms
        #endregion
        #region Public gets and setters
        public Vector3 Position
        {
            get
            {
                return positionTank;
            }
            set
            {
                positionTank = value;
            }
        }
        public float TurretRotation
        {
            get
            {
                return turretRotation;
            }
            set
            {
                turretRotation = MathHelper.WrapAngle(value);
            }
        }
        public float CannonRotation
        {
            get
            {
                return cannonRotation;
            }
            set
            {
                cannonRotation = MathHelper.Clamp(value,
                    MathHelper.ToRadians(-90),
                    MathHelper.ToRadians(0));
            }
        }
        #endregion
        #endregion
        public ClsTank(GraphicsDevice device, ContentManager content, Vector3 position)
        {
            this.model = LoadModel("Tank", content);
            this.positionTank = position;
            this.tank = Matrix.Identity;
            this.rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
            this.transformMatrix = Matrix.CreateTranslation(positionTank);
            // definir o forward,right e up que possamos mudar
            this.relativeForward = this.Forward = Vector3.Forward;
            this.relativeRight = this.Right = Vector3.Right;
            this.Up = Vector3.Up;

            #region Read the bones
            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];
            leftBackWheel = model.Bones["l_back_wheel_geo"];
            rightBackWheel = model.Bones["r_back_wheel_geo"];
            leftFrontWheel = model.Bones["l_front_wheel_geo"];
            rightFrontWheel = model.Bones["r_front_wheel_geo"];
            leftSteer = model.Bones["l_steer_geo"];
            rightSteer = model.Bones["r_steer_geo"];
            hatch = model.Bones["hatch_geo"];
            #endregion
            #region Read the default transforms
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            leftBackWheelTransform = leftBackWheel.Transform;
            rightBackWheelTransform = rightBackWheel.Transform;
            leftFrontWheelTransform = leftFrontWheel.Transform;
            rightFrontWheelTransform = rightFrontWheel.Transform;
            leftSteerTransform = leftSteer.Transform;
            rightSteerTrasnform = rightSteer.Transform;
            // create array to store final bone transforms
            boneTransforms = new Matrix[model.Bones.Count];
            #endregion
        }
        private Model LoadModel(String assetName, ContentManager content)
        {
            Model novoModelo = content.Load<Model>(assetName);
            return novoModelo;
        }
        public void Update(KeyboardState kb, ClsTerreno terreno, GameTime gameTime)
        {

            #region Movimento
            // Canhão
            if (kb.IsKeyDown(Keys.Up) && !kb.IsKeyDown(Keys.Down))
            {
                if (this.cannonRotation >= -Math.PI / 4)
                    this.cannonRotation -= MathHelper.ToRadians(1f);
            }
            else if (kb.IsKeyDown(Keys.Down) && !kb.IsKeyDown(Keys.Up))
            {
                if (this.cannonRotation <= 0)
                    this.cannonRotation += MathHelper.ToRadians(1f);
            }
            // turret
            if (kb.IsKeyDown(Keys.Right) && !kb.IsKeyDown(Keys.Left))
                this.turretRotation += MathHelper.ToRadians(1f);
            else if (kb.IsKeyDown(Keys.Left) && !kb.IsKeyDown(Keys.Right))
                this.turretRotation -= MathHelper.ToRadians(1f);
            // MOVER TANK
            if (kb.IsKeyDown(Keys.G) && !kb.IsKeyDown(Keys.J))
            {
                steerRotation = MathHelper.ToRadians(10f);

                if (kb.IsKeyDown(Keys.Y))
                    rotation.X += MathHelper.ToRadians(1f);
                else if (kb.IsKeyDown(Keys.H))
                    rotation.X -= MathHelper.ToRadians(1f);
            }
            else if (kb.IsKeyDown(Keys.J) && !kb.IsKeyDown(Keys.G))
            {
                steerRotation = MathHelper.ToRadians(-10f);

                if (kb.IsKeyDown(Keys.Y))
                    rotation.X -= MathHelper.ToRadians(1f);
                else if (kb.IsKeyDown(Keys.H))
                    rotation.X += MathHelper.ToRadians(1f);
            }
            else
            {
                steerRotation = MathHelper.ToRadians(0f);
            }
            // Mantem a rotação dentro de 2*pi (360º)
            if (rotation.X > 2 * MathHelper.Pi || rotation.X < -2 * MathHelper.Pi)
                rotation.X = 0;

            if (kb.IsKeyDown(Keys.Y))
            {
                this.positionTank -= this.relativeForward * (float)gameTime.ElapsedGameTime.TotalSeconds*speed;
                this.wheelRotation += MathHelper.ToRadians(1f);

            }
            if (kb.IsKeyDown(Keys.H))
            {
                this.positionTank += this.relativeForward * (float)gameTime.ElapsedGameTime.TotalSeconds*speed;
                this.wheelRotation -= MathHelper.ToRadians(1f);
            }
            #endregion
            #region Tank position calculations
            // trasnformar o relative forward e right
            relativeForward = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(rotation.X));
            relativeForward.Normalize();
            relativeRight = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(rotation.X));
            relativeRight.Normalize();

            this.Up = terreno.GetNormalInterpolation(positionTank);
            this.Forward = Vector3.Cross(Up, relativeRight);
            this.Right = Vector3.Cross(relativeForward, Up); // calcular o vector right
            Right.Normalize();
            transformMatrix = Matrix.CreateTranslation(positionTank);
            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, 45f); // Rotação a dar ao tank
            rotationMatrix.Forward = Forward;
            rotationMatrix.Up = Up;
            rotationMatrix.Right = Right;

            positionTank.Y = terreno.Interpolation(positionTank); // actualiza a altura do tank em relação ao terreno
            #endregion
            #region Apply transformations to the model bones
            // Applies a transformations to any bone (Root, Turret, Cannon, …
            model.Root.Transform = Matrix.CreateScale(scale) * rotationMatrix * transformMatrix;

            Matrix wheel = Matrix.CreateRotationX(wheelRotation);
            Matrix steer = Matrix.CreateRotationY(steerRotation);
            Matrix turret = Matrix.CreateRotationY(turretRotation);
            Matrix cannon = Matrix.CreateRotationX(cannonRotation);

            turretBone.Transform = turret * turretTransform;
            cannonBone.Transform = cannon * cannonTransform;
            leftBackWheel.Transform = wheel * leftBackWheelTransform;
            rightBackWheel.Transform = wheel * rightBackWheelTransform;
            leftFrontWheel.Transform = wheel * leftFrontWheelTransform;
            rightFrontWheel.Transform = wheel * rightFrontWheelTransform;
            leftSteer.Transform = steer * leftSteerTransform;
            rightSteer.Transform = steer * rightSteerTrasnform;

            // Appies transforms to bones in a cascade
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            #endregion
        }
        public void DrawModel(ClsCamara cam)
        {

            // Draw the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = cam.viewMatrix;
                    effect.Projection = cam.projectionMatrix;
                   
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
