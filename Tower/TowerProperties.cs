using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Tower {

    /// <summary>
    /// A collection of properties for Towers purely dependant on the TowerType.
    /// </summary>
    public class TowerProperties {
        public TowerType TowerType { get; private set; }
        public int? TextureID { get; private set; }
        public string Name { get; set; }

        public int Damage { get; set; }
        public int DamageRadius { get; set; }
        public int Speed { get; set; }
        public int Lifetime { get; set; }
        public TimeSpan RateOfFire { get; set; }

        public int Cost { get; set; }

        public TowerProperties(TowerType TowerType, int? TextureID, string Name, int Damage, int DamageRadius, int Speed, int Lifetime, int RateOfFire, int Cost) {
            this.TowerType = TowerType;
            this.TextureID = TextureID;
            this.Name = Name;
            this.Damage = Damage;
            this.DamageRadius = DamageRadius;
            this.Speed = Speed;
            this.Lifetime = Lifetime;
            this.RateOfFire = TimeSpan.FromMilliseconds(RateOfFire);
            this.Cost = Cost;
        }
    }
}
