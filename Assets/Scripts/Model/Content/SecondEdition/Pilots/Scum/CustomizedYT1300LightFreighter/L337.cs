﻿using Content;
using Movement;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CustomizedYT1300LightFreighter
    {
        public class L337 : CustomizedYT1300LightFreighter
        {
            public L337() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "L3-37",
                    "Droid Revolutionary",
                    Faction.Scum,
                    2,
                    5,
                    9,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.L337Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid,
                        Tags.Freighter,
                        Tags.YT1300
                    },
                    seImageNumber: 224
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class L337Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (HostShip.State.ShieldsCurrent == 0 && movement.Bearing == ManeuverBearing.Bank)
            {
                movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
            }
        }
    }
}
