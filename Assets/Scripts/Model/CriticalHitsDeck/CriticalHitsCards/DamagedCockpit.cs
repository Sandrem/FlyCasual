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
            ImageUrl = "http://i.imgur.com/SJjkO3L.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Messages.ShowInfo("Pilot Skill is set to 0");
            Game.UI.AddTestLogEntry("Pilot Skill is set to 0");
            Host.AssignToken(new Tokens.DamagedCockpitCritToken());

            Phases.OnRoundStart += ApplyDelayedEffect;

            Triggers.FinishTrigger();
        }

        private void ApplyDelayedEffect()
        {
            Host.AddPilotSkillModifier(this);
            Roster.UpdateShipStats(Host);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Pilot Skill is restored");
            Game.UI.AddTestLogEntry("Pilot Skill is restored");

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