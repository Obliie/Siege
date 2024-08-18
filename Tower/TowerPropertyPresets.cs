using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Tower {
    public class TowerPropertyPresets {
        public static TowerProperties BASIC_T1 = new TowerProperties(TowerType.BASIC, 0, "Basic Tower", 10, 0, 2, 1000, 200, 100);

        public static TowerProperties EXPLOSIVE_T1 = new TowerProperties(TowerType.EXPLOSIVE, 1, "Explosive Tower", 20, 2, 1, 2000, 1000, 500);

        public static TowerProperties ICE_T1 = new TowerProperties(TowerType.ICE, 2, "Ice Tower", 1, 1, 1, 2000, 750, 200);

        public static TowerProperties SPIKE_T1 = new TowerProperties(TowerType.SPIKE, 3, "Spike Tower", 5, 0, 1, 500, 250, 150);

        public static TowerProperties GetPropertiesForTower(TowerType towerType) {
            switch (towerType) {
                case TowerType.BASIC:
                    return BASIC_T1;
                case TowerType.EXPLOSIVE:
                    return EXPLOSIVE_T1;
                case TowerType.ICE:
                    return ICE_T1;
                case TowerType.SPIKE:
                    return SPIKE_T1;
                default:
                    return null;
            }
        }
    }
}
