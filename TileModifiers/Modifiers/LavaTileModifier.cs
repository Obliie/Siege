using Siege.Core;
using Siege.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.TileModifiers.Modifiers {
    public class LavaTileModifier : TileModifierBase {
        public LavaTileModifier(MapBase map, Tile ApplicationLocation)
            : base(map, TilePropertyPresets.LAVA, ApplicationLocation, TimeSpan.FromSeconds(1), 40) {
            Name = "Lava Tile Modifier";
        }
    }
}
