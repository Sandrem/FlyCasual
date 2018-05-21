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
            Phases.OnPlanningPhaseStart += DealDamageCardFaceupStart;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.MajorHullBreachCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Messages.ShowInfo("Damage cards are dealt as usual");
            Host.Tokens.RemoveCondition(typeof(Tokens.MajorHullBreachCritToken));

            Host.OnCheckFaceupCrit -= DealDamageCardFaceup;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

        private void DealDamageCardFaceupStart()
        {
            Phases.OnPlanningPhaseStart -= DealDamageCardFaceupStart;
            Host.OnCheckFaceupCrit += DealDamageCardFaceup;
        }

        private void DealDamageCardFaceup(ref bool result)
        {
            result = true;
        }

    }

}