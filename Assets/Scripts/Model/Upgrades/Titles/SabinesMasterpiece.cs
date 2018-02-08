using Ship;
using Ship.TIEFighter;
using Upgrade;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UpgradesList
{
    public class SabinesMasterpiece : GenericUpgradeSlotUpgrade
    {
        public SabinesMasterpiece() : base()
        {
            Type = UpgradeType.Title;
            Name = "Sabine's Masterpiece";
            Cost = 1;
            isUnique = true;
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/sabines-masterpiece.png";
            AddedSlots = new List<UpgradeSlot>
             {
                new UpgradeSlot(UpgradeType.Crew) {  },
                new UpgradeSlot(UpgradeType.Illicit) { }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ((ship is TIEFighter) && (ship.faction == Faction.Rebel));
        }
    
    }
}
