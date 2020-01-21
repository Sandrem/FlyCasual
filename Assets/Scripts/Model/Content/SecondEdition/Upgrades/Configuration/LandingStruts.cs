using BoardTools;
using Obstacles;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class LandingStrutsClosed : GenericDualUpgrade
    {
        public LandingStrutsClosed() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Landing Struts (Closed)",
                UpgradeType.Configuration,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.HyenaClassDroidBomber.HyenaClassDroidBomber)),
                abilityType: typeof(Abilities.SecondEdition.LandingStrutsClosedAbility)
            );

            AnotherSide = typeof(LandingStrutsOpen);
            SelectSideOnSetup = false;

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/c23a0130bad7330c0abb6218745910aa.png";
        }
    }

    public class LandingStrutsOpen : GenericDualUpgrade
    {
        public LandingStrutsOpen() : base()
        {
            IsHidden = true;
            NameCanonical = "landingstruts-anotherside";

            UpgradeInfo = new UpgradeCardInfo(
                "Landing Struts (Open)",
                UpgradeType.Configuration,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.HyenaClassDroidBomber.HyenaClassDroidBomber)),
                abilityType: typeof(Abilities.SecondEdition.LandingStrutsOpenAbility)
            );

            AnotherSide = typeof(LandingStrutsClosed);

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/aba79159308e850b635f6c43721ccdee.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandingStrutsClosedAbility : GrapplingStrutsClosedAbility
    {
        protected override void PlayLandingAnimation()
        {
            Animation animation = HostShip.GetModelTransform().Find("Hyena-class Droid Bomber/Body").GetComponent<Animation>();
            animation.Play("HyenaLanding");
        }
    }

    public class LandingStrutsOpenAbility : GrapplingStrutsOpenAbility
    {
        protected override void PlayTakeoffAnimation()
        {
            Animation animation = HostShip.GetModelTransform().Find("Hyena-class Droid Bomber/Body").GetComponent<Animation>();
            animation.Play("HyenaTakeoff");
        }
    }
}