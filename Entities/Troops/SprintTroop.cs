using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Siege.Core;

namespace Siege.Entities.Troops {
    public class SprintTroop : TroopBase {
        public SprintTroop(MapBase map, Vector2 Position, bool Friendly, float? Health = null)
            // Sets the Troop to have the provided map and position with a TextureID of 1 and a speed of 5 pixels per update.
            : base(map, 1, 5F, Friendly, 10, 50, Position, 1, Health) { }

        /// <summary>
        /// Returns that this Troop is a TroopType of TroopType.SPRINTER so that it can be identified.
        /// </summary>
        /// <returns>TroopType.SPRINTER</returns>
        public override TroopType GetTroopType() {
            return TroopType.SPRINTER;
        }
    }
}
