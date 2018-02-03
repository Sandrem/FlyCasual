using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCard
{

    public class StructuralDamage : GenericDamageCard
    {
        public StructuralDamage()
        {
            Name = "Structural Damage";
            Type = CriticalCardType.Ship;
            CancelDiceResults.Add(DieSide.Success);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetAgility += ReduceAgility;
            Roster.UpdateShipStats(Host);

            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.StructuralDamageCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Agility is restored");

            host.Tokens.RemoveCondition(typeof(Tokens.StructuralDamageCritToken));
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