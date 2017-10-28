using Ship;
using Upgrade;

namespace UpgradesList{
	public class StealthDevice : GenericUpgrade {
		public StealthDevice() : base() {
			Type = UpgradeType.Modification;
			Name = "Stealth Device";
			Cost = 3;
		}

		public override void AttachToShip(GenericShip host)
		{
			base.AttachToShip (host);
			host.ChangeAgilityBy (1);
			host.OnAttackHitAsDefender += StealthDeviceCleanup;
		}

		private void StealthDeviceCleanup()
		{
			Host.OnAttackHitAsDefender -= StealthDeviceCleanup;
			Messages.ShowError("Hit! Discarding Stealth Device!");
			Host.ChangeAgilityBy (-1);
			Discard ();
		}
	}
}