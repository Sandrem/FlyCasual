using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class MajorHullBreach : GenericCriticalHit
    {
        public MajorHullBreach()
        {
            Name = "Major Hull Breach";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/f/f3/Major-hull-breach.png";
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Starting the round after you receive this card, all Damage cards dealt to you are dealt faceup");
            Game.UI.AddTestLogEntry("Starting the round after you receive this card, all Damage cards dealt to you are dealt faceup");
            host.AssignToken(new Tokens.MajorHullBreachCritToken());

            Phases.OnPlanningPhaseStart += DealDamageCardFaceupStart;

            host.AfterGenerateAvailableActionsList += AddCancelCritAction;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Damage cards are dealt as usual");
            Game.UI.AddTestLogEntry("Damage cards are dealt as usual");
            host.RemoveToken(typeof(Tokens.MajorHullBreachCritToken));

            host.OnCheckFaceupCrit -= DealDamageCardFaceup;

            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }

        private void DealDamageCardFaceupStart()
        {
            Phases.OnPlanningPhaseStart -= DealDamageCardFaceupStart;
            host.OnCheckFaceupCrit += DealDamageCardFaceup;
        }

        private void DealDamageCardFaceup(ref bool result)
        {
            result = true;
        }

    }

}