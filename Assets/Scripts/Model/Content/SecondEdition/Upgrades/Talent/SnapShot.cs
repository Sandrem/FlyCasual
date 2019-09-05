using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Tokens;
using Ship;

namespace UpgradesList.SecondEdition
{
    public class SnapShot : GenericSpecialWeapon
    {
        public SnapShot() : base()
        {
            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo(
                "SnapShot",
                UpgradeType.Talent,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    // Hacking the range to remove this as a possible weapon when ability is not triggered
                    minRange: -1,
                    maxRange: -1
                ),
                abilityType: typeof(Abilities.SecondEdition.SnapShotAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0c/6b/0c6b9e6c-7c2f-4322-bcf0-f6f2fce44323/swz47_upgrade-snap-shot.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SnapShotAbility : Abilities.FirstEdition.SnapShotAbility
    {
        protected override void EnableWeaponRange()
        {
            (HostUpgrade as IShipWeapon).WeaponInfo.MaxRange = 2;
            (HostUpgrade as IShipWeapon).WeaponInfo.MinRange = 2;
        }

        protected override void SnapShotRestrictionForDefender(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.DiceModificationTiming == DiceModificationTimingType.Opposite)
            {
                Messages.ShowErrorToHuman("Snap Shot: You cannot modify attacker's attack dice");
                canBeUsed = false;
            }
        }
    }
}