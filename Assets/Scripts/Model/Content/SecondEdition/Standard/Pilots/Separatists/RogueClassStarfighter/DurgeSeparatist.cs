using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class DurgeSeparatist : RogueClassStarfighter
        {
            public DurgeSeparatist() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Durge",
                    "On His Own Time",
                    Faction.Separatists,
                    5,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DurgeSeparatistAbility),
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

                PilotNameCanonical = "durge-separatistalliance";

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/b3ba7d17-a6f3-4b6d-9740-f85cef641388/SWZ97_Durgelegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DurgeSeparatistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}