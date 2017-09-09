using Ship;
using Ship.TIEAdvanced;
using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class TIEx1 : GenericUpgrade
    {
        public TIEx1() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "TIE/x1";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/tie-x1.png";
            Cost = 0;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEAdvanced;
        }

        public override void SquadBuilderEffectApply(GenericShip host)
        {
            UpgradeSlot newSlot = new UpgradeSlot(UpgradeType.System) { GrantedBy = this, CostDecrease = 4 };
            host.UpgradeBar.AddSlot(newSlot);
        }

        public override void SquadBuilderEffectRemove(GenericShip host)
        {
            host.UpgradeBar.RemoveSlot(UpgradeType.System, this);
        }
    }
}
