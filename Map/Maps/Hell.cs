using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Siege.Entities;
using Siege.Map.Tiles;
using Siege.Tower;
using Siege.Tower.Towers;
using Siege.Entities.Troops;

namespace Siege.Map.Maps {

    /// <summary>
    /// The 'Hell' map
    /// </summary>
    public class Hell : MapBase {
        public Hell(int TileWidth, int TileHeight)
            // Sets the map to have the provided Tile width and height and the Players starting position to be [40, 134].
            : base(TileWidth, TileHeight, new Vector2(40, 134)) {

            // Sets the size of this map to be 20x10 (In Tiles).
            Height = 10;
            Width = 20;

            // Sets the positions at which Troops spawn on this map.
            PlayerTroopSpawnPosition = new Vector2(1, 6);
            EnemyTroopSpawnPosition = new Vector2(20, 5);

            PlayerCastlePosition = new Vector2(1, 6);
            EnemyCastlePosition = new Vector2(20, 6);

            // Permits the use of Basic, Explosive, Ice and Spike Towers on this map.
            AddPermittedTower(TowerType.BASIC);
            AddPermittedTower(TowerType.EXPLOSIVE);
            AddPermittedTower(TowerType.ICE);
            AddPermittedTower(TowerType.SPIKE);

            // Permits the use of Basic, Sprinter and Heavy Troops on this map.
            AddPermittedTroop(TroopType.BASIC);
            AddPermittedTroop(TroopType.SPRINTER);
            AddPermittedTroop(TroopType.HEAVY);
        }

        /// <summary>
        /// Returns that this Map is a MapType of HELL so that it can be identified
        /// by the MapService.
        /// </summary>
        /// <returns>MapType.HELL</returns>
        public override MapType GetMapType() {
            return MapType.HELL;
        }

        /// <summary>
        /// Initializes the map and sets the default Tile types.
        /// </summary>
        public override void Initialize() {
            base.Initialize();

            // Set the entire map to be grass.
            for (int y = 1; y <= Height; y++) {
                for (int x = 1; x <= Width; x++) {
                    GetRowAtY(y).GetTileAtX(x).Properties = TilePropertyPresets.GRASS;
                }
            }

            // Set the top row to be lava.
            for (int x = 1; x <= Width; x++) {
                GetRowAtY(1).GetTileAtX(x).Properties = TilePropertyPresets.LAVA;
            }

            // Set the bottom row to be lava.
            for (int x = 1; x <= Width; x++) {
                GetRowAtY(10).GetTileAtX(x).Properties = TilePropertyPresets.LAVA;
            }

            // Create a swerving path down the middle.
            for (int x = 1; x <= Width; x++) {
                if (x % 4 == 0) {
                    GetRowAtY(5).GetTileAtX(x).Properties = TilePropertyPresets.PATH;
                } else {
                    GetRowAtY(6).GetTileAtX(x).Properties = TilePropertyPresets.PATH;
                }
            
                if (x % 2 == 1 && x != 1) {
                    GetRowAtY(5).GetTileAtX(x).Properties = TilePropertyPresets.PATH;
                }
            }
        }
    }
}
