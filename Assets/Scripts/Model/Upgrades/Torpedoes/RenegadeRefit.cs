using Ship;
using Ship.UWing;
using Ship.XWing;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList
{

    public class RenegadeRefit : GenericUpgradeSlotUpgrade
    {
        public RenegadeRefit() : base()
        {
            Types.Add(UpgradeType.Torpedo);

            Name = "Renegade Refit";
            Cost = -2;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is XWing || ship is UWing;
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            foreach (var slot in host.UpgradeBar.GetUpgradeSlots().Where(s => s.Type == UpgradeType.Elite))
            {
                slot.CostDecrease++;
            }
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            foreach (var slot in Host.UpgradeBar.GetUpgradeSlots().Where(s => s.Type == UpgradeType.Elite))
            {
                slot.CostDecrease--;
            }
        }
    }
}

