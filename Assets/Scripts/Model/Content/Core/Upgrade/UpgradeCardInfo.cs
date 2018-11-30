using Ship;
using System;
using System.Collections.Generic;

namespace Upgrade
{
    public class UpgradeCardInfo
    {
        public string Name { get; private set; }
        public List<UpgradeType> UpgradeTypes { get; private set; }
        public int Cost { get; set; }
        public int Limited { get; private set; }
        public bool IsLimited { get { return Limited != 0; } }
        public List<Type> AbilityTypes { get; private set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }
        public int SEImageNumber { get; private set; }
        public UpgradeCardRestrictions Restrictions { get; private set; }
        public SpecialWeaponInfo WeaponInfo { get; private set; }

        public UpgradeCardInfo(string name, UpgradeType upgradeType, int cost, bool isLimited = false, int limited = 0, Type abilityType = null, UpgradeCardRestriction restriction = null, UpgradeCardRestrictions restrictions = null, int charges = 0, bool regensCharges = false, int seImageNumber = 0, SpecialWeaponInfo weaponInfo = null)
        {
            Name = name;
            UpgradeTypes = new List<UpgradeType>() { upgradeType };
            Cost = cost;
            AbilityTypes = new List<Type>() { abilityType };
            Charges = charges;
            RegensCharges = regensCharges;
            SEImageNumber = seImageNumber;

            Limited = (isLimited) ? 1 : 0;
            if (limited != 0) Limited = limited;

            if (restrictions != null)
            {
                Restrictions = restrictions;
            }
            else
            {
                Restrictions = new UpgradeCardRestrictions(restriction);
            }

            WeaponInfo = weaponInfo;
        }

        public bool HasType(UpgradeType upgradeType)
        {
            return UpgradeTypes.Contains(upgradeType);
        }
    }
}
