using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R2Astromech : GenericUpgrade
    {

        public R2Astromech() : base()
        {
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R2 Astromech";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/6/6a/R2_Astromech.jpg";
            Cost = 1;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGetManeuverColorDecreaseComplexity += R2AstromechAbility;
        }

        private void R2AstromechAbility(Ship.GenericShip ship, ref Movement movement)
        {
            if (movement.ColorComplexity != Ship.ManeuverColor.None)
            {
                if ((movement.Speed == ManeuverSpeed.Speed1) || (movement.Speed == ManeuverSpeed.Speed2))
                {
                    movement.ColorComplexity = Ship.ManeuverColor.Green;
                }
            }
        }

    }

}
