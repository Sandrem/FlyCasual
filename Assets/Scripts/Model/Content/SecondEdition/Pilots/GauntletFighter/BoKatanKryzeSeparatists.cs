using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class BoKatanKryzeSeparatists : GauntletFighter
        {
            public BoKatanKryzeSeparatists() : base()
            {
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "Bo-Katan Kryze",
                    4,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BoKatanKryzeSeparatistsAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    factionOverride: Faction.Separatists
                );

                PilotNameCanonical = "bokatankryze-separatistalliance";

                ImageUrl = "https://i.imgur.com/QCUreef.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoKatanKryzeSeparatistsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}