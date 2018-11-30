using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.Hwk290
    {
        public class RoarkGarnet : Hwk290
        {
            public RoarkGarnet() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Roark Garnet",
                    4,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.RoarkGarnetAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class RoarkGarnetAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        protected virtual string GenerateAbilityMessage()
        {
            return "Choose another friendly ship in arc.\nUntil the end of the phase, treat that ship's pilot skill value as \"12\".";
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotName,
                    GenerateAbilityMessage(),
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (ActionsHolder.HasTarget(ship)) result += 100;
            result += (12 - ship.State.Initiative);
            return result;
        }

        private void SelectAbilityTarget()
        {
            TargetShip.State.AddPilotSkillModifier(this);
            Phases.Events.OnEndPhaseStart_NoTriggers += RemovePilotSkillModifieer;
            SelectShipSubPhase.FinishSelection();
        }

        private void RemovePilotSkillModifieer()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= RemovePilotSkillModifieer;
            TargetShip.State.RemovePilotSkillModifier(this);
        }

        public virtual void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 12;
        }
    }
}