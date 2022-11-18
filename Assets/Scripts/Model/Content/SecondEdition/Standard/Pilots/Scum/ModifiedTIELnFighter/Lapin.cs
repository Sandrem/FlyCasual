using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class Lapin : ModifiedTIELnFighter
        {
            public Lapin() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lapin",
                    "Stickler for Details",
                    Faction.Scum,
                    2,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LapinAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );
                
                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/1437f98b-cff9-42d8-bca6-2f466f9b9d89/SWZ97_Lapinlegal.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LapinAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}