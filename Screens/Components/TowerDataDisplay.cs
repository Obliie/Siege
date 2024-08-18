using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Siege.Tower;
using Siege.Util;

namespace Siege.Screens.Components {
    public class TowerDataDisplay : IScreenComponent {
        private MapBase Map;

        public int ActualXOffset;
        public int XOffset { 
            get { 
                return Map.XOffset + ActualXOffset; 
            } 
            set { 
                this.ActualXOffset = value; 
            } 
        }
        public int ActualYOffset;
        public int YOffset { 
            get { 
                return Map.YOffset + ActualYOffset; 
            } 
            set { 
                this.ActualYOffset = value; 
            } 
        }

        public TowerBase SelectedTower { get; set; }

        public TowerDataDisplay(MapBase Map) {
            this.Map = Map;
        }

        public void LoadContent(ContentManager Content) { }

        public void UnloadContent() { }

        public void Update(GameTime gameTime) {
            
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            if (SelectedTower != null) {
                spriteBatch.Draw(
                    Map.TowerTextures,
                    new Rectangle(XOffset + Map.TileWidth, YOffset + Map.TileHeight, Map.TileWidth, Map.TileHeight),
                    new Rectangle(SelectedTower.TextureID.Value * Map.TileWidth, 0, Map.TileWidth, Map.TileHeight),
                    Color.White);
            }

            spriteBatch.End();
        }
    }
}
