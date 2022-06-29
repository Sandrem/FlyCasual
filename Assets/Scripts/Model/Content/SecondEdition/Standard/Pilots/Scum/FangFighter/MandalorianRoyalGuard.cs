using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class MandalorianRoyalGuard : FangFighter
        {
            public MandalorianRoyalGuard() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Mandalorian Royal Guard",
                    "Unlikely Ally",
                    Faction.Scum,
                    4,
                    5,
                    10,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.MandalorianRoyalGuardAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/mandalorianroyalguard.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MandalorianRoyalGuardAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}