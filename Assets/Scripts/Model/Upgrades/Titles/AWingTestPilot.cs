using Ship;
using Ship.AWing;
using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class AWingTestPilot : GenericUpgrade
    {
        public AWingTestPilot() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "A-Wing Test Pilot";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/a-wing-test-pilot.png";
            Cost = 0;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ((ship is AWing) && (ship.PilotSkill > 1));
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            Debug.Log("!!!");

            UpgradeSlot newSlot = new UpgradeSlot(UpgradeType.Elite) { GrantedBy = this, MustBeDifferent = true };
            host.UpgradeBar.AddSlot(newSlot);
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            Host.UpgradeBar.RemoveSlot(UpgradeType.Elite, this);
        }
    }
}
