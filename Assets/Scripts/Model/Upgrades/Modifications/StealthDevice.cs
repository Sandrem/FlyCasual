using System;
using Ship;
using Upgrade;
using Abilities;

namespace UpgradesList
{
	public class StealthDevice : GenericUpgrade
    {
		public StealthDevice() : base()
        {
            Types.Add(UpgradeType.Modification);
			Name = "Stealth Device";
			Cost = 3;

            UpgradeAbilities.Add(new StealthDeviceAbility());
		}
    }
}

namespace Abilities
{
    public class StealthDeviceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ChangeAgilityBy(1);
            HostShip.OnAttackHitAsDefender += RegisterStealthDeviceCleanup;
        }

        public override void DeactivateAbility()
        {
            HostShip.ChangeAgilityBy(-1);
            HostShip.OnAttackHitAsDefender += RegisterStealthDeviceCleanup;
        }

        private void RegisterStealthDeviceCleanup()
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Discard Stealth Device",
                TriggerType = TriggerTypes.OnAttackHit,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = StealthDeviceCleanup
            });
        }

        private void StealthDeviceCleanup(object sender, System.EventArgs e)
        {
            Messages.ShowError("Hit! Discarding Stealth Device!");
            HostUpgrade.Discard(Triggers.FinishTrigger);
        }
    }
}