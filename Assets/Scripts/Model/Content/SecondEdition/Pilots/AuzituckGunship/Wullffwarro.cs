using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AuzituckGunship
    {
        public class Wullffwarro : AuzituckGunship
        {
            public Wullffwarro() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wullffwarro",
                    4,
                    56,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WullffwarroAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 31
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WullffwarroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckWullffwarroAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckWullffwarroAbility;
        }

        private void CheckWullffwarroAbility(ref int value)
        {
            if (HostShip.Damage.IsDamaged())
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is damaged, gains +1 attack die");
                value++;
            }
        }
    }
}
