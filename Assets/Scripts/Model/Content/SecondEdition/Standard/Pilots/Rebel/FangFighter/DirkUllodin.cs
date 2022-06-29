using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class DirkUllodin : FangFighter
        {
            public DirkUllodin() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Dirk Ullodin",
                    "Aspiring Commando",
                    Faction.Rebel,
                    3,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DirkUllodinAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/dirkullodin.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DirkUllodinAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}