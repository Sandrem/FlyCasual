using Movement;
using Ship;

namespace Abilities
{
    public class BeforeYouEngage : TriggerForAbility
    {
        private TriggeredAbility Ability;

        public BeforeYouEngage() {}

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnCombatActivation += RegisterTrigger;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnCombatActivation -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.OnCombatActivation,
                delegate { Ability.Action.DoAction(Ability); }
            );
        }
    }
}
