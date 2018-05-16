using Ship;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BoardTools
{

    public class ShipShotDistanceInformation : GeneralShipDistanceInformation
    {
        public bool IsObstructedByAsteroid { get; private set; }
        public bool IsObstructedByBombToken { get; private set; }

        //TODO: Change to arctype
        public bool IsShotAvailable { get; private set; }
        public bool InArc { get; private set; }
        public bool InPrimaryArc { get; private set; }
        public bool InBullseyeArc { get; private set; }
        public bool InMobileArc { get; private set; }
		public bool InRearAuxArc { get; private set; }
        
        //TODO: Change to IWeapon
        public bool CanShootPrimaryWeapon { get; private set; }
        public bool CanShootTorpedoes { get; private set; }
        public bool CanShootMissiles { get; private set; }
        public bool CanShootCannon { get; private set; }
        public bool CanShootTurret { get; private set; }

        public new int Range
        {
            get
            {
                int distance = Mathf.Max(1, Mathf.CeilToInt(Distance / DISTANCE_1));

                if (OnRangeIsMeasured != null) OnRangeIsMeasured(ThisShip, AnotherShip, ChosenWeapon, ref distance);

                return distance;
            }
        }

        IShipWeapon ChosenWeapon { get; set; }

        //EVENTS
        public delegate void EventHandlerShipShipWeaponInt(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range);
        public static event EventHandlerShipShipWeaponInt OnRangeIsMeasured;

        public ShipShotDistanceInformation(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon = null) : base(thisShip, anotherShip)
        {
            ChosenWeapon = chosenWeapon ?? thisShip.PrimaryWeapon;
            
            Calculate();
        }

        protected override void Calculate()
        {
            
        }

        public void CheckObstruction(Action callBack)
        {
            callBack();
        }
    }

}
