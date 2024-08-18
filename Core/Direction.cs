using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Siege.Core {

    /// <summary>
    /// A class storing the Vector2 directions of cardinal directions for easy use.
    /// </summary>
    public static class Direction {
        public static Vector2 NONE = new Vector2(0, 0);
        public static Vector2 NORTH = new Vector2(0, -1);
        public static Vector2 NORTH_EAST = new Vector2(1, -1);
        public static Vector2 EAST = new Vector2(1, 0);
        public static Vector2 SOUTH_EAST = new Vector2(1, 1);
        public static Vector2 SOUTH = new Vector2(0, 1);
        public static Vector2 SOUTH_WEST = new Vector2(-1, 1);
        public static Vector2 WEST = new Vector2(-1, 0);
        public static Vector2 NORTH_WEST = new Vector2(-1, -1);
    }
}
