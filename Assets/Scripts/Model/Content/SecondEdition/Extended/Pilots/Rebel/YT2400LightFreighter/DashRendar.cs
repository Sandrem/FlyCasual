using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YT2400LightFreighter
    {
        public class DashRendar : YT2400LightFreighter
        {
            public DashRendar() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dash Rendar",
                    "Hotshot Mercenary",
                    Faction.Rebel,
                    5,
                    10,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DashRendarAbility),
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 77,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DashRendarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.IsIgnoreObstacles = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.IsIgnoreObstacles = false;
        }
    }
}
