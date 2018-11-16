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
        public int Firepower { get; private set; }
        public int Agility { get; private set; }
        public int Hull { get; set; }
        public int Shields { get; set; }

        public ShipActionsInfo ActionIcons { get; set; }
        public ShipUpgradesInfo UpgradeIcons { get; private set; }

        public SubFaction ShipSubFaction { get; private set; }
        public char Icon { get; private set; }
        public List<Faction> FactionsAll { get; private set; }

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

        public ShipCardInfo(string shipName, BaseSize baseSize, Faction faction, ShipArcsInfo arcInfo, int agility, int hull, int shields, ShipActionsInfo actionIcons, ShipUpgradesInfo upgradeIcons, char icon = ' ', SubFaction shipSubFaction = SubFaction.None, List<Faction> factionsAll = null)
        {
            ShipName = shipName;
            BaseSize = baseSize;

            Faction = faction;

            ArcInfo = arcInfo;
            Firepower = arcInfo.Firepower;

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
