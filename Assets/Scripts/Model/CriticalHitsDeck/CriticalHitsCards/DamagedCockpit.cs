using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCard
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
            Phases.OnRoundStart += ApplyDelayedEffect;

            Host.Tokens.AssignCondition(new Tokens.DamagedCockpitCritToken(Host));
            Triggers.FinishTrigger();
        }

        private void ApplyDelayedEffect()
        {
            Phases.OnRoundStart -= ApplyDelayedEffect;

            Host.AddPilotSkillModifier(this);
            Roster.UpdateShipStats(Host);
        }

        public override void DiscardEffect()
        {
            Messages.ShowInfo("Pilot Skill is restored");

            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedCockpitCritToken));
            Host.RemovePilotSkillModifier(this);
            Roster.UpdateShipStats(Host);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 0;
        }
    }

}