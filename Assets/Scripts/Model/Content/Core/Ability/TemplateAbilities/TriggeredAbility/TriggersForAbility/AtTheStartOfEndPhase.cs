using Movement;
using Ship;
using System.Linq;

namespace Abilities
{
    public class AtTheStartOfEndPhase : TriggerForAbility
    {
        private TriggeredAbility Ability;


        public AtTheStartOfEndPhase()
        {

        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            Phases.Events.OnEndPhaseStart_NoTriggers += RegisterAbility;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.OnEndPhaseStart,
                delegate { Ability.Action.DoAction(Ability); }
            );
        }
    }
}
