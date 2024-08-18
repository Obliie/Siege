using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Siege.Map.Tiles {

    /// <summary>
    /// A Tile is a single block of specific size on a Map.
    /// </summary>
    public class Tile {
        // The X coordinate of this Tile on a Map. 
        public int X { get; private set; }

        // The Y coordinate of this Tile on a Map.
        public int Y { get; private set; }

        // The Bounds of this Tile on a Map. This can be used to check for intersections.
        public Rectangle Bounds { get; private set; }

        // The Properties of this Tile.
        private TileProperties properties = new TileProperties(null, true, false, false, 1F, 0F, true);
        public TileProperties Properties {
            get { return this.properties; }
            set {
                if (this.properties.Modifyable) {
                    this.properties.TextureID = value.TextureID;
                    this.properties.Modifyable = value.Modifyable;
                    this.properties.Solid = value.Solid;
                    this.properties.TroopPath = value.TroopPath;
                    this.properties.SpeedModifier = value.SpeedModifier;
                    this.properties.DamageTick = value.DamageTick;
                    this.properties.AcceptsTileEntity = value.AcceptsTileEntity;
                }
            }
        }

        public Tile(int X, int Y, Rectangle Bounds) {
            this.X = X;
            this.Y = Y;
            this.Bounds = Bounds;
        }
    }
}
