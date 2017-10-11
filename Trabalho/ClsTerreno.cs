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
        Texture2D heightMap,relva;

        Color[] heightMapColors;
        float[,] heightData;
        float scale;
        int terrainWidth, terrainHeight;
        int indexCount;



        public ClsTerreno(GraphicsDevice device, ContentManager content, ClsCamara cam)
        {
            heightMap = content.Load<Texture2D>("lh3d1");
            relva = content.Load<Texture2D>("relva");

            scale = 0.1f;

            effect = new BasicEffect(device);
            //this.worldMatrix = cam.worldMatrix;
            effect.Projection = cam.projectionMatrix;
            effect.World = cam.worldMatrix;
            effect.View = cam.viewMatrix;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = relva;

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
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int z = 0; z < terrainHeight; z++)
                {
                    // (16383)
                    vertex[x + (z * terrainHeight)] = new VertexPositionNormalTexture(new Vector3(x, heightData[x, z], z), new Vector3(0, 1, 0), new Vector2(x%2, x%2));
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
        public void Interpolation(ClsCamara cam)
        {
            
            // vectores perto da camera
            float xa, xb, xc, xd, za, zb, zc, zd, ya, yb, yc, yd;
            // distancias
            float da, db, dc, dd;
            // alturas na posicção x,z
            float yab, ycd, yfinal;

            xa = (int)cam.position.X;
            za = (int)cam.position.Z;
            xb = (int)xa + 1;
            zb = (int)za;
            xc = (int)xa;
            zc = (int)za + 1;
            xd = (int)xa + 1;
            zd = (int)za + 1;
            ya = heightData[(int)xa, (int)za];
            yb = heightData[(int)xb, (int)zb];
            yc = heightData[(int)xc, (int)zc];
            yd = heightData[(int)xd, (int)zd];
            da = cam.position.X - xa;
            db = 1 - da;
            dc = da;
            dd = 1 - dc;

            yab = (db * ya) + (da * yb);
            ycd = (dd * yc) + (dc * yd);

            yfinal = dc*dd*yab + da*db*ycd;
            cam.position.Y = yfinal;
        }

        public void Draw(GraphicsDevice device, ClsCamara cam)
        {
            // World Matrix
            effect.World = cam.worldMatrix;
            //effect.View = cam.viewMatrix;
            //effect.Projection = cam.projectionMatrix;
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
                    (terrainHeight * 2) - 2);
            }
        }
    }
}
