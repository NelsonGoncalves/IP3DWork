using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho
{
    class ClsTerreno
    {
        VertexPositionNormalTexture[] vertex;
        short[] index;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        Matrix worldMatrix;
        BasicEffect effect;
        Texture2D heightMap;
        Vector3 position;
        Vector3 directionBase;
        Color[] heightMapColors;
        float[,] heightData;
        float scale, yaw, pitch, roll, velocity;
        int terrainWidth, terrainHeight;
        int indexCount;



        public ClsTerreno(GraphicsDevice device, ContentManager content, ClsCamara cam)
        {
            heightMap = content.Load<Texture2D>("lh3d1");
            scale = 0.1f; yaw = 0.0f; pitch = 0.0f; roll = 0.0f; velocity = 0.01f;
            directionBase = new Vector3(1, 0, 0);

            effect = new BasicEffect(device);
            //this.worldMatrix = cam.worldMatrix;
            effect.Projection = cam.projectionMatrix;
            effect.World = cam.worldMatrix;
            effect.View = cam.viewMatrix;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;

            LoadHeightData(heightMap);
            CreateVertices(device);
            CreateIndices(device);
        }
        // Recebe um height map processando a informação para um array de floats com as alturas dos vértices a criar
        private void LoadHeightData(Texture2D heightMap)
        {
            terrainWidth = heightMap.Width;//largura da textura(128)
            terrainHeight = heightMap.Height;//altura da textura(128)

            heightMapColors = new Color[terrainWidth * terrainHeight];//(16384)
            heightMap.GetData(heightMapColors);// popular o array de cores

            heightData = new float[terrainWidth, terrainHeight];//(128,128)
            for (int x = 0; x < terrainWidth - 1; x++)
                for (int z = 0; z < terrainHeight; z++)
                    heightData[x, z] = heightMapColors[x + (z * terrainWidth)].R * scale;

        }
        // Preenche o array com os vertices
        private void CreateVertices(GraphicsDevice device)
        {
            vertex = new VertexPositionNormalTexture[(terrainHeight) * (terrainWidth)];
            for (int x = 0; x < terrainWidth - 1; x++)
            {
                for (int z = 0; z < terrainHeight; z++)
                {
                    // (16383)
                    vertex[x + (z * terrainHeight)] = new VertexPositionNormalTexture(new Vector3(x, heightData[x, z], z), new Vector3(0, 1, 0), new Vector2(0, 0));
                }
            }
            vertexBuffer = new VertexBuffer(device,
                typeof(VertexPositionNormalTexture),
                vertex.Length,
                BufferUsage.None
                );
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertex);
        }
        // creates indecies for each Triangle strip
        private void CreateIndices(GraphicsDevice device)
        {
            indexCount = 2 * terrainHeight * (terrainWidth - 1);
            index = new short[indexCount];
            for (int xi = 0; xi < terrainWidth - 1; xi++)
            {
                for (int zi = 0; zi < terrainHeight; zi++)
                {
                    index[(2 * zi) + 0 + (2 * xi * terrainHeight)] = (short)(zi * terrainWidth + xi); //128,129,130...
                    index[(2 * zi) + 1 + (2 * xi * terrainWidth)] = (short)(zi * terrainHeight + 1 + xi); //012345
                }
            }
            indexBuffer = new IndexBuffer(device,
                typeof(short),
                index.Length,
                BufferUsage.None);
            indexBuffer.SetData<short>(index);
        }
        private void Interpolation(ClsCamara cam)
        {
            Vector3 camaraPosition = cam.cameraPosition;
            // vectores perto da camera
            float xa, xb, xc, xd, za, zb, zc, zd, ya, yb, yc, yd;
            // distancias
            float da, db, dc, dd;
            // alturas na posicção x,z
            float yab, ycd, yfinal;

            xa = (int)camaraPosition.X;
            za = (int)camaraPosition.Z;
            xb = (int)xa + 1;
            zb = (int)za + 1;
            xc = (int)xa;
            zc = (int)za + 1;
            xd = (int)xa + 1;
            zd = (int)za + 1;
            ya = heightData[(int)xa, (int)za];
            yb = heightData[(int)xb, (int)zb];
            yc = heightData[(int)xc, (int)zc];
            yd = heightData[(int)xd, (int)zd];
            da = cam.cameraPosition.X - xa;
            db = 1 - da;
            dc = da;
            dd = 1 - dc;

            yab = (db * ya) + (da * yb);
            ycd = (dd * yc) + (dc * yd);

            yfinal = dc;





        }
        public void Update(KeyboardState kb, ClsCamara cam)
        {
            if (kb.IsKeyDown(Keys.Left))
            {
                yaw += MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.Right))
            {
                this.yaw -= MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.Up))
            {
                this.pitch += MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.Down))
            {
                this.pitch -= MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.A))
            {
                this.roll += MathHelper.ToRadians(1.0f);
            }
            if (kb.IsKeyDown(Keys.D))
            {
                this.roll -= MathHelper.ToRadians(1.0f);
            }
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
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
            cam.worldMatrix = Matrix.CreateTranslation(position) * rotation;
        }
        public void Draw(GraphicsDevice device, ClsCamara cam)
        {
            // World Matrix
            effect.World = cam.worldMatrix;
            //Indica o efeito para desenhar os eixos
            // Draw dos lados do prisma
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            effect.CurrentTechnique.Passes[0].Apply(); // apply das mudanças
            for (int i = 0; i < terrainWidth - 1; i++)
            {

                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    i * 2 * terrainWidth,
                    (terrainWidth * 2) - 2);
            }

        }
    }
}
