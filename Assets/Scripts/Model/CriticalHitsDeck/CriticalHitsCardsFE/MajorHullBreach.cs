using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class MajorHullBreach : GenericDamageCard
    {
        public MajorHullBreach()
        {
            Name = "Major Hull Breach";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Phases.Events.OnPlanningPhaseStart += DealDamageCardFaceupStart;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.MajorHullBreachCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Damage cards are dealt as usual");
            Host.Tokens.RemoveCondition(typeof(Tokens.MajorHullBreachCritToken));

            Host.OnCheckFaceupCrit -= DealDamageCardFaceup;
            Host.OnGenerateActions -= CallAddCancelCritAction;
        }

        private void DealDamageCardFaceupStart()
        {
            Phases.Events.OnPlanningPhaseStart -= DealDamageCardFaceupStart;
            Host.OnCheckFaceupCrit += DealDamageCardFaceup;
        }

        private void DealDamageCardFaceup(ref bool result)
        {
            result = true;
        }

    }

}