using System;
using System.Collections.Generic;

namespace Ship
{
    public class ShipCardInfo
    {
        public string ShipName { get; set; }
        public BaseSize BaseSize { get; set; }
        public Faction Faction { get; set; }

        public ShipArcsInfo ArcInfo { get; private set; }
        public int Firepower { get {return ArcInfo.Firepower; } }
        public int Agility { get; private set; }
        public int Hull { get; set; }
        public int Shields { get; set; }

        public ShipActionsInfo ActionIcons { get; private set; }
        public ShipUpgradesInfo UpgradeIcons { get; private set; }

        public char Icon { get; private set; }
        public List<Faction> FactionsAll { get; set; }

        private SubFaction? subFaction { get; set; }
        public SubFaction SubFaction
        {
            get
            {
                if (subFaction != null)
                {
                    return subFaction.Value;
                }
                else
                {
                    switch (Faction)
                    {
                        case Faction.Imperial:
                            return SubFaction.GalacticEmpire;
                        case Faction.Rebel:
                            return SubFaction.RebelAlliance;
                        case Faction.Scum:
                            return SubFaction.ScumAndVillainy;
                        default:
                            throw new NotImplementedException("Invalid faction: " + Faction.ToString());
                    }
                }
            }
            set
            {
                subFaction = value;
            }
        }

        public ShipCardInfo(string shipName, BaseSize baseSize, Faction faction, ShipArcsInfo arcInfo, int agility, int hull, int shields, ShipActionsInfo actionIcons, ShipUpgradesInfo upgradeIcons, char icon = ' ', SubFaction subFaction = SubFaction.None, List<Faction> factionsAll = null)
        {
            ShipName = shipName;
            BaseSize = baseSize;

            Faction = faction;
            if (subFaction != SubFaction.None) SubFaction = subFaction;

            ArcInfo = arcInfo;

            Agility = agility;
            Hull = hull;
            Shields = shields;

            ActionIcons = actionIcons;
            UpgradeIcons = upgradeIcons;

            Icon = icon;

            FactionsAll = (factionsAll != null) ? factionsAll : new List<Faction>() { faction };
        }
    }
}
