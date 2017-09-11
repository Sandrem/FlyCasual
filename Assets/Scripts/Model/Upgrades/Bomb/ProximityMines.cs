using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ProximityMines : GenericBomb
    {

        public ProximityMines() : base()
        {
            Type = UpgradeType.Bomb;
            Name = ShortName = "Proximity Mines";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Bomb/proximity-mines.png";
            Cost = 3;

            bombPrefabPath = "Prefabs/Bombs/ProximityMine";

            IsDropAsAction = true;
            IsDiscardedAfterDropped = true;
        }

    }

}
