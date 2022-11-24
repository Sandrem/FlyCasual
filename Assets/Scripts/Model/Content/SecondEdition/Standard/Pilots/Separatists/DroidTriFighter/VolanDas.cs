using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class VolanDas : DroidTriFighter
    {
        public VolanDas()
        {
            //TODO: Bio-pilot

            IsWIP = true;

            PilotInfo = new PilotCardInfo25
            (
                "Volan Das",
                "Impatient Invader",
                Faction.Separatists,
                5,
                4,
                12,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.VolanDasAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Missile,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.BountyHunter
                }
            );

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/8971dc41-0a34-4f6f-be9e-56e0e94fda79/SWZ97_VolanDaslegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VolanDasAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
