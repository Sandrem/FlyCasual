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
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Pilot Skill is set to 0");
            Game.UI.AddTestLogEntry("Pilot Skill is set to 0");

            //TODO: Only starting from next round
            host.AfterGetPilotSkill += SetPilotSkill0;
            host.AssignToken(new Tokens.DamagedCockpitCritToken());
            Roster.UpdateShipStats(host);
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