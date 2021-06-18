using Ship;
using SubPhases;
using System;
using UnityEngine;

namespace Abilities
{
    public class AtTheStartOfPhase : TriggerForAbility
    {
        private TriggeredAbility Ability;
        public Type SubPhaseType { get; }

        public AtTheStartOfPhase(Type subPhaseType)
        {
            SubPhaseType = subPhaseType;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;

            if (SubPhaseType == typeof(SubPhases.CombatStartSubPhase))
            {
                Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
            }
            else if (SubPhaseType == typeof(SubPhases.EndStartSubPhase))
            {
                Phases.Events.OnEndPhaseStart_NoTriggers += RegisterAbility;
            }
            else
            {
                Debug.LogError("Unknown subphase: cannot subscribe to event");
            }
        }

        public override void Unregister(TriggeredAbility ability)
        {
            if (SubPhaseType == typeof(SubPhases.CombatStartSubPhase))
            {
                Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
            }
            else if (SubPhaseType == typeof(SubPhases.EndStartSubPhase))
            {
                Phases.Events.OnEndPhaseStart_NoTriggers -= RegisterAbility;
            }
            else
            {
                Debug.LogError("Unknown subphase: cannot subscribe to event");
            }
        }

        private void RegisterAbility()
        {
            Ability.RegisterAbilityTrigger
            (
                GetTriggerType(),
                delegate { Ability.Action.DoAction(Ability); }
            );
        }

        private TriggerTypes GetTriggerType()
        {
            if (SubPhaseType == typeof(SubPhases.CombatStartSubPhase))
            {
                return TriggerTypes.OnCombatPhaseStart;
            }
            else if (SubPhaseType == typeof(SubPhases.EndStartSubPhase))
            {
                return TriggerTypes.OnEndPhaseStart;
            }
            else
            {
                Debug.LogError("Unknown subphase: cannot set correct trigger type");
                return TriggerTypes.None;
            }
        }
    }
}
