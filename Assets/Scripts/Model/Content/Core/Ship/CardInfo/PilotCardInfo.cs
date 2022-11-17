using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    public class PilotCardInfo
    {
        public string PilotName { get; set; }
        public string PilotTitle { get; set; }
        public int Initiative { get; set; }

        public int Limited { get; set; }
        public bool IsLimited { get { return Limited != 0; } }

        public Type AbilityType { get; set; }
        public string AbilityText { get; set; }

        public int Force { get; set; }
        public int RegensForce { get; set; }
        public int Charges { get; set; }
        public int RegensCharges { get; set; }

        public int Cost { get; set; }

        public List<UpgradeType> ExtraUpgrades { get; set; }
        public Faction Faction { get; set; }
        public int SEImageNumber { get; set; }

        public PilotCardInfo(string pilotName,
            int initiative,
            int cost,
            bool isLimited = false,
            int limited = 0,
            Type abilityType = null,
            string pilotTitle = "",
            int force = 0,
            int charges = 0,
            int regensCharges = 0,
            UpgradeType extraUpgradeIcon = UpgradeType.None,
            List<UpgradeType> extraUpgradeIcons = null,
            Faction factionOverride = Faction.None,
            int seImageNumber = 0,
            string abilityText = ""
        )
        {
            PilotName = pilotName;
            PilotTitle = pilotTitle;
            Initiative = initiative;

            if (isLimited)
            {
                Limited = 1;
            }
            else
            {
                Limited = limited;
            }

            AbilityType = abilityType;
            AbilityText = abilityText;

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


        public string GetCleanName()
        {
            string cleanName = PilotName;
            if (PilotName.Contains("(")) cleanName = PilotName.Substring(0, PilotName.LastIndexOf("(") - 1);
            return cleanName;
        }
    }
}
