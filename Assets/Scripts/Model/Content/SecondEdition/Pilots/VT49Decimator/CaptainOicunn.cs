using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class CaptainOicunn : VT49Decimator
        {
            public CaptainOicunn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Oicunn",
                    3,
                    84,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.CaptainOicunnAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 146;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainOicunnAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnCanAttackBumpedTarget += CanAttack;
            HostShip.PrimaryWeapon.MinRange = 0;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanAttackBumpedTarget -= CanAttack;
            HostShip.PrimaryWeapon.MinRange = 1;
        }

        private void CanAttack(ref bool canAttack, GenericShip attacker, GenericShip defender)
        {
            canAttack = true;
        }

    }
}
