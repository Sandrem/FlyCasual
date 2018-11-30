using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Upgrade
{
    public class SpecialWeaponInfo
    {
        public int AttackValue { get; private set; }
        public int MinRange { get; private set; }
        public int MaxRange { get; private set; }
        public Type RequiresToken { get; private set; }
        public Type SpendsToken { get; private set; }
        public bool Discard { get; private set; }
        public int Charges { get; private set; }

        public SpecialWeaponInfo(int attackValue, int minRange, int maxRange, Type requiresToken = null, Type spendsToken = null, bool discard = false, int charges = 0)
        {
            AttackValue = attackValue;
            MinRange = minRange;
            MaxRange = maxRange;
            RequiresToken = requiresToken;
            SpendsToken = spendsToken;
            Discard = discard;
            Charges = charges;
        }
    }
}
