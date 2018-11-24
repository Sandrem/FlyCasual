using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class KyleKatarn : Hwk290LightFreighter
        {
            public KyleKatarn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kyle Katarn",
                    3,
                    38,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.KyleKatarnAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 43
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KyleKatarnAbility : Abilities.FirstEdition.KyleKatarnAbility
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                Board.IsShipInArc(HostShip, ship);
        }
    }
}
