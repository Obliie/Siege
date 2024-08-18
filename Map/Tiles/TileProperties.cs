using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Map.Tiles {

    /// <summary>
    /// Properties which a Tile may or may not possess.
    /// </summary>
    public class TileProperties {
        // Gets the ID of this Tiles texture, if it has one.
        public int? TextureID { get; set; }

        public bool Modifyable { get; set; }

        // Gets if this Tile is solid.
        public bool Solid { get; set; }
        
        // Gets if a Troop can travel on this Tile.
        public bool TroopPath { get; set; }

        // The speed modification applied to any ground entities.
        public float SpeedModifier { get; set; }

        // The damage ground entities take per second being on this Tile.
        public float DamageTick { get; set; }

        public bool AcceptsTileEntity { get; set; }

        // The weight of this Tile for pathfinding based on its properties.
        public float EdgeWeight {
            get {
                float weight = (DamageTick + 1) / SpeedModifier;
                if (TroopPath) weight /= 10;
                if (Solid) weight *= 50;
                return weight * 100;
            }
        }

        public TileProperties(int? TextureID, bool Modifyable, bool Solid, bool TroopPath, float SpeedModifier, float DamageTick, bool AcceptsTileEntity) {
            this.TextureID = TextureID;
            this.Modifyable = Modifyable;
            this.Solid = Solid;
            this.TroopPath = TroopPath;
            this.SpeedModifier = SpeedModifier;
            this.DamageTick = DamageTick;
            this.AcceptsTileEntity = AcceptsTileEntity;
        }

        public TileProperties Clone() {
            return new TileProperties(TextureID, Modifyable, Solid, TroopPath, SpeedModifier, DamageTick, AcceptsTileEntity);
        }
    }
}
