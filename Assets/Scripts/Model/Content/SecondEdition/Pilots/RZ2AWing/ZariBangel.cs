using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class ZariBangel : RZ2AWing
        {
            public ZariBangel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zari Bangel",
                    3,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZariBangelAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent }
                );

                ModelInfo.SkinName = "Blue";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/d7f37dbb86bb706dd535e9a65b69149a.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZariBangelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformActionsWhenBumped = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformActionsWhenBumped = false;
        }
    }
}