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
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MajorRhymerAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 109
                );
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

        private void CheckAbility(GenericSpecialWeapon weapon, ref int minRange, ref int maxRange)
        {
            if (weapon.UpgradeInfo.HasType(UpgradeType.Missile) || weapon.UpgradeInfo.HasType(UpgradeType.Torpedo))
            {
                if (minRange > 0) minRange--;
                if (maxRange < 3) maxRange++;
            }
        }
    }
}