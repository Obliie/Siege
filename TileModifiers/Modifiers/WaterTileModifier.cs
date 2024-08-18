using Siege.Core;
using Siege.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.TileModifiers.Modifiers {
    public class WaterTileModifier : TileModifierBase {
        public WaterTileModifier(MapBase map, Tile ApplicationLocation)
            : base(map, TilePropertyPresets.WATER, ApplicationLocation, TimeSpan.FromSeconds(2), 20) {
            Name = "Water Tile Modifier";
        }
    }
}
