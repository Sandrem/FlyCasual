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

            //arc
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            SubscribeOnHit();
        }

        private void SubscribeOnHit()
        {
            //on hit: cancel result, deal damage, assign ion
        }

    }

}