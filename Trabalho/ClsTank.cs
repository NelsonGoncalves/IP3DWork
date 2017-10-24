using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Trabalho
{
    class ClsTank
    {
        Model model;

        // Turret and Cannon bones
        ModelBone turretBone;
        ModelBone cannonBone;
        // Default transforms
        Matrix cannonTransform;
        Matrix turretTransform;
        // Keeps all transforms
        Matrix[] boneTransforms;        BasicEffect effect;

        public ClsTank(GraphicsDevice device, ContentManager content, ClsCamara cam)
        {
            effect = new BasicEffect(device); // instanciar
            effect.Projection = cam.projectionMatrix; // Projection matrix será universal
            effect.World = Matrix.Identity; // World Matrix representa a posição do objeto no espaço(3D)
            effect.View = cam.viewMatrix; // ViewMatrix será universal
            effect.VertexColorEnabled = false; // desligar as cores
            effect.TextureEnabled = true; // ligar as texturas
            model = LoadModel("Tank", content, cam);
            // Read bones
            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];
            // Read bone default transforms
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            // create array to store final bone transforms
            boneTransforms = new Matrix[model.Bones.Count];



        }
        private Model LoadModel(String assetName, ContentManager content, ClsCamara cam)
        {
            Model novoModelo = content.Load<Model>(assetName);
            foreach (ModelMesh mesh in novoModelo.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }
            return novoModelo;
        }
        public void DrawModel(ClsCamara cam)
        {
            // Applies a transformations to any bone (Root, Turret, Cannon, …
            model.Root.Transform = Matrix.CreateTranslation(new Vector3(0f, 0f, 1f));
            turretBone.Transform = Matrix.CreateRotationY(1f);
            cannonBone.Transform = Matrix.CreateRotationX(2f);
            // Appies transforms to bones in a cascade
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
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
