using System;
using Ship;
using Upgrade;
using Abilities;
using RuleSets;
using System.Collections.Generic;

namespace UpgradesList
{
	public class StealthDevice : GenericUpgrade, ISecondEditionUpgrade, IVariableCost
    {
		public StealthDevice() : base()
        {
            Types.Add(UpgradeType.Modification);
			Name = "Stealth Device";
			Cost = 3;

            UpgradeAbilities.Add(new StealthDeviceAbility());
		}

        public void AdaptUpgradeToSecondEdition()
        {
            //Nothing to do here, behavior is slightly different, see trigger below for stealth device ability
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 3},
                {1, 4},
                {2, 6},
                {3, 8}
            };

            Cost = agilityToCost[ship.Agility];
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
            if (RuleSet.Instance is FirstEdition)
            {
                HostShip.OnAttackHitAsDefender += RegisterStealthDeviceCleanup;
            }
            else
            {
                HostShip.OnDamageWasSuccessfullyDealt += RegisterStealthDeviceCleanupSe;
            }
        }

        public override void DeactivateAbility()
        {
            HostShip.ChangeAgilityBy(-1);
            HostShip.OnAttackHitAsDefender -= RegisterStealthDeviceCleanup;
            HostShip.OnDamageWasSuccessfullyDealt -= RegisterStealthDeviceCleanupSe;
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

        private void RegisterStealthDeviceCleanupSe(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Discard Stealth Device",
                TriggerType = TriggerTypes.OnDamageWasSuccessfullyDealt, //Stealth Device in SE is deactivated on damage taken
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