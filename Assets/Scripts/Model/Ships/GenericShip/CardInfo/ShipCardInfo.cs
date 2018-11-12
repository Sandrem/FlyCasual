using System;
using System.Collections.Generic;

namespace Ship
{
    public class ShipCardInfo
    {
        public string ShipName { get; set; }
        public Faction Faction { get; private set; }

        public ShipArcsInfo ArcInfo { get; private set; }
        public int Firepower { get; private set; }
        public int Agility { get; private set; }
        public int Hull { get; set; }
        public int Shields { get; private set; }

        public ShipActionsInfo ActionIcons { get; private set; }
        public ShipUpgradesInfo UpgradeIcons { get; private set; }

        public SubFaction ShipSubFaction { get; private set; }
        public char Icon { get; private set; }
        public List<Faction> FactionsAll { get; private set; }

        public ShipCardInfo(string shipName, Faction faction, ShipArcsInfo arcInfo, int agility, int hull, int shields, ShipActionsInfo actionIcons, ShipUpgradesInfo upgradeIcons, char icon = ' ', SubFaction shipSubFaction = SubFaction.None, List<Faction> factionsAll = null)
        {
            ShipName = shipName;
            Faction = faction;

            ArcInfo = arcInfo;
            Agility = agility;
            Hull = hull;
            Shields = shields;

            ActionIcons = actionIcons;
            UpgradeIcons = upgradeIcons;

            ShipSubFaction = shipSubFaction;
            Icon = icon;

            FactionsAll = (factionsAll != null) ? factionsAll : new List<Faction>() { faction };
        }
    }
}
