using Ship;
using Upgrade;
using UnityEngine;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class ZebOrrelios : GenericUpgrade
    {
        public ZebOrrelios() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Zeb\" Orrelios",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.ZebOrreliosCrewAbility),
                seImageNumber: 94
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(391, 9)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ZebOrreliosCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.OnUpdateWeaponRangeGlobal += AllowRange0Primaries;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnUpdateWeaponRangeGlobal -= AllowRange0Primaries;
        }

        private void AllowRange0Primaries(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target)
        {
            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon && (weapon.HostShip == HostShip || target == HostShip))
            {
                minRange = 0;
            }
        }
    }
}
