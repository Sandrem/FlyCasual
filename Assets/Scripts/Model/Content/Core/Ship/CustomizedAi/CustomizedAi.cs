using ActionsList;
using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ship
{
    public class CustomizedAi
    {
        public GenericShip Host { get; private set; }

        public delegate void EventHandlerShipWeaponInt(GenericShip targetShip, IShipWeapon weapon, ref int priority);
        public delegate void EventHandlerArcFacingInt(ArcFacing facing, ref int priority);
        public delegate void EventHandlerActionInt(GenericAction action, ref int priority);

        public event EventHandlerShipWeaponInt OnGetWeaponPriority;
        public event EventHandlerArcFacingInt OnGetRotateArcFacingPriority;
        public event EventHandlerActionInt OnGetActionPriority;

        public CustomizedAi(GenericShip host)
        {
            Host = host;
        }

        public void CallGetWeaponPriority(GenericShip targetShip, IShipWeapon weapon, ref int priority)
        {
            OnGetWeaponPriority?.Invoke(targetShip, weapon, ref priority);
        }

        public void CallGetRotateArcFacingPriority(ArcFacing facing, ref int priority)
        {
            OnGetRotateArcFacingPriority?.Invoke(facing, ref priority);
        }

        public void CallGetActionPriority(GenericAction action, ref int priority)
        {
            OnGetActionPriority?.Invoke(action, ref priority);
        }
    }
}
