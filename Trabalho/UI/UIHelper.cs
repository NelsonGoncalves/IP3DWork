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
    static class UIHelper
    {
        #region Fields
        public static Texture2D ButtonTexture;
        public static SpriteFont ButtonFont;
        #endregion
        #region Helper Methods
        public static UIButton CreateButton(string id, string text, int x, int y)
        {
            UIButton b = new UIButton(id, new Vector2(x, y), new Vector2(25 - ButtonFont.MeasureString(text).X / 2, 10), ButtonFont, text, Color.White, ButtonTexture);
            b.Disabled = false;
            return b;
        }
        public static UIText CreateTextblock(string id, string text, int x, int y)
        {
            UIText b = new UIText(id, new Vector2(x, y), Vector2.Zero, ButtonFont, text, Color.White);
            return b;
        }
        public static void SetButtonState(string prefix, Boolean disabled, Dictionary<string, UIWidget> uiElements)
        {
            foreach (string widget in uiElements.Keys)
            {
                if (uiElements[widget].ID.StartsWith(prefix))
                    if (uiElements[widget] is UIButton)
                        ((UIButton)uiElements[widget]).Disabled = disabled;
            }
        }
        public static void SetElementVisibility(string prefix, Boolean visible, Dictionary<string, UIWidget> uiElements)
        {
            foreach (string widget in uiElements.Keys)
            {
                if (uiElements[widget].ID.StartsWith(prefix))
                    ((UIWidget)uiElements[widget]).Visible = visible;
            }
        }
        public static void SetElementText(UIWidget uiElement, string text)
        {
            if (uiElement is UIText)
                ((UIText)uiElement).Text = text;
        }
        #endregion
        #region Interface Update
        public static void UpdateUI(Dictionary<string, UIWidget> uiElements, List<ClsTank> tanks, KeyboardState keyboard, MouseState mouse)
        {
            UpdateTextBlocks(uiElements, tanks);
            if (mouse.RightButton == ButtonState.Released)
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    foreach (UIWidget widget in uiElements.Values)
                    {
                        if (widget is UIButton)
                        {
                            ((UIButton)widget).HitTest(new Point(mouse.X, mouse.Y));
                        }
                    }
                }
                else
                {
                    foreach (UIWidget widget in uiElements.Values)
                    {
                        if (widget is UIButton)
                        {
                            ((UIButton)widget).Pressed = false;
                        }
                    }
                }
            }
        }
        private static void UpdateTextBlocks(Dictionary<string, UIWidget> uiElements, List<ClsTank> tanks)
        {
            float p1Elevation = MathHelper.ToDegrees(tanks[0].CannonRotation) * -1;
            float p1Rot = MathHelper.ToDegrees(tanks[0].TurretRotation);
            p1Rot = 180 - p1Rot;
            UIHelper.SetElementText(uiElements["p1Rotation"], "Angle: " + p1Rot.ToString("N2"));
            UIHelper.SetElementText(uiElements["p1Elevation"], "Elevation: " + p1Elevation.ToString("N2"));
            //float p2Elevation = MathHelper.ToDegrees(tanks[1].TurretRotation) * -1;
            //float p2Rot = MathHelper.ToDegrees(tanks[1].TurretRotation);
            //p2Rot = 180 - p2Rot;
            //UIHelper.SetElementText(uiElements["p2Rotation"], "Angle: " + p2Rot.ToString("N2"));
            //UIHelper.SetElementText(uiElements["p2Elevation"], "Elevation: " + p2Elevation.ToString("N2"));
        }
        #endregion

    }
}
