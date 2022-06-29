using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class BodicaVenj : FangFighter
        {
            public BodicaVenj() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Bodica Venj",
                    "Wrathful Warrior",
                    Faction.Rebel,
                    4,
                    5,
                    6,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BodicaVenjAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/bodicavenj.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BodicaVenjAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}