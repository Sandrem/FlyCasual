using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Tokens;
using Ship;

namespace UpgradesList.SecondEdition
{
    public class SnapShot : GenericSpecialWeapon, IVariableCost
    {
        public SnapShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "SnapShot",
                UpgradeType.Talent,
                cost: 7,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 2,
                    maxRange: 2
                ),
                abilityType: typeof(Abilities.SecondEdition.SnapShotAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0c/6b/0c6b9e6c-7c2f-4322-bcf0-f6f2fce44323/swz47_upgrade-snap-shot.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<BaseSize, int> sizeToCost = new Dictionary<BaseSize, int>()
            {
                {BaseSize.Small, 7},
                {BaseSize.Medium, 8},
                {BaseSize.Large, 9},
            };

            UpgradeInfo.Cost = sizeToCost[ship.ShipInfo.BaseSize];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SnapShotAbility : Abilities.FirstEdition.SnapShotAbility
    {
        protected override void EnableWeaponRange()
        {
            // Do nothing
        }

        protected override void DisableWeaponRange()
        {
            // Do nothing
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