using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCard
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
            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.MajorHullBreachCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Damage cards are dealt as usual");
            host.Tokens.RemoveCondition(typeof(Tokens.MajorHullBreachCritToken));

            host.OnCheckFaceupCrit -= DealDamageCardFaceup;
            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
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