using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class StructuralDamage : GenericDamageCard
    {
        public StructuralDamage()
        {
            Name = "Structural Damage";
            Type = CriticalCardType.Ship;
            CancelDiceResults.Add(DieSide.Success);
            ImageUrl = "https://i.imgur.com/fJEhlAG.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGotNumberOfDefenceDice += DebuffDefenceRolls;
                        
            Host.Tokens.AssignCondition(new Tokens.StructuralDamageSECritToken(Host));
            Triggers.FinishTrigger();
        }

        private void DebuffDefenceRolls(ref int dice)
        {
            if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.ShipId == Host.ShipId)
            {
                Messages.ShowInfo("Structural Damage: Roll one fewer defence die");
                dice--;
            }
        }
                
        public override void DiscardEffect()
        {
            Host.Tokens.RemoveCondition(typeof(Tokens.StructuralDamageSECritToken));
            Host.AfterGotNumberOfDefenceDice -= DebuffDefenceRolls;
        }
    }

}