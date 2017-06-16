using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R5Astromech : GenericUpgrade
    {

        public R5Astromech() : base()
        {
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R5 Astromech";
            ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/3/3a/R5_Astromech.jpg";
            Cost = 1;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            Phases.OnEndPhaseStart += R5AstromechAbility;
        }

        private void R5AstromechAbility()
        {
            List<CriticalHitCard.GenericCriticalHit> critsList = Host.GetAssignedCritCards();
            if (critsList.Count > 0)
            {
                //TODO:
                //Select with window
                //Play sound // Sounds.PlaySoundOnce("R2D2-Proud");
                int randomIndex = Random.Range(0, critsList.Count);
                critsList[randomIndex].DiscardEffect(Host);
            }
        }

    }

}
