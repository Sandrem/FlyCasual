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
            ImageUrl = "http://i.imgur.com/4p5MZvU.jpg";
            CancelDiceResults.Add(DiceSide.Success);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Messages.ShowInfo("Agility is reduced by 1");
            Game.UI.AddTestLogEntry("Agility is reduced by 1");
            Host.AssignToken(new Tokens.StructuralDamageCritToken());

            Host.AfterGetAgility += ReduceAgility;
            Roster.UpdateShipStats(Host);

            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Triggers.FinishTrigger();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Agility is restored");
            Game.UI.AddTestLogEntry("Agility is restored");
            host.RemoveToken(typeof(Tokens.StructuralDamageCritToken));

            host.AfterGetAgility -= ReduceAgility;
            Roster.UpdateShipStats(host);

            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }

        private void ReduceAgility(ref int value)
        {
            value--;
        }

    }

}