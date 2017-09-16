using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{
    public class BlindedPilot : GenericCriticalHit
    {
        public BlindedPilot()
        {
            Name = "Blinded Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryPerformAttack += OnTryPreformAttack;
            Host.AfterAttackWindow += DiscardEffect;

            Host.AssignToken(new Tokens.BlindedPilotCritToken(), Triggers.FinishTrigger);
        }

        private void OnTryPreformAttack(ref bool result)
        {
            Messages.ShowErrorToHuman("Blinded Pilot: Cannot perfom attack now");
            result = false;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Blinded Pilot: Crit is flipped, pilot can perfom attacks");

            host.OnTryPerformAttack -= OnTryPreformAttack;
            host.RemoveToken(typeof(Tokens.BlindedPilotCritToken));

            host.AfterAttackWindow -= DiscardEffect;
        }
    }

}