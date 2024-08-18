using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Map.Tiles;
using Siege.Entities;
using Microsoft.Xna.Framework;

namespace Siege.Tower.Towers {

    /// <summary>
    /// A type of Tower, called 'Explosive'.
    /// </summary>
    public class SpikeTower : TowerBase {
        public SpikeTower(MapBase map, bool Friendly, Tile Position)
            : base(map, Position, TowerPropertyPresets.SPIKE_T1, Friendly) { }

        /// <summary>
        /// Returns that this Tower is a TowerType of TowerType.SPIKE so that it can be identified.
        /// </summary>
        /// <returns>TowerType.SPIKE</returns>
        public override TowerType GetTowerType() {
            return TowerType.SPIKE;
        }

        /// <summary>
        /// Fires Projectiles from this Tower.
        /// </summary>
        public override void FireProjectiles() {
            // Create a Projectile template originating from the Tower's center with a texture, a damage value of 25, a damage radius of 2,
            // a speed of 1 and a lifetime of 2 seconds.
            ProjectileBuilder PBuilder = PBuilderBase.TextureID(0);

            // Fire Projectiles in the North East, North West, South East and South West directions.
            Projectiles.Add(PBuilder.Direction(Direction.NORTH).Build());
            Projectiles.Add(PBuilder.Direction(Direction.EAST).Build());
            Projectiles.Add(PBuilder.Direction(Direction.SOUTH).Build());
            Projectiles.Add(PBuilder.Direction(Direction.WEST).Build());
            Projectiles.Add(PBuilder.Direction(Direction.NORTH_EAST).Build());
            Projectiles.Add(PBuilder.Direction(Direction.SOUTH_EAST).Build());
            Projectiles.Add(PBuilder.Direction(Direction.SOUTH_WEST).Build());
            Projectiles.Add(PBuilder.Direction(Direction.NORTH_WEST).Build());
        }
    }
}
