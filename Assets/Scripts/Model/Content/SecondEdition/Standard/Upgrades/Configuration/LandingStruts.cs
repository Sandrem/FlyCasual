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
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.HyenaClassDroidBomber.HyenaClassDroidBomber)),
                abilityType: typeof(Abilities.SecondEdition.LandingStrutsClosedAbility)
            );

            NameCanonical = "landingstruts";

            AnotherSide = typeof(LandingStrutsOpen);
            SelectSideOnSetup = false;
        }
    }

    public class LandingStrutsOpen : GenericDualUpgrade
    {
        public LandingStrutsOpen() : base()
        {
            IsHidden = true;
            NameCanonical = "landingstruts-sideb";

            UpgradeInfo = new UpgradeCardInfo(
                "Landing Struts (Open)",
                UpgradeType.Configuration,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.HyenaClassDroidBomber.HyenaClassDroidBomber)),
                abilityType: typeof(Abilities.SecondEdition.LandingStrutsOpenAbility)
            );

            AnotherSide = typeof(LandingStrutsClosed);
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