using Movement;
using Ship;
using System.Linq;

namespace Abilities
{
    public class AfterPlacingForces : TriggerForAbility
    {
        private TriggeredAbility Ability;


        public AfterPlacingForces()
        {

        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            Phases.Events.OnSetupEnd += RegisterAbility;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            Phases.Events.OnSetupEnd -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.OnSetupEnd,
                delegate { Ability.Action.DoAction(Ability); }
            );
        }
    }
}
