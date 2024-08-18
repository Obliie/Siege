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
    /// A type of Tower, called 'Ice'.
    /// </summary>
    public class IceTower : TowerBase {
        public IceTower(MapBase map, bool Friendly, Tile Position)
            : base(map, Position, TowerPropertyPresets.ICE_T1, Friendly) { }

        /// <summary>
        /// Returns that this Tower is a TowerType of TowerType.ICE so that it can be identified.
        /// </summary>
        /// <returns>TowerType.ICE</returns>
        public override TowerType GetTowerType() {
            return TowerType.ICE;
        }

        /// <summary>
        /// Fires Projectiles from this Tower.
        /// </summary>
        public override void FireProjectiles() {
            // Extend the projectile builder already defined in the TowerBase by adding a Texture and HitAction which
            // slows down the speed of a Troop by up to a maximumm of half.
            ProjectileBuilder PBuilder = PBuilderBase
                .TextureID(2)
                .HitAction(Troops => Troops.ForEach(Troop => {
                    if (Troop.SpeedModifier > 0.5F) Troop.SpeedModifier *= 0.8F;
                }));

            // Fire Projectiles in the North, South East and South West directions.
            Projectiles.Add(PBuilder.Direction(Direction.NORTH).Build());
            Projectiles.Add(PBuilder.Direction(Direction.SOUTH_EAST).Build());
            Projectiles.Add(PBuilder.Direction(Direction.SOUTH_WEST).Build());
        }
    }
}
