using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class DamagedCockpit : GenericDamageCard, IModifyPilotSkill
    {
        public DamagedCockpit()
        {
            Name = "Damaged Cockpit";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Phases.Events.OnRoundStart += ApplyDelayedEffect;

            Host.Tokens.AssignCondition(typeof(Tokens.DamagedCockpitCritToken));
            Triggers.FinishTrigger();
        }

        private void ApplyDelayedEffect()
        {
            Phases.Events.OnRoundStart -= ApplyDelayedEffect;

            Host.State.AddPilotSkillModifier(this);
            Roster.UpdateShipStats(Host);
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Pilot Skill is restored");

            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedCockpitCritToken));
            Host.State.RemovePilotSkillModifier(this);
            Roster.UpdateShipStats(Host);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 0;
        }
    }

}