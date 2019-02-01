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
                    78,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainOicunnAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 146
                );
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
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 0);
            HostShip.OnCanAttackBumpedTarget += CanAttack;
        }

        public override void DeactivateAbility()
        {
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 1);
            HostShip.OnCanAttackBumpedTarget -= CanAttack;
        }

        private void CanAttack(ref bool canAttack, GenericShip attacker, GenericShip defender)
        {
            canAttack = true;
        }

    }
}
