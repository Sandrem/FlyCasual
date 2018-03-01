using System;
using Ship;
using Upgrade;

namespace UpgradesList
{
	public class StealthDevice : GenericUpgrade
    {
		public StealthDevice() : base() {
            Types.Add(UpgradeType.Modification);
			Name = "Stealth Device";
			Cost = 3;
		}

		public override void AttachToShip(GenericShip host)
		{
			base.AttachToShip (host);
			host.ChangeAgilityBy (1);
			host.OnAttackHitAsDefender += RegisterStealthDeviceCleanup;
		}

		private void RegisterStealthDeviceCleanup()
		{
            Triggers.RegisterTrigger(new Trigger {
                Name = "Discard Stealth Device",
                TriggerType = TriggerTypes.OnAttackHit,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = StealthDeviceClenup
            });
		}

        private void StealthDeviceClenup(object sender, System.EventArgs e)
        {
            Host.OnAttackHitAsDefender -= RegisterStealthDeviceCleanup;
            
            TryDiscard(Triggers.FinishTrigger);
        }

        public override void Discard(Action callBack)
        {
            Messages.ShowError("Hit! Discarding Stealth Device!");
            Host.ChangeAgilityBy(-1);

            base.Discard(callBack);
        }
    }
}