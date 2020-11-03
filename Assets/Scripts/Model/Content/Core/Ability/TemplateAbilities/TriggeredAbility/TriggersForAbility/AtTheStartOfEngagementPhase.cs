using Movement;
using Ship;
using System.Linq;

namespace Abilities
{
    public class AtTheStartOfEngagementPhase : TriggerForAbility
    {
        private TriggeredAbility Ability;


        public AtTheStartOfEngagementPhase()
        {

        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.OnCombatPhaseStart,
                delegate { Ability.Action.DoAction(Ability); }
            );
        }
    }
}
