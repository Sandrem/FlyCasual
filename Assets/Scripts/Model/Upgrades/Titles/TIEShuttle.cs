using Ship;
using Ship.TIEBomber;
using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class TIEShuttle : GenericUpgrade
    {
        public TIEShuttle() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "TIE Shuttle";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/tie-shuttle.png";
            Cost = 0;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEBomber;
        }

        public override void SquadBuilderEffectApply(GenericShip host)
        {
            host.UpgradeBar.ForbidSlots(UpgradeType.Torpedoes);
            host.UpgradeBar.ForbidSlots(UpgradeType.Missiles);
            host.UpgradeBar.ForbidSlots(UpgradeType.Bomb);

            UpgradeSlot crew1 = new UpgradeSlot(UpgradeType.Crew) { GrantedBy = this, MaxCost = 4 };
            host.UpgradeBar.AddSlot(crew1);

            UpgradeSlot crew2 = new UpgradeSlot(UpgradeType.Crew) { GrantedBy = this, MaxCost = 4 };
            host.UpgradeBar.AddSlot(crew2);
        }

        public override void SquadBuilderEffectRemove(GenericShip host)
        {
            host.UpgradeBar.AllowSlots(UpgradeType.Torpedoes);
            host.UpgradeBar.AllowSlots(UpgradeType.Missiles);
            host.UpgradeBar.AllowSlots(UpgradeType.Bomb);

            host.UpgradeBar.RemoveSlot(UpgradeType.Crew, this);
            host.UpgradeBar.RemoveSlot(UpgradeType.Crew, this);
        }
    }
}
