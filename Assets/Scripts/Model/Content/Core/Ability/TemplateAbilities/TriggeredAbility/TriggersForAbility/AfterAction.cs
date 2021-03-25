using ActionsList;
using System;

namespace Abilities
{
    public class AfterAction : TriggerForAbility
    {
        private TriggeredAbility Ability;
        private Type ActionType;

        public AfterAction(Type actionType)
        {
            ActionType = actionType;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action.GetType() == ActionType)
            {
                Ability.RegisterAbilityTrigger
                (
                    TriggerTypes.OnActionIsPerformed,
                    delegate { Ability.Action.DoAction(Ability); }
                );
            }
        }
    }
}
