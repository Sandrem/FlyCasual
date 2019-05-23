using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Phantom : GenericUpgrade
    {
        public Phantom() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Phantom",
                UpgradeType.Title,
                cost: 2,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Rebel),
                    new ShipRestriction(
                        typeof(Ship.SecondEdition.AttackShuttle.AttackShuttle),
                        typeof(Ship.SecondEdition.SheathipedeClassShuttle.SheathipedeClassShuttle)
                    )
                ),
                seImageNumber: 106
            );
        }        
    }
}