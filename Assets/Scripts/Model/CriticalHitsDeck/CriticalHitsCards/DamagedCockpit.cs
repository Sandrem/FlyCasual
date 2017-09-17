using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class DamagedCockpit : GenericCriticalHit, IModifyPilotSkill
    {
        public DamagedCockpit()
        {
            Name = "Damaged Cockpit";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Phases.OnRoundStart += ApplyDelayedEffect;

            Host.AssignToken(new Tokens.DamagedCockpitCritToken(), Triggers.FinishTrigger);
        }

        private void ApplyDelayedEffect()
        {
            Host.AddPilotSkillModifier(this);
            Roster.UpdateShipStats(Host);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Pilot Skill is restored");

            host.RemoveToken(typeof(Tokens.DamagedCockpitCritToken));
            host.RemovePilotSkillModifier(this);
            Roster.UpdateShipStats(host);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 0;
        }
    }

}