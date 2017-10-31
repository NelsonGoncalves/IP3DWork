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
        Model model;
        BasicEffect effect;
        Matrix world, view, projection;
        // Turret and Cannon bones
        ModelBone turretBone;
        ModelBone cannonBone;
        // Default transforms
        Matrix cannonTransform;
        Matrix turretTransform;
        // Keeps all transforms
        Matrix[] boneTransforms;

        float scale;
        float turretAngle = 0.0f;
        float cannonAngle = 0.0f;

        Vector3 position;
        Vector3 directionHorizontal;


        public ClsTank(GraphicsDevice device, ContentManager content, ClsCamara cam)
        {
            float aspectRatio = device.Viewport.AspectRatio;
            position = cam.cameraPosition;
            directionHorizontal = new Vector3(1, 0, 1);

            world = Matrix.CreateScale(0.005f);
            view = Matrix.CreateLookAt(new Vector3(0, 5, 5), Vector3.Zero, Vector3.Up); // ViewMatrix será universal
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                            aspectRatio,
                            0.01f,
                            100.0f); ; // Projection matrix será universal

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
        public void Update(KeyboardState kb, ClsTerreno terreno,ClsCamara cam)
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
            Vector3 tankNormal =  terreno.GetNormal(position); // normal do terreno na posição do tank
            Vector3 tankRight = Vector3.Cross(directionHorizontal, tankNormal);
            Vector3 tankDirection = Vector3.Cross(tankNormal, tankRight);
            Matrix rotation = Matrix.Identity;
            rotation.Forward = tankDirection;
            rotation.Up = tankNormal;
            rotation.Right = tankRight;

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
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
