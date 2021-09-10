using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class FennRauRebel : FangFighter
        {
            public FennRauRebel() : base()
            {
                IsWIP = true;

                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "Fenn Rau",
                    6,
                    60,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.FennRauRebelFangAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    factionOverride: Faction.Rebel
                );

                PilotNameCanonical = "fennrau-rebelalliance";

                ImageUrl = "https://i.imgur.com/czHjZ4D.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FennRauRebelFangAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}