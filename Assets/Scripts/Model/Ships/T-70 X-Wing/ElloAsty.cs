using System;

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
            Phases.OnActivationPhaseEnd += RegisterElloAstyAbility;
            Phases.OnBeforePlaceForces += RegisterElloAstyAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnActivationPhaseEnd -= RegisterElloAstyAbility;
            Phases.OnBeforePlaceForces -= RegisterElloAstyAbility;
        }

        private void RegisterElloAstyAbility()
        {
            if (!HostShip.HasToken(typeof(Tokens.StressToken)))
            {
                HostShip.Maneuvers[LEFT_TALON_ROLL] = Movement.ManeuverColor.White;
                HostShip.Maneuvers[RIGHT_TALON_ROLL] = Movement.ManeuverColor.White;
            }
            else
            {
                HostShip.Maneuvers[LEFT_TALON_ROLL] = Movement.ManeuverColor.Red;
                HostShip.Maneuvers[RIGHT_TALON_ROLL] = Movement.ManeuverColor.Red;
            }
        }
    }
}