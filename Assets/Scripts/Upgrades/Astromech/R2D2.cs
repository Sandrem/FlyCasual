using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class R2D2 : GenericUpgrade
    {

        public R2D2(Ship.GenericShip host) : base(host)
        {
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R2-D2";
            isUnique = true;

            host.OnMovementFinish += R2D2RegenShield;
        }

        private void R2D2RegenShield(Ship.GenericShip host)
        {
            if (host.AssignedManeuver.ColorComplexity == Ship.ManeuverColor.Green)
            {
                if (host.CanRegenShields())
                {
                    host.RestoreShield();
                    Game.UI.ShowInfo("R2-D2: Shield is restored");
                }
            }
        }

    }

}
