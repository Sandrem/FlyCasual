using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AuzituckGunship
    {
        public class Wullffwarro : AuzituckGunship
        {
            public Wullffwarro() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wullffwarro",
                    7,
                    30,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.WullffwarroAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
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