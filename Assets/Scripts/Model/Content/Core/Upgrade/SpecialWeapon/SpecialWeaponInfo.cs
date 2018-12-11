using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Upgrade
{
    public class SpecialWeaponInfo
    {
        public int AttackValue { get; private set; }
        public int MinRange { get; private set; }
        public int MaxRange { get; private set; }
        public Type RequiresToken { get; private set; }
        public Type SpendsToken { get; private set; }
        public int Charges { get; private set; }
        public bool Discard { get; private set; }
        public bool TwinAttack { get; private set; }
        public bool CanShootOutsideArc { get; private set; }
        public ArcType Arc { get; private set; }
        public bool UsesCharges { get { return Charges > 0; } }

        public SpecialWeaponInfo(int attackValue, int minRange, int maxRange, Type requiresToken = null, Type spendsToken = null, int charges = 0, bool discard = false, bool twinAttack = false, bool canShootOutsideArc = false, ArcType arc = ArcType.Front)
        {
            AttackValue = attackValue;
            MinRange = minRange;
            MaxRange = maxRange;
            RequiresToken = requiresToken;
            SpendsToken = spendsToken;
            Discard = discard;
            Charges = charges;
            TwinAttack = twinAttack;
            Arc = arc;
            CanShootOutsideArc = canShootOutsideArc;
        }
    }
}
