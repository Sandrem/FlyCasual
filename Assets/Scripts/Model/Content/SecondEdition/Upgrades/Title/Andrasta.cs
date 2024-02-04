using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Andrasta : GenericUpgrade
    {
        public Andrasta() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Andrasta",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Device),
                addAction: new ActionInfo(typeof(ReloadAction)),
                restrictions: new UpgradeCardRestrictions(
                    new ShipRestriction(typeof(Ship.SecondEdition.FiresprayClassPatrolCraft.FiresprayClassPatrolCraft)),
                    new FactionRestriction(Faction.Scum)
                ),
                seImageNumber: 146
            );
        }        
    }
}