using Arcs;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Outrider : GenericUpgrade
    {
        public Outrider() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Outrider",
                UpgradeType.Title,
                cost: 5,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.YT2400.YT2400)),
                abilityType: typeof(Abilities.FirstEdition.OutriderAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class OutriderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ToggleOutriderAbility(true);
            HostShip.OnDiscardUpgrade += TurnOffOutriferAbilityIfCannon;
        }

        public override void DeactivateAbility()
        {
            ToggleOutriderAbility(false);
            HostShip.OnDiscardUpgrade -= TurnOffOutriferAbilityIfCannon;
        }

        private void ToggleOutriderAbility(bool isActive)
        {
            GenericSpecialWeapon cannon = (GenericSpecialWeapon)HostShip.UpgradeBar.GetInstalledUpgrade(UpgradeType.Cannon);

            if (cannon != null)
            {
                HostShip.ArcsInfo.GetArc<ArcPrimary>().ShotPermissions.CanShootPrimaryWeapon = !isActive;

                HostShip.ArcsInfo.GetArc<OutOfArc>().ShotPermissions.CanShootPrimaryWeapon = !isActive;
                HostShip.ArcsInfo.GetArc<OutOfArc>().ShotPermissions.CanShootCannon = isActive;

                // TODOREVERT
                //cannon.CanShootOutsideArc = isActive;
            }
        }

        private void TurnOffOutriferAbilityIfCannon()
        {
            if (GenericUpgrade.CurrentUpgrade.HasType(UpgradeType.Cannon))
            {
                ToggleOutriderAbility(false);
            }
        }

    }
}