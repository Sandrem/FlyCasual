using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SlaveI : GenericUpgrade
    {
        public SlaveI() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Slave I",
                UpgradeType.Title,
                cost: 5,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Torpedo),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.FiresprayClassPatrolCraft.FiresprayClassPatrolCraft)),
                abilityType: typeof(Abilities.FirstEdition.BobaFettEmpireAbility),
                seImageNumber: 154
            );
        }        
    }
}