using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Map.Tiles;
using Siege.Tower.Towers;

namespace Siege.Tower {

    // An Enumeration of all possible TowerTypes which can be created.
    public enum TowerType {
        BASIC = 0,
        EXPLOSIVE = 1,
        ICE = 2,
        SPIKE = 3
    }

    /// <summary>
    /// A series of Methods which can be Invoked on a TowerType.
    /// </summary>
    public static class TowerTypeMethods {
        public static TowerBase CreateFakeTower(this TowerType towerType) {
            return CreateTowerFromData(towerType, null, true, null);
        }

        /// <summary>
        /// Creates a Tower of the provided Type.
        /// </summary>
        /// <param name="TType">The Type of tower to create.</param>
        /// <param name="TMap">The map the tower will be operational on</param>
        /// <param name="friendly">If the tower is friendly</param>
        /// <param name="TPosition">The Towers position on the map</param>
        /// <returns></returns>
        public static TowerBase CreateTowerFromData(TowerType? TType, MapBase TMap, bool friendly, Tile TPosition) {
            switch (TType.Value) {
                case TowerType.BASIC:
                    return new BasicTower(TMap, friendly, TPosition);
                case TowerType.EXPLOSIVE:
                    return new ExplosiveTower(TMap, friendly, TPosition);
                case TowerType.ICE:
                    return new IceTower(TMap, friendly, TPosition);
                case TowerType.SPIKE:
                    return new SpikeTower(TMap, friendly, TPosition);
                default:
                    return null;
            }
        }
    }
}
