using Movement;
using Ship;

namespace Abilities
{
    public class AfterYouPerfromAttack : TriggerForAbility
    {
        private TriggeredAbility Ability;

        public AfterYouPerfromAttack() {}

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnAttackFinishAsAttacker += RegisterTrigger;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnAttackFinishAsAttacker -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.OnAttackFinish,
                delegate { Ability.Action.DoAction(Ability); }
            );
        }
    }
}
