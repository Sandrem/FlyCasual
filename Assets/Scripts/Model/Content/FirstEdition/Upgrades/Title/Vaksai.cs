using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Vaksai : GenericUpgrade
    {
        public Vaksai() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Vaksai",
                UpgradeType.Title,
                cost: 0,
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true },
                    new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true }
                },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.KihraxzFighter.KihraxzFighter))
            );
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            foreach (var slot in host.UpgradeBar.GetUpgradeSlots())
            {
                slot.CostDecrease++;
            }
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            foreach (var slot in Host.UpgradeBar.GetUpgradeSlots())
            {
                slot.CostDecrease--;
            }
        }
    }
}

