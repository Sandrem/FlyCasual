using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    public class PilotCardInfo25 : PilotCardInfo
    {
        private static Faction factionOverride;

        public int LoadoutValue { get; }
        public string SkinName { get; set; }
        public List<Tags> Tags { get; }
        public List<Legality> LegalityInfo { get; }
        public bool IsStandardLayout { get; }
        public bool AffectedByStandardized { get; }

        public PilotCardInfo25(
            string pilotName,
            string pilotTitle,
            Faction faction,
            int initiative,
            int cost,
            int loadoutValue,
            bool isLimited = false,
            int limited = 0,
            Type abilityType = null,
            int force = 0,
            int regensForce = 1,
            int charges = 0,
            int regensCharges = 0,
            UpgradeType extraUpgradeIcon = UpgradeType.None,
            List<UpgradeType> extraUpgradeIcons = null,
            int seImageNumber = 0,
            string abilityText = "",
            string skinName = null,
            List<Tags> tags = null,
            bool affectedByStandardized = true,
            List<Legality> legality = null,
            bool isStandardLayout = false) : base(pilotName, initiative, cost, isLimited, limited, abilityType, pilotTitle, force, charges, regensCharges,
            extraUpgradeIcon, extraUpgradeIcons, factionOverride, seImageNumber, abilityText)
        {
            PilotName = pilotName;
            PilotTitle = pilotTitle;
            Faction = faction;

            Initiative = initiative;
            Cost = cost;
            LoadoutValue = (isStandardLayout) ? int.MaxValue : loadoutValue;

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
            RegensForce = regensForce;
            Charges = charges;
            RegensCharges = regensCharges;

            SEImageNumber = seImageNumber;

            ExtraUpgrades = new List<UpgradeType>();
            if (extraUpgradeIcon != UpgradeType.None) ExtraUpgrades.Add(extraUpgradeIcon);
            if (extraUpgradeIcons != null) ExtraUpgrades.AddRange(extraUpgradeIcons);

            AffectedByStandardized = affectedByStandardized;

            SkinName = skinName;
            Tags = tags ?? new List<Tags>();
            LegalityInfo = legality ?? new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal };

            IsStandardLayout = isStandardLayout;
        }
    }
}
