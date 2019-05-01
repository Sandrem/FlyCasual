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

        public event EventHandlerShipWeaponInt OnGetWeaponPriority;

        public CustomizedAi(GenericShip host)
        {
            Host = host;
        }

        public void CallGetWeaponPriority(GenericShip targetShip, IShipWeapon weapon, ref int priority)
        {
            OnGetWeaponPriority?.Invoke(targetShip, weapon, ref priority);
        }
    }
}
