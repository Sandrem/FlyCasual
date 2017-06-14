using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        public delegate void EventHandler();
        public delegate void EventHandlerInt(ref int data);
        public delegate void EventHandlerBool(ref bool data);
        public delegate void EventHandlerActionBool(ActionsList.GenericAction action, ref bool data);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement movement);
        public delegate void EventHandlerShipCrit(GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit);

        public event EventHandlerShip AfterStatsAreChanged;
        public event EventHandlerInt AfterGetPilotSkill;
        public event EventHandlerInt AfterGetAgility;

    }

}
