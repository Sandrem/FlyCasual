using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    public class ShipUpgradesInfo
    {
        public List<UpgradeType> Upgrades { get; private set; }

        public ShipUpgradesInfo(params UpgradeType[] upgrades)
        {
            Upgrades = upgrades.ToList();
        }
    }
}
