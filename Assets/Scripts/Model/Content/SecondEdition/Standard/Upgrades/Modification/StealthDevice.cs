using Upgrade;
using Ship;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class StealthDevice : GenericUpgrade, IVariableCost
    {
        public StealthDevice() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Stealth Device",
                UpgradeType.Modification,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.StealthDeviceAbility),
                seImageNumber: 77
            );
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

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StealthDeviceAbility : Abilities.FirstEdition.StealthDeviceAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ShipInfo.Agility++;
            HostShip.ChangeAgilityBy(1);

            HostShip.OnDamageWasSuccessfullyDealt += RegisterStealthDeviceCleanupSe;
        }

        public override void DeactivateAbility()
        {
            HostShip.ShipInfo.Agility--;
            HostShip.ChangeAgilityBy(-1);

            HostShip.OnDamageWasSuccessfullyDealt -= RegisterStealthDeviceCleanupSe;
        }

        private void RegisterStealthDeviceCleanupSe(GenericShip ship, bool isCritical)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Discard Stealth Device",
                TriggerType = TriggerTypes.OnDamageWasSuccessfullyDealt, //Stealth Device in SE is deactivated on damage taken
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = StealthDeviceCleanup
            });
        }
    }
}