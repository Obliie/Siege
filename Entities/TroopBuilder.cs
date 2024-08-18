using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Siege.Core;
using Siege.Entities.Troops;
using Siege.Map.Tiles;

namespace Siege.Entities {

    /// <summary>
    /// A Builder for Troops allowing them to be easily created in batches if they
    /// possess similar properties.
    /// </summary>
    public class TroopBuilder : IBuilder<TroopBase> {
        // The TroopType of the Troop.
        private TroopType? TType;

        // The Map of the Troop.
        private MapBase TMap;
        // The Vector2 position of the Troop on the map.
        private Vector2? TPosition;
        // If the Troop will be Friendly.
        private bool? TFriendly;
        // The health of the Troop, if it is not to be created with max health.
        private float? THealth;

        /// <summary>
        /// Creates an instance of this Builder.
        /// </summary>
        /// <returns>A new instance of this Builder.</returns>
        public static TroopBuilder Builder() {
            return new TroopBuilder();
        }

        /// <summary>
        /// Sets the Map of the Troop to build.
        /// </summary>
        /// <param name="TMap">The Troop's map.</param>
        /// <returns>This Builder.</returns>
        public TroopBuilder Map(MapBase TMap) {
            this.TMap = TMap;

            return this;
        }

        /// <summary>
        /// Sets the Position of the Troop to build.
        /// </summary>
        /// <param name="TPosition">The Troop's position.</param>
        /// <returns>This Builder.</returns>
        public TroopBuilder Position(Vector2 TPosition) {
            this.TPosition = TPosition;

            return this;
        }

        /// <summary>
        /// Sets the Type of the Troop to build.
        /// </summary>
        /// <param name="TType">The Troop's type.</param>
        /// <returns>This Builder.</returns>
        public TroopBuilder Type(TroopType TType) {
            this.TType = TType;

            return this;
        }

        /// <summary>
        /// Sets if the Troop to build is Friendly.
        /// </summary>
        /// <param name="TFriendly">If the Troop is friendly.</param>
        /// <returns>This Builder.</returns>
        public TroopBuilder Friendly(bool TFriendly) {
            this.TFriendly = TFriendly;

            return this;
        }

        public TroopBuilder Health(float THealth) {
            this.THealth = THealth;

            return this;
        }

        /// <summary>
        /// Validates the data provided to the Builder.
        /// </summary>
        /// <returns>The Builder but if data is missing a BuilderExceptions.MissingData exception will be thrown.</returns>
        public IBuilder<TroopBase> Validate() {
            if (!TType.HasValue || TMap == null || !TFriendly.HasValue) {
                throw new BuilderExceptions.MissingData();
            }

            return this;
        }

        /// <summary>
        /// Constructs the Troop based on the data provided to the Builder.
        /// </summary>
        /// <returns>The newly constructed Troop.</returns>
        public TroopBase Build() {
            Validate();

            if (TPosition == null) {
                if (TFriendly.Value) {
                    TPosition = new Vector2((TMap.PlayerTroopSpawnPosition.X - 1) * TMap.TileWidth, (TMap.PlayerTroopSpawnPosition.Y - 1) * TMap.TileHeight);
                } else {
                    TPosition = new Vector2((TMap.EnemyTroopSpawnPosition.X - 1) * TMap.TileWidth, (TMap.EnemyTroopSpawnPosition.Y - 1) * TMap.TileHeight);
                }
            }

            // Create a different Object based on the TowerType provided.
            return TroopTypeMethods.CreateTroopFromData(TType, TMap, TPosition.Value, TFriendly.Value, THealth);
        }
    }
}
