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

            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.StructuralDamageCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Messages.ShowInfo("Agility is restored");

            Host.Tokens.RemoveCondition(typeof(Tokens.StructuralDamageCritToken));
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;

            Host.AfterGetAgility -= ReduceAgility;
            Roster.UpdateShipStats(Host);
        }

        private void ReduceAgility(ref int value)
        {
            value--;
        }

    }

}