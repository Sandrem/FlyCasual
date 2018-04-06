using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Abilities;

namespace UpgradesList
{
    public class MistHunter : GenericUpgradeSlotUpgrade
    {
        private readonly UpgradeSlot TractorBeamSlot;

        public MistHunter() : base()
        { 
            Types.Add(UpgradeType.Title);
            Name = "Mist Hunter";
            Cost = 0;
            isUnique = true;

            TractorBeamSlot = new UpgradeSlot(UpgradeType.Cannon);
            AddedSlots = new List<UpgradeSlot> { TractorBeamSlot };

            UpgradeAbilities.Add(new GenericActionBarAbility<BarrelRollAction>());    
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);
            if (Global.ActiveScene != Global.Scene.Battle)
            {
                TractorBeamSlot.PreInstallUpgrade(new TractorBeam() { Host = host }, host);
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

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Ship.G1AStarfighter.G1AStarfighter;
        }
        
    }
}
