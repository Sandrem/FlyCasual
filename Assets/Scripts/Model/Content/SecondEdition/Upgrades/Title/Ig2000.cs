using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Ig2000 : GenericUpgrade
    {
        public Ig2000() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "IG-2000",
                UpgradeType.Title,
                cost: 2,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Aggressor.Aggressor)),
                abilityType: typeof(Abilities.FirstEdition.Ig2000Ability),
                seImageNumber: 149
            );
        }        
    }
}