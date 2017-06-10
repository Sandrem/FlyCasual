using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R2D2 : GenericUpgrade
    {

        public R2D2() : base()
        {
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R2-D2";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/5/57/R2-d2.png";
            isUnique = true;
            Cost = 4;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnMovementFinish += R2D2RegenShield;
        }

        private void R2D2RegenShield(Ship.GenericShip host)
        {
            if (host.AssignedManeuver.ColorComplexity == Ship.ManeuverColor.Green)
            {
                if (host.TryRegenShields())
                {
                    AudioSource audio = Selection.ThisShip.Model.GetComponent<AudioSource>();
                    audio.PlayOneShot((AudioClip)Resources.Load("Sounds/R2D2-Proud"));

                    Game.UI.ShowInfo("R2-D2: Shield is restored");
                }
            }
        }

    }

}
