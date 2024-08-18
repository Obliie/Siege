using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Map.Tiles {

    /// <summary>
    /// A set of default property presets for Tiles allowing for easy creation of Tiles
    /// with default properties.
    /// </summary>
    public class TilePropertyPresets {
        public static TileProperties PATH = new TileProperties(0, true, false, true, 1.5F, 0F, false);
        public static TileProperties LAVA = new TileProperties(1, true, false, false, 0.5F, 10F, false);
        public static TileProperties GRASS = new TileProperties(2, true, false, false, 0.8F, 0F, true);
        public static TileProperties WATER = new TileProperties(3, true, false, false, 0.5F, 0F, true);
        public static TileProperties CASTLE = new TileProperties(null, false, true, true, 0.1F, 2F, false);
        public static TileProperties CASTLE_ENTERANCE = new TileProperties(4, false, false, true, 1.0F, 5F, false);
        public static TileProperties ROCK = new TileProperties(5, true, true, false, 1.0F, 0F, false);
    }
}
