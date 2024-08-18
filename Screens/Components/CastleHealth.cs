using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Siege.Util;
using Siege.Events;

namespace Siege.Screens.Components {
    public class CastleHealth : IScreenComponent {
        private MapBase Map;
        private Texture2D HealthBarTexture;

        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int MaxPlayerCastleHealth { get; set; }
        public int MaxEnemyCastleHealth { get; set; }
        public double PlayerCastleHealth { get; set; }
        public double EnemyCastleHealth { get; set; }

        public CastleHealth(MapBase Map, int MaxPlayerCastleHealth, int MaxEnemyCastleHealth) {
            this.Map = Map;

            this.MaxPlayerCastleHealth = MaxPlayerCastleHealth;
            this.PlayerCastleHealth = MaxPlayerCastleHealth;

            this.MaxEnemyCastleHealth = MaxEnemyCastleHealth;
            this.EnemyCastleHealth = MaxEnemyCastleHealth;

            Map.CastleDamage += OnCastleDamage;
        }

        public void LoadContent(ContentManager Content) {
            this.HealthBarTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_CASTLE_HEALTHBAR_TEXTURE);
        }

        public void UnloadContent() {
            this.HealthBarTexture = null;
        }

        public void Update(GameTime gameTime) {
            PlayerCastleHealth = MathHelper.Clamp((float)PlayerCastleHealth, 0F, (float)MaxPlayerCastleHealth);
            EnemyCastleHealth = MathHelper.Clamp((float)EnemyCastleHealth, 0F, (float)MaxEnemyCastleHealth);
        }

        public void OnCastleDamage(object sender, CastleDamageEventArgs args) {
            if (args.Cause.Friendly) {
                EnemyCastleHealth -= args.DamageAmount;
            } else {
                PlayerCastleHealth -= args.DamageAmount;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            if (PlayerCastleHealth > 0) {
                int PlayerHealthBarWidth = (int)Math.Ceiling(HealthBarTexture.Width * (PlayerCastleHealth / (double)MaxPlayerCastleHealth));
                spriteBatch.Draw(TextureHelper.GenerateRectangleTexture(Color.Green),
                    new Rectangle(XOffset, YOffset, PlayerHealthBarWidth, HealthBarTexture.Height),
                    Color.White);
            } else {
                string PlayerHealthText = "DEAD";
                spriteBatch.DrawString(Siege.INSTANCE.Font_14_Bold, PlayerHealthText, new Vector2(XOffset + ((HealthBarTexture.Width / 2) - ((PlayerHealthText.Length * 5) / 2)), HealthBarTexture.Height / 4), Color.Black);
            }

            if (EnemyCastleHealth > 0) {
                int EnemyHealthBarWidth = (int)Math.Ceiling(HealthBarTexture.Width * (EnemyCastleHealth / (double)MaxEnemyCastleHealth));
                spriteBatch.Draw(TextureHelper.GenerateRectangleTexture(Color.Red),
                    new Rectangle((Map.Width * Map.TileWidth) - HealthBarTexture.Width + XOffset + (HealthBarTexture.Width - EnemyHealthBarWidth), 0 + YOffset, EnemyHealthBarWidth, HealthBarTexture.Height),
                    Color.White);
            } else {
                string EnemyHealthText = "DEAD";
                spriteBatch.DrawString(Siege.INSTANCE.Font_14_Bold, EnemyHealthText, new Vector2((Map.Width * Map.TileWidth) - HealthBarTexture.Width + XOffset + ((HealthBarTexture.Width / 2) - (EnemyHealthText.Length * 5) / 2), HealthBarTexture.Height / 4), Color.Black);
            }

            spriteBatch.Draw(HealthBarTexture,
                new Rectangle(XOffset, YOffset, HealthBarTexture.Width, HealthBarTexture.Height),
                Color.White);
            spriteBatch.Draw(HealthBarTexture,
                new Rectangle((Map.Width * Map.TileWidth) - HealthBarTexture.Width + XOffset, YOffset, HealthBarTexture.Width, HealthBarTexture.Height), 
                null,
                Color.White, 
                0F, 
                Vector2.Zero, 
                SpriteEffects.FlipHorizontally, 
                0F);

            spriteBatch.End();
        }
    }
}
