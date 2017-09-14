using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;

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
            IsDetonationByContact = true;

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            //Roll 3 dice - suffer all results
            callBack();
        }

    }

}
