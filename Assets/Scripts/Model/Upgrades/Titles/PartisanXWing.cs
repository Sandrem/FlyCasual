﻿using Ship;
using Ship.XWing;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;

namespace UpgradesList
{
    public class PartisanXWing : GenericUpgradeSlotUpgrade
    {
        public PartisanXWing() : base()
        {
            FromMod = typeof(PartisanXWingMod);

            Type = UpgradeType.Title;
            Name = "Partisan X-Wing";
            Cost = 1;

            ImageUrl = "https://i.imgur.com/9hmgWSk.jpg";

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.SalvagedAstromech),
                new UpgradeSlot(UpgradeType.Illicit)
            };
            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Astromech
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is XWing) && (ship.MaxShields < 3);
        }
    }
}
