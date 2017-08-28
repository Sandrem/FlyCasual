using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class StructuralDamage : GenericCriticalHit
    {
        public StructuralDamage()
        {
            Name = "Structural Damage";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/damage-decks/core-tfa/structural-damage.png";
            CancelDiceResults.Add(DiceSide.Success);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetAgility += ReduceAgility;
            Roster.UpdateShipStats(Host);

            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Host.AssignToken(new Tokens.StructuralDamageCritToken(), Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Agility is restored");

            host.RemoveToken(typeof(Tokens.StructuralDamageCritToken));
            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;

            host.AfterGetAgility -= ReduceAgility;
            Roster.UpdateShipStats(host);
        }

        private void ReduceAgility(ref int value)
        {
            value--;
        }

    }

}