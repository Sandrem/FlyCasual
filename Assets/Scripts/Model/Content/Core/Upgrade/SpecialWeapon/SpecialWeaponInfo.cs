using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Upgrade
{
    public class ArcsList
    {
        private List<ArcType> StoredArcsList { get; set; }

        public int Count { get { return StoredArcsList.Count; } }

        public ArcsList(ArcType arcType)
        {
            StoredArcsList = new List<ArcType>() { arcType };
        }

        public bool Contains(ArcType arcType)
        {
            bool result = StoredArcsList.Contains(arcType);

            // Double turret is two single turrets
            if (!result && arcType == ArcType.SingleTurret)
            {
                result = StoredArcsList.Contains(ArcType.DoubleTurret);
            }

            return result;
        }

        public void Add(ArcType arcType)
        {
            StoredArcsList.Add(arcType);
        }

        public void Remove(ArcType arcType)
        {
            StoredArcsList.Remove(arcType);
        }
    }

    public class SpecialWeaponInfo
    {
        public int AttackValue { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public List<Type> RequiresTokens { get; set; }
        public Type SpendsToken { get; private set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }
        public bool Discard { get; private set; }
        public bool TwinAttack { get; private set; }
        public bool CanShootOutsideArc { get; private set; }
        public ArcsList ArcRestrictions { get; private set; }
        public bool UsesCharges { get { return Charges > 0; } }
        public virtual bool NoRangeBonus { get; protected set; }

        public SpecialWeaponInfo(int attackValue, int minRange, int maxRange, List<Type> requiresTokens = null, Type requiresToken = null, Type spendsToken = null, int charges = 0, bool regensCharges = false, bool discard = false, bool twinAttack = false, bool canShootOutsideArc = false, ArcType arc = ArcType.Front, bool noRangeBonus = false)
        {
            AttackValue = attackValue;
            MinRange = minRange;
            MaxRange = maxRange;

            if (requiresToken != null) RequiresTokens = new List<Type>() { requiresToken };
            if (requiresTokens != null) RequiresTokens = new List<Type>(requiresTokens);
            if (RequiresTokens == null) RequiresTokens = new List<Type>();

            SpendsToken = spendsToken;
            Discard = discard;
            Charges = charges;
            RegensCharges = regensCharges;
            TwinAttack = twinAttack;
            ArcRestrictions = new ArcsList(arc);
            CanShootOutsideArc = canShootOutsideArc;
            NoRangeBonus = noRangeBonus;
        }
    }
}
