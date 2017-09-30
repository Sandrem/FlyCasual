﻿using Ship;
using Ship.YWing;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class BombLoadout : GenericUpgradeSlotUpgrade
    {
        public BombLoadout() : base()
        {
            Type = UpgradeType.Torpedo;
            Name = "Bomb Loadout";
            Cost = 0;
            Limited = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Bomb)
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YWing;
        }
    }
}
