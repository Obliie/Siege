using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Siege.Map.Tiles {
    
    /// <summary>
    /// A TileRow is a collection of Tiles which form a Row in the X coordinate plane.
    /// </summary>
    public class TileRow {
        // The Width of a single Tile on this Row.
        public int TileWidth { get; private set; }

        // The Height of a single Tile on this Row.
        public int TileHeight { get; private set; }

        // The Y coordinate of this TileRow on a Map.
        public int Y { get; set; }

        // The collection of Tiles which form this Row sorted with their X coordinate.
        public Dictionary<int, Tile> Row = new Dictionary<int, Tile>();

        public TileRow(int Y, int TileWidth, int TileHeight) {
            this.Y = Y;
            this.TileWidth = TileWidth;
            this.TileHeight = TileHeight;
        }

        /// <summary>
        /// Gets the Tile at the specified X coordinate on this Row.
        /// </summary>
        /// <param name="X">The X coordinate of which to retrieve a Tile from.</param>
        /// <returns>The Tile if it exists, else null.</returns>
        public Tile GetTileAtX(int X) {
            Tile tile;

            Row.TryGetValue(X, out tile);

            return tile;
        }

        /// <summary>
        /// Creates a Tile at the specified X coordinate on this Row, if one already exists it is replaced.
        /// </summary>
        /// <param name="X">The X coordinate at which to create a Tile.</param>
        /// <returns>The newly created Tile.</returns>
        public Tile CreateTileAtX(int X) {
            Rectangle Bounds = new Rectangle((X - 1) * TileWidth, (Y - 1) * TileHeight, TileWidth, TileHeight);
            Tile tile = new Tile(X, this.Y, Bounds);

            Row.Add(X, tile);

            return tile;
        }
    }
}
