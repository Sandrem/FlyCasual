using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class IonCannonTurret : GenericSecondaryWeapon
    {
        public IonCannonTurret() : base()
        {
            Type = UpgradeSlot.Turret;

            Name = "Ion Cannon Turret";
            ShortName = "Ion Cannon Turret";
            ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/f/f3/Ion_Cannon_Turret.png";
            Cost = 5;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);
        }

        public override bool IsShotAvailable(Ship.GenericShip anotherShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            int distance = Actions.GetRange(Host, anotherShip);
            if (distance < MinRange) return false;
            if (distance > MaxRange) return false;

            return result;
        }

    }

}