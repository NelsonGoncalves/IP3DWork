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
        Matrix worldMatrix; // World Matrix
        BasicEffect effect; // efeito básico a usar
        Texture2D heightMap, relva; // height map e textura
        Color[] heightMapColors; // Array de cores que receberá as cores do height map
        private float[,] heightData; // Array bidimensional que receberá os valores de altura do terreno
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

            LoadHeightData(heightMap); // Metodo responsável pelos valores das alturas do terreno
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
            heightData = new float[terrainWidth, terrainHeight];//(128,128)
            // popular o array de alturas
            for (int x = 0; x < terrainWidth; x++)//(127)
                for (int z = 0; z < terrainHeight; z++) // (128)
                    // para cada posição ir buscar o valor R da cor e multiplicar por um escalar(permite facilmente manipular as alturas)
                    heightData[x, z] = heightMapColors[x + (z * terrainWidth)].R * scale;

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
                    vertex[x + (z * terrainHeight)] = new VertexPositionNormalTexture(new Vector3(
                        x, heightData[x, z], z), //Cria cada vertice com uma altura diferente acedendo ao heightData para cada (x,z)
                        new Vector3(0, 1, 0),// vector Up
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

            // valores das cordenadas dos 4 vertices vizinhos à posição da camera
            float xa, xb, xc, xd, za, zb, zc, zd, ya, yb, yc, yd;
            // distancias entre a posição da camera e a posição dos 4 vértices vizinhos
            float daX, dbX, dcX, ddX, daZ, dcZ;
            // alturas na posicção x,z
            float yab, ycd, yfinal;
            // definir as cordenadas dos pontos vizinhos
            xa = (int)cam.cameraPosition.X;
            za = (int)cam.cameraPosition.Z;
            xb = xa + 1;
            zb = za;
            xc = xa;
            zc = za + 1;
            xd = xa + 1;
            zd = za + 1;
            // alturas guardadas no array respectivo
            ya = heightData[(int)xa, (int)za];
            yb = heightData[(int)xb, (int)zb];
            yc = heightData[(int)xc, (int)zc];
            yd = heightData[(int)xd, (int)zd];
            // calcular as distancias da posição da camera em X e Z com os vertices vizinhos
            // X
            daX = cam.cameraPosition.X - xa; //0.6
            dbX = 1 - daX; // 0.4
            dcX = daX; // 0.6
            ddX = 1 - dcX; // 0.4
            // Z
            daZ = cam.cameraPosition.Z - zb;//0.8
            dcZ = 1 - daZ; //0.2
            // altura media entre a,b && c,d
            yab = (dbX * ya) + (daX * yb); // (0.4*5.4f)+(0.6*6.2f)
            ycd = (ddX * yc) + (dcX * yd); // (0.4*5.2f)+(0.6*6.0f)
            // 
            yfinal = yab * daZ + ycd * dcZ; // (0.4*5.4f)+(0.6*6.2f) * 0.8 + (0.4*5.2f)+(0.6*6.0f) * 0.2
            cam.cameraPosition.Y = yfinal + 5; // + 5 para a camera ficar à superficie o terreno
        }
        public void Update(ClsCamara cam,GameTime gameTime)
        {
            Interpolation(cam);
        }
        public void Draw(GraphicsDevice device, ClsCamara cam)
        {
            // update da View e da Projection Matrix devido ao input do utilizador
            effect.View = cam.viewMatrix;
            effect.Projection = cam.projectionMatrix;
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
