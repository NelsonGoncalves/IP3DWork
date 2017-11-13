using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho.UI
{
    class UIText : UIWidget
    {
        #region Properties
        public Vector2 TextOffset;
        public SpriteFont Font;
        public string Text;
        public Color TextColor;
        #endregion
        #region Constructor
        public UIText(string id, Vector2 position, Vector2 textOffset, SpriteFont font, string text, Color textColor) : base(id, position)
        {
            TextOffset = textOffset;
            Font = font;
            Text = text;
            TextColor = textColor;
        }
        #endregion
        #region Draw
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.DrawString(
                Font,
                Text,
                Position + TextOffset,
                TextColor);
            }
            base.Draw(spriteBatch);
        }
        #endregion
    }
}
