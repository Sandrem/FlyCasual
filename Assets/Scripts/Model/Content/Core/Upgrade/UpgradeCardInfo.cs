using Actions;
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
        public bool FeIsLimitedPerShip { get; private set; }
        public List<Type> AbilityTypes { get; private set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }
        public int SEImageNumber { get; private set; }
        public UpgradeCardRestrictions Restrictions { get; private set; }
        public SpecialWeaponInfo WeaponInfo { get; private set; }
        public ActionInfo AddAction { get; private set; }
        public bool FEIsLimited { get; private set; }
        public List<UpgradeSlot> AddedSlots { get; private set; }
        public List<UpgradeType> ForbiddenSlots { get; private set; }

        public UpgradeCardInfo(string name, UpgradeType type = UpgradeType.None, List<UpgradeType> types = null, int cost = 0, bool isLimited = false, int limited = 0, Type abilityType = null, UpgradeCardRestriction restriction = null, UpgradeCardRestrictions restrictions = null, int charges = 0, bool regensCharges = false, int seImageNumber = 0, SpecialWeaponInfo weaponInfo = null, ActionInfo addAction = null, UpgradeSlot addSlot = null, List<UpgradeSlot> addSlots = null, bool feIsLimitedPerShip = false, UpgradeType forbidSlot = UpgradeType.None, List<UpgradeType> forbidSlots = null)
        {
            Name = name;
            Cost = cost;
            AbilityTypes = new List<Type>() { abilityType };
            Charges = charges;
            RegensCharges = regensCharges;
            SEImageNumber = seImageNumber;
            FEIsLimited = feIsLimitedPerShip;
            WeaponInfo = weaponInfo;
            AddAction = addAction;

            Limited = (isLimited) ? 1 : 0;
            if (limited != 0) Limited = limited;

            FeIsLimitedPerShip = feIsLimitedPerShip;

            UpgradeTypes = new List<UpgradeType>();
            if (type != UpgradeType.None) UpgradeTypes.Add(type);
            if (types != null) UpgradeTypes.AddRange(types);

            Restrictions = new UpgradeCardRestrictions();
            if (restriction != null) Restrictions = new UpgradeCardRestrictions(restriction);
            if (restrictions != null) Restrictions = restrictions;

            AddedSlots = new List<UpgradeSlot>();
            if (addSlot != null) AddedSlots.Add(addSlot);
            if (addSlots != null) AddedSlots.AddRange(addSlots);

            ForbiddenSlots = new List<UpgradeType>();
            if (forbidSlot != UpgradeType.None) ForbiddenSlots.Add(forbidSlot);
            if (forbidSlots != null) ForbiddenSlots.AddRange(forbidSlots);
        }

        public bool HasType(UpgradeType upgradeType)
        {
            return UpgradeTypes.Contains(upgradeType);
        }
    }
}
