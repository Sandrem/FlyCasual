using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class Durge : RogueClassStarfighter
        {
            public Durge() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Durge",
                    "Hard to Kill",
                    Faction.Scum,
                    5,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DurgeAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.BountyHunter
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/9e694fa9-2526-4309-9c51-ad78603548bb/SWZ97_Durge2legal.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DurgeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}