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
        public class LandoCalrissian : ScavengedYT1300
        {
            public LandoCalrissian() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Lando Calrissian",
                    "Old General",
                    Faction.Resistance,
                    5,
                    8,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianResistanceAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Title,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );

                PilotNameCanonical = "landocalrissian-scavengedyt1300";

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/01ba9a50-1f9e-4ba8-be73-171a3ae59511/SWZ97_LandoCalrissianlegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandoCalrissianResistanceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}