using Microsoft.Xna.Framework;
using Siege.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Entities.Troops {

    /// <summary>
    /// A type of Troop, called 'Heavy'.
    /// </summary>
    public class HeavyTroop : TroopBase {
        public HeavyTroop(MapBase map, Vector2 Position, bool Friendly, float? Health = null)
            // Sets the Troop to have the provided map and position with a TextureID of 0 and a speed of 2 pixels per update.
            : base(map, 2, 0.5F, Friendly, 20, 500, Position, 2, Health) { }

        /// <summary>
        /// Returns that this Troop is a TroopType of TroopType.HEAVY so that it can be identified.
        /// </summary>
        /// <returns>TroopType.HEAVY</returns>
        public override TroopType GetTroopType() {
            return TroopType.HEAVY;
        }
    }
}
