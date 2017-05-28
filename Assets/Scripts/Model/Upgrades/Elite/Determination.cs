using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class Determination : GenericUpgrade
    {

        public Determination(Ship.GenericShip host) : base(host)
        {
            Type = UpgradeSlot.Elite;
            Name = ShortName = "Determination";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/f/fc/Determination.jpg";

            host.OnAssignCrit += CancelPilotCrits;
        }

        private void CancelPilotCrits(Ship.GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit)
        {
            if (crit.Type == CriticalCardType.Pilot) {
                Game.UI.ShowInfo("Determination: Crit with \"Pilot\" trait is discarded");
                Game.UI.AddTestLogEntry("Determination: Crit with \"Pilot\" trait is discarded");
                crit = null;
            }
        }

    }

}
