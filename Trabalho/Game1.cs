using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Trabalho.UI;

namespace Trabalho
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Properties
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ClsCamara cam;
        ClsTerreno terrain;
        KeyboardState keyboard;
        MouseState mouse;
        ClsEixos eixos;
        List<ClsTank> tanks = new List<ClsTank>();
        Dictionary<string, UIWidget> uiElements;
        #endregion
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;

            cam = new ClsCamara(this, GraphicsDevice);

            eixos = new ClsEixos(GraphicsDevice, cam);

            terrain = new ClsTerreno(GraphicsDevice, Content, cam);
            //StartNewRound();
            tanks.Add(new ClsTank(GraphicsDevice, Content, new Vector3(64, 30, 64)));
            base.Initialize();
        }
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            UIHelper.ButtonTexture = Content.Load<Texture2D>("buttonUI");
            UIHelper.ButtonFont = Content.Load<SpriteFont>("pericles14");
            CreateUIElements();
            UIHelper.SetElementVisibility("p", true, uiElements);
            // TODO: use this.Content to load your game content here
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            //UIHelper.UpdateUI(uiElements, tanks, keyboard, mouse); //UI
            cam.Update(keyboard, mouse, terrain); // cam
            foreach (ClsTank tank in tanks) // tanks
                tank.Update(keyboard, terrain, gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            // TODO: Add your drawing code here   
            //UI


            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            terrain.Draw(GraphicsDevice, cam);
            foreach (ClsTank tank in tanks)
                tank.DrawModel(cam);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            spriteBatch.Begin();
            foreach (UIWidget widget in uiElements.Values)
                widget.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            base.Draw(gameTime);

        }
        public void StartNewRound()
        {
            Random rand = new Random();
            tanks.Clear();
            Vector3 p1Position = new Vector3(rand.Next(8, 56), 0, rand.Next(8, 56));
            Vector3 p2Position = new Vector3(rand.Next(8, 56), 0, rand.Next(8, 56));
            int p1Quadrant = rand.Next(0, 4);
            switch (p1Quadrant)
            {
                case 0:
                    p2Position += new Vector3(64, 0, 64);
                    break;
                case 1:
                    p1Position += new Vector3(64, 0, 0);
                    p2Position += new Vector3(0, 0, 64);
                    break;
                case 2:
                    p1Position += new Vector3(0, 0, 64);
                    p2Position += new Vector3(64, 0, 0);
                    break;
                case 3:
                    p1Position += new Vector3(64, 0, 64);
                    break;
            }
            tanks.Add(
            new ClsTank(
            GraphicsDevice,
            Content,
            p1Position));
            tanks.Add(
            new ClsTank(
            GraphicsDevice,
            Content,
            p2Position));
        }
        #region Interface area
        public void CreateUIElements()
        {
            uiElements = new Dictionary<string, UIWidget>();
            uiElements.Add("p1Up",
            UIHelper.CreateButton("p1Up", "W", 60, 10));
            uiElements.Add("p1Down",
            UIHelper.CreateButton("p1Down", "S", 60, 65));
            uiElements.Add("p1Left",
            UIHelper.CreateButton("p1Left", "A", 5, 35));
            uiElements.Add("p1Right",
            UIHelper.CreateButton("p1Right", "D", 115, 35));
            uiElements.Add("p1Fire",
            UIHelper.CreateButton("p1Fire", "Fire", 175, 35));
            uiElements.Add("p1Rotation",
            UIHelper.CreateTextblock("p1Rotation", "x", 5, 120));
            uiElements.Add("p1Elevation",
            UIHelper.CreateTextblock("p1Elevation", "x", 5, 135));
            uiElements.Add("p2Up",
            UIHelper.CreateButton("p2Up", "8", 685, 10));
            uiElements.Add("p2Down",
            UIHelper.CreateButton("p2Down", "5", 685, 65));
            uiElements.Add("p2Left",
            UIHelper.CreateButton("p2Left", "4", 630, 35));
            uiElements.Add("p2Right",
            UIHelper.CreateButton("p2Right", "6", 740, 35));
            uiElements.Add("p2Fire",
            UIHelper.CreateButton("p2Fire", "Fire", 570, 35));
            uiElements.Add("p2Rotation",
            UIHelper.CreateTextblock("p2Rotation", "x", 580, 120));
            uiElements.Add("p2Elevation",
            UIHelper.CreateTextblock("p2Elevation", "x", 580, 135));
            foreach (UIWidget widget in uiElements.Values)
            {
                if (widget is UIButton)
                {
                    ((UIButton)widget).Clicked += new UIButton.ClickHandler(UIButton_Clicked);
                }
            }
        }
        void UIButton_Clicked(object sender, UIButtonArgs e)
        {
            int playerNumber = int.Parse(e.ID.Substring(1, 1)) - 1;
            string buttonName = e.ID.Substring(2);
            switch (buttonName)
            {
                case "Left":
                    tanks[playerNumber].TurretRotation += 0.01f;
                    break;
                case "Right":
                    tanks[playerNumber].TurretRotation -= 0.01f;
                    break;
                case "Up":
                    tanks[playerNumber].CannonRotation -= 0.01f;
                    break;
                case "Down":
                    tanks[playerNumber].CannonRotation += 0.01f;
                    break;
                case "Fire":
                    break;
            }
        }
        #endregion

    }
}
