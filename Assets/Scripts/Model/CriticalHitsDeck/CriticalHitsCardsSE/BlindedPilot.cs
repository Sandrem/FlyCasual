using System;
using System.Collections;
using System.Collections.Generic;
using ActionsList;
using UnityEngine;

namespace DamageDeckCardSE
{
    public class BlindedPilot : GenericDamageCard
    {
        public BlindedPilot()
        {
            Name = "Blinded Pilot";
            Type = CriticalCardType.Pilot;
            AiAvoids = true;
            ImageUrl = "https://i.imgur.com/OoQBMf7.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAvailableActionEffect += RestrictActionEffectsToForceOnly;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.BlindedPilotSECritToken(Host));
            Triggers.FinishTrigger();
        }

        private void RestrictActionEffectsToForceOnly(Ship.GenericShip ship, ActionsList.GenericAction action, ref bool data)
        {            
            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker.ShipId == Host.ShipId && !(action is ForceAction))
            {
                data = false;
            }
        }

        public override void DiscardEffect()
        {
            Host.OnTryAddAvailableActionEffect -= RestrictActionEffectsToForceOnly;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
            Messages.ShowInfo("Blinded Pilot: Crit is flipped, pilot can fully modify attacks");
            Host.Tokens.RemoveCondition(typeof(Tokens.BlindedPilotSECritToken));            
        }         
    }

}