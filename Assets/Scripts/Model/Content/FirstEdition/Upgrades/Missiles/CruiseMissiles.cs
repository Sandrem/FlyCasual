using Ship;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class CruiseMissiles : GenericSpecialWeapon
    {
        public CruiseMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cruise Missiles",
                UpgradeType.Missile,
                cost: 3,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 1,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.CruiseMissilesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class CruiseMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckCruiseMissilesAbility;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after attack dice are rolled
            HostShip.OnDefenceStartAsDefender += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned()
        {
            HostShip.OnDefenceStartAsDefender -= DeactivateAbilityPlanned;
            HostShip.AfterGotNumberOfAttackDice -= CheckCruiseMissilesAbility;
        }

        private void CheckCruiseMissilesAbility(ref int diceCount)
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                if (Combat.Attacker.AssignedManeuver != null) diceCount += Mathf.Min(Combat.Attacker.AssignedManeuver.Speed, 4);
            }
        }
    }
}
