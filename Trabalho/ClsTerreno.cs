using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho
{
    class ClsTerreno
    {
        //Globals
        VertexPositionNormalTexture[] vertex; // Array de vertices
        short[] index; // Array de indices
        VertexBuffer vertexBuffer; // buffer para os vértices
        IndexBuffer indexBuffer; // buffer para os indices
        BasicEffect effect; // efeito básico a usar
        Texture2D heightMap, relva; // height map e textura
        Color[] heightMapColors; // Array de cores que receberá as cores do height map
        private Vector3[,] heightData; // Array bidimensional que receberá os valores de altura do terreno
        private Vector3[,] normals;
        float scale; // escala para definir as alturas do terreno
        int terrainWidth, terrainHeight; // Altura e largura do terreno
        int indexCount; // counter para o numero de indices

        public ClsTerreno(GraphicsDevice device, ContentManager content, ClsCamara cam)
        {
            // Loading das texturas
            heightMap = content.Load<Texture2D>("lh3d1");
            relva = content.Load<Texture2D>("relva");
            // set da escala para a altura do terreno
            scale = 0.1f;
            // inicialização do efeito a usar
            effect = new BasicEffect(device); // instanciar
            effect.Projection = cam.projectionMatrix; // Projection matrix será universal
            effect.World = Matrix.Identity; // World Matrix representa a posição do objeto no espaço(3D)
            effect.View = cam.viewMatrix; // ViewMatrix será universal
            effect.VertexColorEnabled = false; // desligar as cores
            effect.TextureEnabled = true; // ligar as texturas
            effect.Texture = relva; // atribuir a relva como textura do terreno

            effect.LightingEnabled = true; // liga a iluminação
            effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1); // a red light
            effect.DirectionalLight0.Direction = new Vector3(1, 1, 1);  // coming along the x-axis
            effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1); // with green highlights
            //effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            //effect.EmissiveColor = new Vector3(1, 1, 0);


            LoadHeightData(heightMap); // Metodo responsável pelos valores das alturas do terreno
            CreateNormals(); // Cria as normais de cada vértice (iluminação)
            CreateVertices(device); // Metodo responsável por criar os vértices e os respectivo buffer
            CreateIndices(device); // Metodo responável por criar os indices e os respectivo buffer
        }
        // Recebe um height map processando a informação para um array de floats com as alturas dos vértices a criar
        private void LoadHeightData(Texture2D heightMap)
        {
            terrainWidth = heightMap.Width;//largura da textura(128)
            terrainHeight = heightMap.Height;//altura da textura(128)

            heightMapColors = new Color[terrainWidth * terrainHeight];//(16384)
            heightMap.GetData(heightMapColors);// popular o array de cores
            // incialização do array de alturas
            heightData = new Vector3[terrainWidth, terrainHeight];//(128,128)
            for (int z = 0; z < terrainWidth; z++)//(127)
                for (int x = 0; x < terrainHeight; x++) // (128)
                    heightData[x, z] = new Vector3(x, heightMapColors[x + (z * terrainWidth)].R * scale, z);

        }
        // Create normals
        private void CreateNormals()
        {
            Vector3 v1, v2, v3, v4, v5, v6, v7, v8;
            Vector3 n, n1, n2, n3, n4, n5, n6, n7, n8;
            normals = new Vector3[terrainWidth, terrainHeight];
            // centro
            for (int z = 0; z < terrainWidth; z++)
            {
                for (int x = 0; x < terrainHeight; x++)
                {
                    if (z == 0 && x == 0)
                    {
                        // back left corner - 2 tri 3 vertices
                        v1 = heightData[x, z + 1] - heightData[x, z];
                        v2 = heightData[x + 1, z + 1] - heightData[x, z];
                        v3 = heightData[x + 1, z] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3);
                        n = (n1 + n2) / 2.0f;
                    }
                    else if ((z > 0 && z < (terrainWidth - 1)) && x == 0)
                    {
                        // left edge - 4 tri 5 vertices
                        v1 = heightData[x, z - 1] - heightData[x, z];
                        v2 = heightData[x + 1, z - 1] - heightData[x, z];
                        v3 = heightData[x + 1, z] - heightData[x, z];
                        v4 = heightData[x + 1, z + 1] - heightData[x, z];
                        v5 = heightData[x, z + 1] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3); n3 = Vector3.Cross(v3, v4); n4 = Vector3.Cross(v4, v5);
                        n = (n1 + n2 + n3 + n4) / 4.0f;
                    }
                    else if (z == (terrainHeight - 1) && x == 0)
                    {
                        // front left corner - 2 tri 3 vertices
                        v1 = heightData[x, z - 1] - heightData[x, z];
                        v2 = heightData[x + 1, z - 1] - heightData[x, z];
                        v3 = heightData[x + 1, z] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3);
                        n = (n1 + n2) / 2.0f;
                    }
                    else if (z == (terrainHeight - 1) && (x > 0 && x < (terrainWidth - 1)))
                    {
                        // front edge - 4 tri 5 vertices
                        v1 = heightData[x - 1, z] - heightData[x, z];
                        v2 = heightData[x - 1, z - 1] - heightData[x, z];
                        v3 = heightData[x, z - 1] - heightData[x, z];
                        v4 = heightData[x + 1, z - 1] - heightData[x, z];
                        v5 = heightData[x + 1, z] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3); n3 = Vector3.Cross(v3, v4); n4 = Vector3.Cross(v4, v5);
                        n = (n1 + n2 + n3) / 3.0f;
                    }
                    else if (z == (terrainHeight - 1) && x == (terrainWidth - 1))
                    {
                        // front right corner - 2 tri 3 vertices
                        v1 = heightData[x - 1, z] - heightData[x, z];
                        v2 = heightData[x - 1, z - 1] - heightData[x, z];
                        v3 = heightData[x - 1, z] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3);
                        n = (n1 + n2) / 2.0f;
                    }
                    else if ((z > 0 && z < (terrainHeight - 1)) && x == (terrainWidth - 1))
                    {
                        // right edge - 4 tri 5 vertices
                        v1 = heightData[x, z - 1] - heightData[x, z];
                        v2 = heightData[x - 1, z - 1] - heightData[x, z];
                        v3 = heightData[x - 1, z] - heightData[x, z];
                        v4 = heightData[x - 1, z + 1] - heightData[x, z];
                        v5 = heightData[x, z + 1] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3); n3 = Vector3.Cross(v3, v4); n4 = Vector3.Cross(v4, v5);
                        n = (n1 + n2 + n3 + n4) / 4.0f;
                    }
                    else if (z == 0 && x == (terrainWidth - 1))
                    {
                        // back right corner - 2 tri 3 vertices
                        v1 = heightData[x - 1, z] - heightData[x, z];
                        v2 = heightData[x - 1, z + 1] - heightData[x, z];
                        v3 = heightData[x, z + 1] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3);
                        n = (n1 + n2) / 2.0f;
                    }
                    else if (z == 0 && (x > 0 && x < (terrainWidth - 1)))
                    {
                        // back edge - 4 tri 5 vertices
                        v1 = heightData[x - 1, z] - heightData[x, z];
                        v2 = heightData[x - 1, z + 1] - heightData[x, z];
                        v3 = heightData[x, z + 1] - heightData[x, z];
                        v4 = heightData[x + 1, z + 1] - heightData[x, z];
                        v5 = heightData[x + 1, z] - heightData[x, z];
                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3); n3 = Vector3.Cross(v3, v4); n4 = Vector3.Cross(v4, v5);
                        n = (n1 + n2 + n3 + n4) / 4.0f;
                    }
                    else
                    {
                        // internal - 8 tri 8 vertices
                        v1 = heightData[x, z - 1] - heightData[x, z];
                        v2 = heightData[x + 1, z - 1] - heightData[x, z];
                        v3 = heightData[x + 1, z] - heightData[x, z];
                        v4 = heightData[x + 1, z + 1] - heightData[x, z];
                        v5 = heightData[x, z + 1] - heightData[x, z];
                        v6 = heightData[x - 1, z + 1] - heightData[x, z];
                        v7 = heightData[x - 1, z] - heightData[x, z];
                        v8 = heightData[x - 1, z - 1] - heightData[x, z];

                        n1 = Vector3.Cross(v1, v2); n2 = Vector3.Cross(v2, v3); n3 = Vector3.Cross(v3, v4);
                        n4 = Vector3.Cross(v4, v5); n5 = Vector3.Cross(v5, v6); n6 = Vector3.Cross(v6, v7);
                        n7 = Vector3.Cross(v7, v8); n8 = Vector3.Cross(v8, v1);
                        n = (n1 + n2 + n3 + n4 + n5 + n6 + n7 + n8) / 8.0f;
                    }
                    n.Normalize();
                    normals[x, z] = new Vector3(n.X, n.Y, n.Z);
                }
            }
        }
        // Preenche o array com os vertices
        private void CreateVertices(GraphicsDevice device)
        {
            // inicializa o array de texturas
            vertex = new VertexPositionNormalTexture[(terrainHeight) * (terrainWidth)];//(128*128)

            for (int z = 0; z < terrainWidth; z++)
            {
                for (int x = 0; x < terrainHeight; x++)
                {
                    vertex[x + (z * terrainHeight)] = new VertexPositionNormalTexture(
                        new Vector3(x, heightData[x, z].Y, z), //Cria cada vertice com uma altura diferente acedendo ao heightData para cada (x,z)
                        normals[x, z],// vector Up
                        new Vector2((x % 2), (z % 2))); // x: pares serão sempre 0 e impares 1
                }
            }
            // set do vertexBuffer
            vertexBuffer = new VertexBuffer(device,
                typeof(VertexPositionNormalTexture),
                vertex.Length,
                BufferUsage.None
                );
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertex); // atribuir vertex como o array de vértices a usar na Draw
        }
        // creates indecies for each Triangle strip
        private void CreateIndices(GraphicsDevice device)
        {
            // numero de indices necessários para ligar todos os vértices com triangle strips
            indexCount = 2 * terrainHeight * (terrainWidth - 1);
            // incialização do array de indices
            index = new short[indexCount]; // (32512)
            for (int xi = 0; xi < terrainWidth - 1; xi++)//(127)
            {
                for (int zi = 0; zi < terrainHeight; zi++)
                {
                    index[(2 * zi) + 0 + (2 * xi * terrainHeight)] = (short)(zi * terrainWidth + xi); //0,128,256,...,16382
                    index[(2 * zi) + 1 + (2 * xi * terrainWidth)] = (short)(zi * terrainHeight + 1 + xi); //1,129,257,...,16383
                }
            }
            // set do indexBuffer
            indexBuffer = new IndexBuffer(device,
                typeof(short),
                index.Length,
                BufferUsage.None);
            indexBuffer.SetData<short>(index); // atribuir o array de indices a usar na Draw
        }
        // Faz a interpolação das alturas
        public void Interpolation(ClsCamara cam)
        {
            //Debug.WriteLine("X:" + cam.cameraPosition.X + "Y:" + cam.cameraPosition.Y + "Z:" + cam.cameraPosition.Z);
            //Debug.WriteLine("DirectionX" + cam.cameraTarget.X + "DirectionY:" + cam.cameraTarget.Y + "DirectionZ:" + cam.cameraTarget.Z);
            //Debug.WriteLine("camY:" + cam.cameraPosition.Y);
            // definir as cordenadas dos pontos vizinhos
            int x = (int)cam.cameraPosition.X;
            int z = (int)cam.cameraPosition.Z;
            // valores das cordenadas dos 4 vertices vizinhos à posição da camera 
            // alturas na posicção x,z
            float ya = heightData[x, z].Y;
            float yb = heightData[x + 1, z].Y;
            float yc = heightData[x, z + 1].Y;
            float yd = heightData[x + 1, z + 1].Y;
            // distancias entre a posição da camera e a posição dos 4 vértices vizinhos
            // calcular as distancias da posição da camera em X e Z com os vertices vizinhos
            // X
            float daX = cam.cameraPosition.X - x; //0.6
            float dbX = 1 - daX; // 0.4
            float dcX = daX; // 0.6
            float ddX = 1 - dcX; // 0.4
                                 // Z
            float daZ = cam.cameraPosition.Z - z;//0.8
            float dcZ = 1 - daZ; //0.2
                                 // altura media entre a,b && c,d
            float yab = (dbX * ya) + (daX * yb); // (0.4*5.4f)+(0.6*6.2f)
            float ycd = (ddX * yc) + (dcX * yd); // (0.4*5.2f)+(0.6*6.0f)
                                                 // 
            float yfinal = yab * dcZ + ycd * daZ; // (0.4*5.4f)+(0.6*6.2f) * 0.8 + (0.4*5.2f)+(0.6*6.0f) * 0.2
            cam.cameraPosition.Y = yfinal + 5; // + 5 para a camera ficar à superficie o terreno
        }
        public void Update(ClsCamara cam, GameTime gameTime)
        {
            Interpolation(cam);
        }
        public void Draw(GraphicsDevice device, ClsCamara cam)
        {
            // update da View e da Projection Matrix devido ao input do utilizador
            effect.View = cam.viewMatrix;
            // definir o vertexBuffer e indexBuffer a usar;
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            effect.CurrentTechnique.Passes[0].Apply(); // apply das mudanças
            for (int i = 0; i < terrainWidth - 1; i++)
            {
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0, // offset dos vértices
                    i * 2 * terrainWidth, // offset para desenhar as 127 strips
                    (terrainWidth * 2) - 2); // numero de triangulos a desenhar. Largura*2 - 2 porque os dois primeiros vértices não desenham triangulo
            }
        }
    }
}
