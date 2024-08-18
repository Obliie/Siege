using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Siege.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Screens.Components {
    public class TextButton : Button {
        public string Text { get; set; }
        public Color TextColor { get; set; }

        public TextButton(ButtonAction OnDown, ButtonAction OnRelease, string Text, Color ButtonColor, Color TextColor)
            : this(OnDown, OnRelease, null, Text, ButtonColor, TextColor) { }

        public TextButton(ButtonAction OnDown, ButtonAction OnRelease, Tooltip tooltip, string Text, Color ButtonColor, Color TextColor)
            : base(null, OnDown, OnRelease, tooltip) {
            this.Texture = TextureHelper.GenerateRectangleTexture(ButtonColor);

            this.Text = Text;
            this.TextColor = TextColor;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);

            spriteBatch.Begin();

            spriteBatch.DrawString(Siege.INSTANCE.Font_14_Regular, Text, 
                new Vector2(this.XOffset + 5, this.YOffset + (Height / 4)), TextColor);

            spriteBatch.End();
        }
    }
}
