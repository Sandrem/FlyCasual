using System;
using Ship;
using Movement;

namespace Ship
{
    namespace T70XWing
    {
        public class ElloAsty : T70XWing
        {
            public ElloAsty() : base()
            {
                PilotName = "Ello Asty";
                PilotSkill = 7;
                Cost = 30;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ElloAstyAbility());
            }
        }
    }
}

namespace Abilities
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

        private void CheckElloAstyAbility(GenericShip ship, ref MovementStruct movement)
        {
            if (movement.Bearing == ManeuverBearing.TallonRoll)
            {
                if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    movement.ColorComplexity = ManeuverColor.White;
                }
            }
        }
    }
}