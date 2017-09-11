using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class SeismicCharges : GenericBomb
    {

        public SeismicCharges() : base()
        {
            Type = UpgradeType.Bomb;
            Name = ShortName = "Seismic Charges";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Bomb/seismic-charges.png";
            Cost = 2;

            bombPrefabPath = "Prefabs/Bombs/SeismicCharge";

            IsDropAfterManeuverRevealed = true;
            IsDiscardedAfterDropped = true;
        }

    }

}
