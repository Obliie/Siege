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
    /// A type of Tower, called 'Basic'.
    /// </summary>
    public class BasicTower: TowerBase {
        public BasicTower(MapBase map, bool Friendly, Tile Position)
            : base(map, Position, TowerPropertyPresets.BASIC_T1, Friendly) { }

        /// <summary>
        /// Returns that this Tower is a TowerType of TowerType.BASIC so that it can be identified.
        /// </summary>
        /// <returns>TowerType.BASIC</returns>
        public override TowerType GetTowerType() {
            return TowerType.BASIC;
        }

        /// <summary>
        /// Fires Projectiles from this Tower.
        /// </summary>
        public override void FireProjectiles() {
            // Extend the projectile builder already defined in the TowerBase by adding a Texture.
            ProjectileBuilder PBuilder = PBuilderBase.TextureID(0);

            // Fire Projectiles in the North, East, South and West directions.
            Projectiles.Add(PBuilder.Direction(Direction.NORTH).Build());
            Projectiles.Add(PBuilder.Direction(Direction.EAST).Build());
            Projectiles.Add(PBuilder.Direction(Direction.SOUTH).Build());
            Projectiles.Add(PBuilder.Direction(Direction.WEST).Build());
        }
    }
}
