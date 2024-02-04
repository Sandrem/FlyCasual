using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class XanaduBlood : GenericUpgrade
    {
        public XanaduBlood() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Xanadu Blood",
                UpgradeType.Title,
                cost: 0,       
                isLimited: true,
                addSlots: new List<UpgradeSlot>
                {
                    new UpgradeSlot(UpgradeType.Crew),
                    new UpgradeSlot(UpgradeType.Device),
                },
                addAction: new ActionInfo(typeof(CloakAction), ActionColor.Red),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.RogueClassStarfighter.RogueClassStarfighter))
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/xanadublood.png";
        }
    }
}