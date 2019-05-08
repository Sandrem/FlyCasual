using System;
using System.Collections;
using System.Collections.Generic;
using ActionsList;
using Ship;
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
            ImageUrl = "https://i.imgur.com/cBMWZZh.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAvailableDiceModification += RestrictActionEffectsToForceOnly;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.BlindedPilotSECritToken));
            Triggers.FinishTrigger();
        }

        private void RestrictActionEffectsToForceOnly(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {            
            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker.ShipId == Host.ShipId && !(action is ForceAction))
            {
                canBeUsed = false;
            }
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.OnTryAddAvailableDiceModification -= RestrictActionEffectsToForceOnly;
            Host.OnGenerateActions -= CallAddCancelCritAction;
            Messages.ShowInfo(Host.PilotInfo.PilotName + " is no longer Blinded and may fully modify their attacks");
            Host.Tokens.RemoveCondition(typeof(Tokens.BlindedPilotSECritToken));            
        }         
    }

}