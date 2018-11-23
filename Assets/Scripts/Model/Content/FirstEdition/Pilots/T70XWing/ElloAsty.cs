using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class ElloAsty : T70XWing
        {
            public ElloAsty() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ello Asty",
                    7,
                    30,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ElloAstyAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class ElloAstyAbility : GenericAbility
    {
        private const string LEFT_TALON_ROLL = "3.L.E";
        private const string RIGHT_TALON_ROLL = "3.R.E";

        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckElloAstyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckElloAstyAbility;
        }

        private void CheckElloAstyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Bearing == ManeuverBearing.TallonRoll)
            {
                if (!HostShip.Tokens.HasToken(typeof(StressToken)))
                {
                    movement.ColorComplexity = MovementComplexity.Normal;
                }
            }
        }
    }
}