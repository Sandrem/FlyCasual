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

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Cannot perform attack next time");
            Game.UI.AddTestLogEntry("Cannot perform attack next time");

            host.OnTryPerformAttack += OnTryPreformAttack;
            host.AssignToken(new Tokens.BlindedPilotCritToken());

            host.AfterAttackWindow += DiscardEffect;
        }

        private void OnTryPreformAttack(ref bool result)
        {
            Game.UI.ShowError("Blinded Pilot: Cannot perfom attack now");
            result = false;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Blinded Pilot: Crit is flipped, pilot can perfom attacks");
            Game.UI.AddTestLogEntry("Blinded Pilot: Crit is flipped, pilot can perfom attacks");

            host.OnTryPerformAttack -= OnTryPreformAttack;
            host.RemoveToken(typeof(Tokens.BlindedPilotCritToken));

            host.AfterAttackWindow -= DiscardEffect;
        }
    }

}