using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    public class ShipCardInfo
    {
        public string ShipName { get; set; }
        public BaseSize BaseSize { get; set; }
        public Faction DefaultShipFaction { get; set; }

        public ShipArcsInfo ArcInfo { get; set; }
        public int Firepower { get {return ArcInfo.Arcs.First().Firepower; } }
        public int Agility { get; set; }
        public int Hull { get; set; }
        public int Shields { get; set; }
        public int Charges { get; set; }
        public int RegensCharges { get; set; }

        public ShipActionsInfo ActionIcons { get; set; }
        public ShipActionsInfo PotentialActionIcons { get; set; }
        public ShipUpgradesInfo UpgradeIcons { get; set; }

        public List<Faction> FactionsAll { get; set; }

        public Faction SubFaction { get; set; }

        public string Description { get; set; }
        public string AbilityText { get; set; }

        public ShipCardInfo(string shipName, BaseSize baseSize, Faction faction, ShipArcsInfo arcInfo, int agility, int hull, int shields, ShipActionsInfo actionIcons, ShipUpgradesInfo upgradeIcons, Faction subFaction = Faction.None, List<Faction> factionsAll = null, string description = null, string abilityText = "")
        {
            ShipName = shipName;
            BaseSize = baseSize;

            DefaultShipFaction = faction;
            if (subFaction != Faction.None) SubFaction = subFaction;

            ArcInfo = arcInfo;

            Agility = agility;
            Hull = hull;
            Shields = shields;

            ActionIcons = actionIcons;
            PotentialActionIcons = new ShipActionsInfo();
            UpgradeIcons = upgradeIcons;

            FactionsAll = (factionsAll != null) ? factionsAll : new List<Faction>() { faction };

            Description = description;

            AbilityText = abilityText;
        }

        public virtual Type IconicPilot(Faction squadFaction) => null;
    }
}
