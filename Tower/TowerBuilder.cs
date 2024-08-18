using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Map.Tiles;
using Siege.Tower.Towers;

namespace Siege.Tower {

    /// <summary>
    /// A Builder for Towers allowing them to be easily created in batches if they
    /// possess similar properties.
    /// </summary>
    public class TowerBuilder : IBuilder<TowerBase> {
        // The TowerType of the Tower.
        private TowerType? TType;
        
        // The Map of the Tower.
        private MapBase TMap;
        private bool friendly;
        // The Tile position of the Tower on the map.
        private Tile TPosition;

        /// <summary>
        /// Creates an instance of this Builder.
        /// </summary>
        /// <returns>A new instance of this Builder.</returns>
        public static TowerBuilder Builder() {
            return new TowerBuilder();
        }

        /// <summary>
        /// Sets the Map of the Tower to build.
        /// </summary>
        /// <param name="TMap">The Tower's map.</param>
        /// <returns>This Builder.</returns>
        public TowerBuilder Map(MapBase TMap) {
            this.TMap = TMap;

            return this;
        }

        public TowerBuilder Friendly(bool friendly) {
            this.friendly = friendly;

            return this;
        }

        /// <summary>
        /// Sets the Position of the Tower to build.
        /// </summary>
        /// <param name="TPosition">The Tower's position.</param>
        /// <returns>This Builder.</returns>
        public TowerBuilder Position(Tile TPosition) {
            this.TPosition = TPosition;

            return this;
        }

        /// <summary>
        /// Sets the Type of the Tower to build.
        /// </summary>
        /// <param name="TType">The Tower's type.</param>
        /// <returns>This Builder.</returns>
        public TowerBuilder Type(TowerType TType) {
            this.TType = TType;

            return this;
        }

        /// <summary>
        /// Validates the data provided to the Builder.
        /// </summary>
        /// <returns>The Builder but if data is missing a BuilderExceptions.MissingData exception will be thrown.</returns>
        public IBuilder<TowerBase> Validate() {
            if (!TType.HasValue || TMap == null || TPosition == null) {
                throw new BuilderExceptions.MissingData();
            }

            return this;
        }

        /// <summary>
        /// Constructs the Tower based on the data provided to the Builder.
        /// </summary>
        /// <returns>The newly constructed Tower.</returns>
        public TowerBase Build() {
            Validate();

            // Create a different Object based on the TowerType provided.
            return TowerTypeMethods.CreateTowerFromData(TType, TMap, friendly, TPosition);
        }
    }
}
