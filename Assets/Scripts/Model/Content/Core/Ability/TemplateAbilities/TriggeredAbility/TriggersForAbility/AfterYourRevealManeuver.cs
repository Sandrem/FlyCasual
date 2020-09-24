using Movement;
using Ship;
using UnityEngine;

namespace Abilities
{
    public class AfterYourRevealManeuver : TriggerForAbility
    {
        private TriggeredAbility Ability;
        public bool IfBank { get; }
        public bool IfTurn { get; }

        public AfterYourRevealManeuver
        (
            bool ifBank = false,
            bool ifTurn = false
        )
        {
            IfBank = ifBank;
            IfTurn = ifTurn;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnManeuverIsRevealed += CheckConditions;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnManeuverIsRevealed -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship)
        {
            bool conditionsAreMet = false;

            if (IfBank && ship.RevealedManeuver.Bearing == ManeuverBearing.Bank) conditionsAreMet = true;
            if (IfTurn && ship.RevealedManeuver.Bearing == ManeuverBearing.Turn) conditionsAreMet = true;

            if (conditionsAreMet)
            {
                Ability.RegisterAbilityTrigger
                (
                    TriggerTypes.OnManeuverIsRevealed,
                    delegate { Ability.Action.DoAction(Ability); }
                );
            }
        }
    }
}
