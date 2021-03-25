using Movement;
using Ship;

namespace Abilities
{
    public class BeforeYouExecuteManeuver : TriggerForAbility
    {
        private TriggeredAbility Ability;
        private ConditionsBlock Conditions;

        public BeforeYouExecuteManeuver(ConditionsBlock conditions)
        {
            Conditions = conditions;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.BeforeMovementIsExecuted += RegisterTrigger;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.BeforeMovementIsExecuted -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            ConditionArgs args = new ConditionArgs()
            {
                ShipToCheck = Ability.HostShip
            };

            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.BeforeMovementIsExecuted,
                delegate
                {
                    if (Conditions.Passed(args))
                    {
                        Ability.Action.DoAction(Ability); 
                    }
                    else
                    {
                        Triggers.FinishTrigger();
                    }
                }
            );
        }
    }
}
