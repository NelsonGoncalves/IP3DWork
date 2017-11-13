using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho.UI
{
    class UIButton : UIText
    {
        #region Properties
        public Texture2D Texture;
        public bool Disabled;
        public bool Pressed;
        public Rectangle Bounds { get; private set; }
        #endregion
        #region Event-related Items
        public delegate void ClickHandler(object sender, UIButtonArgs e);
        public event ClickHandler Clicked;
        #endregion
        #region Constructor
        public UIButton(string id, Vector2 position, Vector2 textOffset, SpriteFont font, string text, Color textColor, Texture2D texture)
         : base(id, position, textOffset, font, text, textColor)
        {
            Texture = texture;
            this.Bounds = new Rectangle((int)position.X, (int)position.Y, Texture.Width, Texture.Height / 3);
        }
        #endregion
        #region Helper Methods
        public bool Contains(Point location)
        {
            return Visible && Bounds.Contains(location);
        }

        public bool Contains(Vector2 location)
        {
            return Contains(new Point((int)location.X, (int)location.Y));
        }
        public void HitTest(Point location)
        {
            if (Visible && !Disabled)
            {
                if (Contains(location))
                {
                    Pressed = true;
                    Clicked(
                    this,
                    new UIButtonArgs(
                    this.ID,
                    new Vector2(location.X, location.Y)));
                }
                else
                {
                Pressed = false;
                }
            }
        }
        #endregion
        #region Draw
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                Point drawBase = Point.Zero;
                if (Disabled)
                    drawBase = new Point(0, Bounds.Height);
                if (Pressed)
                    drawBase = new Point(0, Bounds.Height * 2);
                spriteBatch.Draw(Texture,Position,new Rectangle(drawBase.X, drawBase.Y,Bounds.Width, Bounds.Height),Color.White);
            }
            base.Draw(spriteBatch);
        }
        #endregion
    }
}
