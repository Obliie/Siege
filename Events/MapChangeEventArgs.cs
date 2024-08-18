using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Map.Tiles;

namespace Siege.Events {
    
    public delegate void MapChangeEventHandler(object sender, MapChangeEventArgs args);

    public class MapChangeEventArgs : EventArgs {
        public readonly Tile tile;

        public MapChangeEventArgs(Tile tile) {
            this.tile = tile;
        }
    }
}
