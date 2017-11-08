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
        Matrix world, view, projection;
        // Turret and Cannon bones
        ModelBone turretBone;
        ModelBone cannonBone;
        // Default transforms

        float scale;


        private Model model; // modelo
        private GraphicsDevice device; // device
        private Vector3 position; // posição do tank
        private Vector3 directionHorizontal; // direção base
        private float tankRotation;
        private float turretAngle = 0.0f;
        private float cannonAngle = 0.0f;
        private Matrix cannonTransform;
        private Matrix turretTransform;
        private Matrix[] boneTransforms;        // Keeps all transforms
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public float TankRotation
        {
            get
            {
                return tankRotation;
            }
            set
            {
                tankRotation = MathHelper.WrapAngle(value);
            }
        }
        public float TurretAngle
        {
            get
            {
                return turretAngle;
            }
            set
            {
                turretAngle = MathHelper.WrapAngle(value);
            }
        }
        public float CannonAngle
        {
            get
            {
                return cannonAngle;
            }
            set
            {
                cannonAngle = MathHelper.Clamp(value,
                    MathHelper.ToRadians(-90),
                    MathHelper.ToRadians(0));
            }
        }


        public ClsTank(GraphicsDevice device, ContentManager content, ClsCamara cam)
        {
            float aspectRatio = device.Viewport.AspectRatio;
            position = new Vector3(64f, 30f, 64f);
            directionHorizontal = new Vector3(1, 0, 1);

            world = Matrix.CreateScale(0.005f);
            view = cam.viewMatrix; // ViewMatrix será universal
            projection = cam.projectionMatrix; // Projection matrix será universal

            model = LoadModel("Tank", content);
            scale = 0.01f;
            // Read bones
            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];
            // Read bone default transforms
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            // create array to store final bone transforms
            boneTransforms = new Matrix[model.Bones.Count];

        }
        private Model LoadModel(String assetName, ContentManager content)
        {
            Model novoModelo = content.Load<Model>(assetName);
            //foreach (ModelMesh mesh in novoModelo.Meshes)
            //{
            //    foreach (ModelMeshPart meshPart in mesh.MeshParts)
            //    {
            //        meshPart.Effect = effect.Clone();
            //    }
            //}
            return novoModelo;
        }

        public void Update(KeyboardState kb, ClsTerreno terreno, ClsCamara cam)
        {
            if (kb.IsKeyDown(Keys.Left))
                turretAngle += MathHelper.ToRadians(1);
            if (kb.IsKeyDown(Keys.Right))
                turretAngle -= MathHelper.ToRadians(1);
            if (kb.IsKeyDown(Keys.Up))
                cannonAngle += MathHelper.ToRadians(1);
            if (kb.IsKeyDown(Keys.Down))
                cannonAngle -= MathHelper.ToRadians(1);
            Matrix translation = Matrix.CreateTranslation(position);
            position = terreno.GetHeight(position);
            Vector3 tankNormal = terreno.GetNormal(position); // normal do terreno na posição do tank
            Vector3 tankRight = Vector3.Cross(directionHorizontal, tankNormal);
            Vector3 tankDirection = Vector3.Cross(tankNormal, tankRight);
            Matrix rotation = Matrix.Identity;
            rotation.Forward = tankDirection;
            rotation.Up = tankNormal;
            rotation.Right = tankRight;

            //            position += tankDirection;

            // Applies a transformations to any bone (Root, Turret, Cannon, …
            //model.Root.Transform = Matrix.CreateTranslation(new Vector3(0f, 0f, 1f));
            model.Root.Transform = Matrix.CreateScale(scale) * rotation * translation;
            turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;

            // Appies transforms to bones in a cascade
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

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
