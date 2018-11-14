using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class MajorRhymer : TIESaBomber, TIE
        {
            public MajorRhymer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Major Rhymer",
                    4,
                    34,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.MajorRhymerAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 109;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorRhymerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnUpdateWeaponRange += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnUpdateWeaponRange -= CheckAbility;
        }

        private void CheckAbility(GenericSecondaryWeapon weapon, ref int minRange, ref int maxRange)
        {
            if (weapon.Types.Contains(UpgradeType.Missile) || weapon.Types.Contains(UpgradeType.Torpedo))
            {
                if (minRange > 0) minRange--;
                if (maxRange < 3) maxRange++;
            }
        }
    }
}