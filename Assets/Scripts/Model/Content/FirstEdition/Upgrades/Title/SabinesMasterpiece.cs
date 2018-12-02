using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SabinesMasterpiece : GenericUpgrade
    {
        public SabinesMasterpiece() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sabine's Masterpiece",
                UpgradeType.Title,
                cost: 1,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new ShipRestriction(typeof(Ship.FirstEdition.TIEFighter.TIEFighter)),
                    new FactionRestriction(Faction.Rebel)
                ),
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.Crew),
                    new UpgradeSlot(UpgradeType.Illicit)
                }
            );

            // TODOREVERT
            // ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/sabines-masterpiece.png";
        }
    }
}