using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    public class PilotCardInfo
    {
        public string PilotName { get; private set; }
        public string PilotTitle { get; private set; }
        public int Initiative { get; private set; }
        public int Limited { get; private set; }

        public Type AbilityType { get; private set; }

        public int Force { get; private set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }

        public int Cost { get; private set; }

        public List<UpgradeType> ExtraUpgrades { get; private set; }
        public Faction Faction { get; private set; }
        public int SEImageNumber { get; private set; }

        public PilotCardInfo(string pilotName, int initiative, int cost, int limited = 0, Type abilityType = null, string pilotTitle = "", int force = 0, int charges = 0, bool regensCharges = false, UpgradeType extraUpgradeIcon = UpgradeType.None, List<UpgradeType> extraUpgradeIcons = null, Faction factionOverride = Faction.None, int seImageNumber = 0)
        {
            PilotName = pilotName;
            PilotTitle = pilotTitle;
            Initiative = initiative;
            Limited = limited;

            AbilityType = abilityType;

            Force = force;
            Charges = charges;
            RegensCharges = regensCharges;

            Cost = cost;

            SEImageNumber = seImageNumber;

            ExtraUpgrades = new List<UpgradeType>();
            if (extraUpgradeIcon != UpgradeType.None) ExtraUpgrades.Add(extraUpgradeIcon);
            if (extraUpgradeIcons != null) ExtraUpgrades.AddRange(extraUpgradeIcons);
            if (factionOverride != Faction.None) Faction = factionOverride;
        }
    }
}
