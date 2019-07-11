﻿using System;
using System.Collections.Generic;
using Upgrade;
using static Ship.GenericShip;

namespace Ship
{
    public class PilotCardInfo
    {
        public string PilotName { get; private set; }
        public string PilotTitle { get; private set; }
        public int Initiative { get; set; }

        public int Limited { get; private set; }
        public bool IsLimited { get { return Limited != 0; } }

        public Type AbilityType { get; private set; }
        public string AbilityText { get; private set; }

        public int Force { get; set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }

        public int Cost { get; private set; }

        public List<UpgradeType> ExtraUpgrades { get; private set; }
        public Faction Faction { get; private set; }
        public int SEImageNumber { get; private set; }

        public PilotCardInfo(string pilotName, int initiative, int cost, bool isLimited = false, int limited = 0, Type abilityType = null, string pilotTitle = "", int force = 0, int charges = 0, bool regensCharges = false, UpgradeType extraUpgradeIcon = UpgradeType.None, List<UpgradeType> extraUpgradeIcons = null, Faction factionOverride = Faction.None, int seImageNumber = 0, string abilityText = "")
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
    }
}
