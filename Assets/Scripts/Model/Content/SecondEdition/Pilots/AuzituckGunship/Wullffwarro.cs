using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;

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
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.WullffwarroAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 31;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class WullffwarroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckWullffwarroAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckWullffwarroAbility;
        }

        private void CheckWullffwarroAbility(ref int value)
        {
            if ((HostShip.State.ShieldsCurrent == 0) && (HostShip.State.HullCurrent < HostShip.State.HullMax)) value++;
        }
    }
}
