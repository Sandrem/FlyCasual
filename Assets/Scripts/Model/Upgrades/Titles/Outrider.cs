using Ship;
using Ship.YT2400;
using Upgrade;
using System.Linq;
using System;
using Abilities;

namespace UpgradesList
{
    public class Outrider : GenericUpgrade
    {
        public Outrider() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Outrider";
            Cost = 5;

            isUnique = true;

            UpgradeAbilities.Add(new OutriderAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YT2400;
        }
    }
}

namespace Abilities
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
            GenericSecondaryWeapon cannon = (GenericSecondaryWeapon)HostShip.UpgradeBar.GetInstalledUpgrade(UpgradeType.Cannon);

            if (cannon != null)
            {
                HostShip.ArcInfo.OutOfArcShotPermissions.CanShootPrimaryWeapon = !isActive;
                HostShip.ArcInfo.GetPrimaryArc().ShotPermissions.CanShootPrimaryWeapon = !isActive;

                HostShip.ArcInfo.OutOfArcShotPermissions.CanShootCannon = isActive;
                cannon.CanShootOutsideArc = isActive;
            }
        }

        private void TurnOffOutriferAbilityIfCannon()
        {
            if (GenericUpgrade.CurrentUpgrade.hasType(UpgradeType.Cannon))
            {
                ToggleOutriderAbility(false);
            }
        }

    }
}
