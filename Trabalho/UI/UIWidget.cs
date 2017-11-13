using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho.UI
{

    class UIWidget
    {
        #region Properties
        public string ID { get; private set; }
        public bool Visible;
        public Vector2 Position;
        #endregion
        #region Constructor
        public UIWidget(string id, Vector2 position)
        {
            this.ID = id;
            this.Position = position;
            this.Visible = false;
        }
        #endregion
        #region Virtual methods
        public virtual void Update(GameTime gameTime)
        {
            
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
        #endregion
    }

}
