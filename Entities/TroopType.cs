using Microsoft.Xna.Framework;
using Siege.Core;
using Siege.Entities.Troops;
using Siege.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Entities {
    public enum TroopType {
        BASIC = 0,
        SPRINTER = 1,
        HEAVY = 2
    }

    public static class TroopTypeMethods {
        public static TroopBase CreateFakeTroop(this TroopType troopType) {
            return CreateTroopFromData(troopType, null, new Vector2(0, 0), true);
        }

        public static TroopBase CreateTroopFromData(TroopType? TType, MapBase TMap, Vector2 TPosition, bool Friendly, float? Health = null) {
            switch (TType.Value) {
                case TroopType.BASIC:
                    return new BasicTroop(TMap, TPosition, Friendly, Health);
                case TroopType.SPRINTER:
                    return new SprintTroop(TMap, TPosition, Friendly, Health);
                case TroopType.HEAVY:
                    return new HeavyTroop(TMap, TPosition, Friendly, Health);
                default:
                    return null;
            }
        }
    }
}
