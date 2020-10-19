using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class WilhuffTarkin : NimbusClassVWing
        {
            public WilhuffTarkin() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wilhuff Tarkin",
                    3,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WilhuffTarkinAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent } 
                );

                ImageUrl = "https://i.imgur.com/IMMW1ey.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WilhuffTarkinAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}