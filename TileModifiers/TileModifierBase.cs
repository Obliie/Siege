using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Siege.Core;
using Siege.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.TileModifiers {

    /// <summary>
    /// A temporary modification to a Tile on a map.
    /// </summary>
    public abstract class TileModifierBase : ITileEntity {
        // The map on which this modifier is active.
        private MapBase map;

        // The Tile to which this modification should be applied.
        private Tile ApplicationLocation;
        // The Tiles old properties.
        private TileProperties OldProperties;
        // The Tiles new (modified) properties.
        private TileProperties NewProperties;

        // The time at which this TileModifier was activated.
        private TimeSpan CreationTime = TimeSpan.FromSeconds(0);
        // The length of time this TileModifier should be active.
        private TimeSpan Lifetime;

        // The cost of placing this TileModifier.
        public int Cost { get; set; }
        // The human readable name of this TileModifier
        public string Name { get; set; }
        // If this TileModifier has despawned yet.
        public bool Despawned { get; set; }

        public TileModifierBase(MapBase map, TileProperties NewProperties, Tile ApplicationLocation, TimeSpan Lifetime, int Cost) {
            this.map = map;
            this.ApplicationLocation = ApplicationLocation;
            this.OldProperties = ApplicationLocation.Properties.Clone();
            this.NewProperties = NewProperties;
            this.Lifetime = Lifetime;
            this.Cost = Cost;
            this.Despawned = false;
        }

        /// <summary>
        /// Gets the Position at which this TileModifier is applied.
        /// </summary>
        /// <returns></returns>
        public Tile GetTilePosition() {
            return ApplicationLocation;
        }

        /// <summary>
        /// Updates the Tiles properties with the new (modified) properties.
        /// </summary>
        public void OnAcceptPlacement() {
            ApplicationLocation.Properties = NewProperties;
        }

        /// <summary>
        /// Sets the Tiles properties back to what they were.
        /// </summary>
        public void Remove() {
            ApplicationLocation.Properties = OldProperties;
        }

        /// <summary>
        /// Allows the TileModifier to check for any logic. If it should despawn, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            // If the CreationTime hasn't been updated, set it to this instant.
            if (CreationTime.TotalMilliseconds == 0) {
                CreationTime = TimeSpan.FromMilliseconds(gameTime.TotalGameTime.TotalMilliseconds);
            }

            // If the TileModifier has exceeded its Lifetime, set it to be despawned.
            if (gameTime.TotalGameTime.TotalMilliseconds > CreationTime.TotalMilliseconds + Lifetime.TotalMilliseconds) {
                Despawned = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            // Nothing is required here as the Tile properties are updated directly on the Map.
            // The Maps Draw method will update the visual for the user.
            return;
        }
    }
}
