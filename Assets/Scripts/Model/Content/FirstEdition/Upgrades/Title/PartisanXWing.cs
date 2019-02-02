using Ship;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;

namespace UpgradesList.FirstEdition
{
    public class PartisanXWing : GenericUpgrade
    {
        public PartisanXWing() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Partisan X-Wing",
                UpgradeType.Title,
                cost: 1,
                addSlots: new List<UpgradeSlot>()
                {
                new UpgradeSlot(UpgradeType.SalvagedAstromech),
                new UpgradeSlot(UpgradeType.Illicit)
                },
                forbidSlot: UpgradeType.Astromech,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.XWing.XWing))
            );

            FromMod = typeof(PartisanXWingMod);
            ImageUrl = "https://i.imgur.com/tkAl2Io.png";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipInfo.Shields < 3;
        }
    }
}