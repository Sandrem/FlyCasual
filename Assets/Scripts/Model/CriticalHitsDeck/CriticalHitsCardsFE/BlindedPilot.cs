﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{
    public class BlindedPilot : GenericDamageCard
    {
        public BlindedPilot()
        {
            Name = "Blinded Pilot";
            Type = CriticalCardType.Pilot;
            AiAvoids = true;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryPerformAttack += OnTryPreformAttack;
            Host.AfterAttackWindow += DiscardEffect;

            Host.Tokens.AssignCondition(typeof(Tokens.BlindedPilotCritToken));
            Triggers.FinishTrigger();
        }

        private void OnTryPreformAttack(ref bool result, List<string> stringList)
        {
            stringList.Add("Blinded Pilot: Cannot perfom attack now");
            result = false;
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Blinded Pilot: Crit is flipped, pilot can perfom attacks");

            Host.OnTryPerformAttack -= OnTryPreformAttack;
            Host.Tokens.RemoveCondition(typeof(Tokens.BlindedPilotCritToken));

            Host.AfterAttackWindow -= DiscardEffect;
        }
    }

}