using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho.UI
{
    class UIButtonArgs:System.EventArgs
    {
        #region Properties
        public Vector2 Location { get; private set; }
        public string ID { get; private set; }
        #endregion
        #region Constructor
        public UIButtonArgs(string id, Vector2 location)
        {
            this.ID = id;
            this.Location = location;
        }
        #endregion
    }
}
