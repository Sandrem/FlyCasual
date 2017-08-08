using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class DamagedCockpit : GenericCriticalHit
    {
        public DamagedCockpit()
        {
            Name = "Damaged Cockpit";
            Type = CriticalCardType.Ship;
            ImageUrl = "http://i.imgur.com/SJjkO3L.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Game.UI.ShowInfo("Pilot Skill is set to 0");
            Game.UI.AddTestLogEntry("Pilot Skill is set to 0");
            Host.AssignToken(new Tokens.DamagedCockpitCritToken());

            Phases.OnRoundStart += ApplyDelayedEffect;

            Triggers.FinishTrigger();
        }

        private void ApplyDelayedEffect()
        {
            Host.AfterGetPilotSkill += SetPilotSkill0;
            Roster.UpdateShipStats(Host);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Pilot Skill is restored");
            Game.UI.AddTestLogEntry("Pilot Skill is restored");

            host.RemoveToken(typeof(Tokens.DamagedCockpitCritToken));
            host.AfterGetPilotSkill -= SetPilotSkill0;
            Roster.UpdateShipStats(host);
        }

        private void SetPilotSkill0(ref int value)
        {
            //BUG: No activations at all
            value = 0;
        }
    }

}