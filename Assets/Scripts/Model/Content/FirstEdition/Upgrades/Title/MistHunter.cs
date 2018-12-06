using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Actions;

namespace UpgradesList.FirstEdition
{
    public class MistHunter : GenericUpgrade
    {
        private readonly UpgradeSlot TractorBeamSlot;

        public MistHunter() : base()
        {
            TractorBeamSlot = new UpgradeSlot(UpgradeType.Cannon);

            UpgradeInfo = new UpgradeCardInfo(
                "Mist Hunter",
                UpgradeType.Title,
                cost: 0,       
                isLimited: true,
                addSlot: TractorBeamSlot,
                addAction: new ActionInfo(typeof(BarrelRollAction)),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.G1AStarfighter.G1AStarfighter))
            );
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            if (Global.ActiveScene != Global.Scene.Battle)
            {
                TractorBeamSlot.PreInstallUpgrade(new TractorBeam() { HostShip = host }, host);
                TractorBeamSlot.OnRemovePreInstallUpgrade += TractorBeamRemoved;
            }
        }

        private void TractorBeamRemoved(GenericUpgrade upgrade)
        {
            TractorBeamSlot.OnRemovePreInstallUpgrade -= TractorBeamRemoved;
            this.Slot.RemovePreInstallUpgrade();
        }

        public override void PreDettachFromShip()
        {
            if (Global.ActiveScene != Global.Scene.Battle)
            {
                TractorBeamSlot.RemovePreInstallUpgrade();
            }
            
            base.PreDettachFromShip();
        }
    }
}