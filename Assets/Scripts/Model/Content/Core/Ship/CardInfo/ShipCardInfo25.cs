using Actions;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using Content;

namespace Ship
{
    public class ShipCardInfo25 : ShipCardInfo
    {
        private static Faction faction; // TODO: Remove
        private static Faction subFaction; // TODO: Remove
        private static List<Faction> factionsAll; // TODO: Remove
        private static string description; // TODO: Remove
        public List<Legality> LegalityInfo { get; }
        public FactionData FactionData { get; }
        public override Type IconicPilot(Faction faction) => FactionData.IconicPilot(faction);

        public ShipCardInfo25(
            string shipName,
            BaseSize baseSize,
            FactionData factionData,
            ShipArcsInfo arcInfo, int agility, int hull, int shields,
            ShipActionsInfo actionIcons,
            ShipUpgradesInfo upgradeIcons = null,
            List<LinkedActionInfo> linkedActions = null,
            string abilityText = "",
            List<Legality> legality = null) : base(shipName, baseSize, faction, arcInfo, agility, hull, shields, actionIcons, upgradeIcons, subFaction, factionsAll, description, abilityText)
        {
            ShipName = shipName;
            BaseSize = baseSize;

            FactionData = factionData;
            DefaultShipFaction = factionData.DefaultFaction;
            FactionsAll = factionData.AllFactions;

            ArcInfo = arcInfo;

            Agility = agility;
            Hull = hull;
            Shields = shields;

            ActionIcons = actionIcons;
            if (linkedActions != null) foreach (LinkedActionInfo linkedAction in linkedActions) { ActionIcons.AddLinkedAction(linkedAction); };

            UpgradeIcons = upgradeIcons ?? new ShipUpgradesInfo();

            AbilityText = abilityText;

            LegalityInfo = legality ?? new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal };
        }
    }
}
