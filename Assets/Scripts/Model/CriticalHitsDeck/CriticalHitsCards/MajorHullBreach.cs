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
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Starting the round after you receive this card, all Damage cards dealt to you are dealt faceup.");
        }
    }

}