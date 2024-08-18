using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Entities;

namespace Siege.Events {

    public delegate void CastleDamageEventHandler(object sender, CastleDamageEventArgs args);

    public class CastleDamageEventArgs : EventArgs {
        public readonly TroopBase Cause;
        public readonly double DamageAmount;

        public CastleDamageEventArgs(double DamageAmount, TroopBase Cause) {
            this.DamageAmount = DamageAmount;
            this.Cause = Cause;
        }
    }
}
