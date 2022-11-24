using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class JulJerjerrod : TIESeBomber
        {
            public JulJerjerrod() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Jul Jerjerrod",
                    "Security Commander",
                    Faction.FirstOrder,
                    4,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JulJerjerrodAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/f646cd72-d2a9-446e-82b6-66028abfcea5/SWZ97_JulJerjerrodlegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JulJerjerrodAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
