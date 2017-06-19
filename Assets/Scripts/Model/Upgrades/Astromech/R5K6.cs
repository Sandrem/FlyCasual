using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R5K6 : GenericUpgrade
    {

        public R5K6() : base()
        {
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R5-K6";
            ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/d/df/R5-K6.png";
            isUnique = true;
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterTokenIsSpent += R5K6Ability;
        }

        private void R5K6Ability(Ship.GenericShip ship, System.Type type)
        {
            if (type == typeof(Tokens.BlueTargetLockToken))
            {
                // TODO:
                // Astromech sound
                // Visual dice throwing
                int randomValue = Random.Range(0, 8);
                // Notification about result
                if (randomValue > 5)
                {
                    // Cannot be used now
                    Actions.AssignTargetLockToPair(Host, Selection.AnotherShip);
                }

            }
        }

    }

}
