using Ship;
using Ship.YT2400;
using Upgrade;
using System.Linq;
using System;

namespace UpgradesList
{
    public class Outrider : GenericUpgrade
    {
        public Outrider() : base()
        {
            Type = UpgradeType.Title;
            Name = "Outrider";
            Cost = 5;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YT2400;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            ToggleOutriderAbility(true);
            Host.OnDiscardUpgrade += TurnOffOutriferAbilityIfCannon;
        }

        public override void Discard(Action callBack)
        {
            ToggleOutriderAbility(false);

            base.Discard(callBack);
        }

        private void ToggleOutriderAbility(bool isActive)
        {
            GenericSecondaryWeapon cannon = (GenericSecondaryWeapon)Host.UpgradeBar.GetUpgradesAll().Find(n => n.Type == UpgradeType.Cannon);

            if (cannon != null)
            {
                Host.ArcInfo.OutOfArcShotPermissions.CanShootPrimaryWeapon = !isActive;
                Host.ArcInfo.GetPrimaryArc().ShotPermissions.CanShootPrimaryWeapon = !isActive;

                Host.ArcInfo.OutOfArcShotPermissions.CanShootCannon = isActive;
                cannon.CanShootOutsideArc = isActive;
            }
        }

        private void TurnOffOutriferAbilityIfCannon()
        {
            if (CurrentUpgrade.Type == UpgradeType.Cannon)
            {
                ToggleOutriderAbility(false);
            }
        }

    }
}
