using Ship;
using System;
using System.Collections.Generic;

namespace Upgrade
{
    public class UpgradeCardInfo
    {
        public string Name { get; private set; }
        public List<UpgradeType> UpgradeTypes { get; private set; }
        public int Cost { get; private set; }
        public int Limited { get; private set; }
        public bool IsLimited { get { return Limited != 0; } }
        public List<Type> AbilityTypes { get; private set; }
        public Faction RestrictionFaction { get; private set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }
        public int SEImageNumber { get; private set; }

        public UpgradeCardInfo(string name, UpgradeType upgradeType, int cost, bool isLimited = false, Type abilityType = null, Faction restrictionFaction = Faction.None, BaseSize restrictionSize = BaseSize.None, int charges = 0, bool regensCharges = false, int seImageNumber = 0)
        {
            Name = name;
            UpgradeTypes = new List<UpgradeType>() { upgradeType };
            Cost = cost;
            Limited = (isLimited) ? 1 : 0;
            AbilityTypes = new List<Type>() { abilityType };
            RestrictionFaction = restrictionFaction;
            Charges = charges;
            RegensCharges = regensCharges;
            SEImageNumber = seImageNumber;
        }

        public bool HasType(UpgradeType upgradeType)
        {
            return UpgradeTypes.Contains(upgradeType);
        }
    }
}
