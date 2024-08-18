using Siege.Core;
using Siege.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.TileModifiers.Modifiers {

    /// <summary>
    /// A TileModifier which temporarily sets a Tile on the map to be a Rock for 5 seconods.
    /// </summary>
    public class RockTileModifier : TileModifierBase {
        public RockTileModifier(MapBase map, Tile ApplicationLocation)
            : base(map, TilePropertyPresets.ROCK, ApplicationLocation, TimeSpan.FromSeconds(5), 25) {
            Name = "Rock Tile Modifier";
        }
    }
}
