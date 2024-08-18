using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Map.Tiles;
using Siege.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Siege.Core;

namespace Siege.Threding.Tasks {

    /// <summary>
    /// Keeps a Troops velocity updated based on a provided Path and the Troops current position.
    /// </summary>
    public class PathfindingTask {
        // The distance down the Path the Troop has travelled.
        private int CurrentTile = 0;

        // The Troop whose velocity is being updated.
        private TroopBase Troop;
        // The Path the Troop should travel down.
        private List<Tile> Path;

        // The X and Y coordinate of the previous Tile the Troop was on.
        private int PreviousTileX;
        private int PreviousTileY;

        public PathfindingTask(TroopBase Troop, List<Tile> Path) {
            this.Troop = Troop;
            this.Path = Path;
        }

        public void Run() {
            // If the Troop has not reached the end of the Path set its velocity to be in the direction which takes it from
            // its current position to the position of the next Tile in the Path, otherwise set it to have no direction.
            if (Path.Count > CurrentTile) {
                Troop.TDirection = Vector2.Normalize(new Vector2((Path.ElementAt(CurrentTile).X * Troop.map.TileWidth) - 24, (Path.ElementAt(CurrentTile).Y * Troop.map.TileHeight) - 24) - Troop.Position);
            } else {
                Troop.TDirection = Direction.NONE;
            }

            // If the Troop has moved (Its X or Y Tile coordinates have changed, increment its Tile distance travelled.
            if (Troop.TileX != PreviousTileX || Troop.TileY != PreviousTileY) {
                CurrentTile++;
            }

            // Update the Troops previous Tile locations.
            this.PreviousTileX = Troop.TileX;
            this.PreviousTileY = Troop.TileY;
        }

        /// <summary>
        /// Updates the PathfindingTasks Path with a new Path to follow.
        /// </summary>
        /// <param name="NewPath">The new Path to follow</param>
        public void UpdatePath(List<Tile> NewPath) {
            this.Path = NewPath;
            this.CurrentTile = 1;
        }
    }
}
