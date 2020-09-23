using ActionsList;
using Movement;
using Ship;
using System;

namespace Abilities
{
    public class AfterYouPerformAction : TriggerForAbility
    {
        private TriggeredAbility Ability;
        private Type HasToken;

        public Type ActionType { get; }

        public AfterYouPerformAction(Type actionType, Type hasToken = null)
        {
            ActionType = actionType;
            HasToken = hasToken;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnActionIsPerformed += RegisterTrigger;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnActionIsPerformed -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericAction action)
        {
            if (action.GetType() == ActionType
                && ((HasToken == null) || (HasToken != null && Ability.HostShip.Tokens.HasToken((HasToken), '*')))
            )
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
