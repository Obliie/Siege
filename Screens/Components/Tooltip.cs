using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Siege.Util;

namespace Siege.Screens.Components {
    public class Tooltip {
        public string Title { get; set; }
        public List<string> Text = new List<string>();

        public Tooltip(string Title, params string[] Text) {
            this.Title = Title;
            this.Text.AddRange(Text);
        }

        // The Offsets from which this Tooltip should be drawn.
        private int XOffsetValue = 0;
        public int XOffset { 
            get { 
                return XOffsetValue + Mouse.GetState().X; 
            } 
            set {
                this.XOffsetValue = value;
            } 
        }
        private int YOffsetValue = 0;
        public int YOffset {
            get {
                return YOffsetValue + Mouse.GetState().Y;
            }
            set {
                this.YOffsetValue = value;
            }
        }

        /// <summary>
        /// Draws this Tooltip.
        /// </summary>
        /// <param name="spriteBatch">The game's SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            spriteBatch.Draw(TextureHelper.GenerateRectangleTexture(Color.Black), new Rectangle(XOffset, YOffset, 200, (Text.Count * 15 + 20)), Color.White);

            spriteBatch.DrawString(Siege.INSTANCE.Font_14_Bold, Title, new Vector2(XOffset, YOffset), Color.Red);

            int LineOffset = 20;
            foreach (string s in Text) {
                spriteBatch.DrawString(Siege.INSTANCE.Font_14_Regular, s, new Vector2(XOffset, YOffset + LineOffset), Color.Red);
                LineOffset += 15;
            }

            spriteBatch.End();
        }
    }
}
