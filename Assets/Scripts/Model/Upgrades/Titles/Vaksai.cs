using Ship;
using Ship.Kihraxz;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class Vaksai : GenericUpgradeSlotUpgrade
    {
        public Vaksai() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Vaksai";
            Cost = 0;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true },
                new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Kihraxz;
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
