using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class GrapplingStruts : GenericUpgrade
    {
        public GrapplingStruts() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Grappling Struts (Test)",
                UpgradeType.Configuration,
                cost: 0, //TODO
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.VultureClassDroidFighter.VultureClassDroidFighter)),
                abilityType: typeof(Abilities.SecondEdition.GrapplingStrutsAbility)
            );

            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/5d/Config_StrutsClosed.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GrapplingStrutsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.IsIgnoreObstacles = true;
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility() {}

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.IsLandedOnObstacle)
            {
                HostShip.TogglePeg(false);

                Animation animation = HostShip.GetModelTransform().Find("Vulture/Body").GetComponent<Animation>();
                animation.Play("Landing");
            }
        }
    }
}