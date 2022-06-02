using Upgrade;
using Ship;
using System.Collections.Generic;
using Content;

namespace UpgradesList.SecondEdition
{
    public class StealthDevice : GenericUpgrade
    {
        public StealthDevice() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Stealth Device",
                UpgradeType.Modification,
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.StealthDeviceAbility),
                seImageNumber: 77,
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );
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