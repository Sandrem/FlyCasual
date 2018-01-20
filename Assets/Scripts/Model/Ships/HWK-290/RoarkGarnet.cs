﻿using Ship;
using System.Collections.Generic;
using System;
using SubPhases;

namespace Ship
{
    namespace HWK290
    {
        public class RoarkGarnet : HWK290
        {
            public RoarkGarnet() : base()
            {
                PilotName = "Roark Garnet";
                PilotSkill = 4;
                Cost = 19;

                IsUnique = true;

                faction = Faction.Rebel;

                PilotAbilities.Add(new Abilities.RoarkGarnetAbility());
            }
        }
    }
}

namespace Abilities
{
    public class RoarkGarnetAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget, 
                    new List<TargetTypes> { TargetTypes.OtherFriendly }, 
                    new UnityEngine.Vector2(1, 3),
                    null,
                    false);                
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectAbilityTarget()
        {
            TargetShip.AddPilotSkillModifier(this);
            Phases.OnEndPhaseStart += RemovePilotSkillModifieer;
            SelectShipSubPhase.FinishSelection();
        }

        private void RemovePilotSkillModifieer()
        {
            Phases.OnEndPhaseStart -= RemovePilotSkillModifieer;
            TargetShip.RemovePilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 12;
        }
    }
}
