using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Siege.Core;

namespace Siege.Entities.Troops {

    /// <summary>
    /// A type of Troop, called 'Basic'.
    /// </summary>
    public class BasicTroop : TroopBase {
        public BasicTroop(MapBase map, Vector2 Position, bool Friendly, float? Health = null)
            // Sets the Troop to have the provided map and position with a TextureID of 0 and a speed of 2 pixels per update.
            : base(map, 0, 2F, Friendly, 10, 200, Position, 2, Health) { }

        /// <summary>
        /// Returns that this Troop is a TroopType of TroopType.BASIC so that it can be identified.
        /// </summary>
        /// <returns>TroopType.BASIC</returns>
        public override TroopType GetTroopType() {
            return TroopType.BASIC;
        }
    }
}
