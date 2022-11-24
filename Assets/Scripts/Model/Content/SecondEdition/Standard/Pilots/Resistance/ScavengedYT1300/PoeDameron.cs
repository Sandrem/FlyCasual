using ActionsList;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class PoeDameron : ScavengedYT1300
        {
            public PoeDameron() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Poe Dameron",
                    "A Difficult Man",
                    Faction.Resistance,
                    6,
                    7,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PoeDameronResistanceAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Title,
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );

                PilotNameCanonical = "poedameron-scavengedyt1300";

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/6d63e9f9-05c3-48c3-bcc1-768e378a5560/SWZ97_PoeDameronlegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PoeDameronResistanceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}