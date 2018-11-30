using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class StealthDevice : GenericUpgrade
    {
        public StealthDevice() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Stealth Device",
                UpgradeType.Modification,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.StealthDeviceAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
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
            HostShip.OnAttackHitAsDefender -= RegisterStealthDeviceCleanup;
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

        protected void StealthDeviceCleanup(object sender, System.EventArgs e)
        {
            Messages.ShowError("Hit! Discarding Stealth Device!");
            HostUpgrade.Discard(Triggers.FinishTrigger);
        }
    }
}