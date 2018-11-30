using Ship;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ProtonRockets : GenericSpecialWeapon
    {
        public ProtonRockets() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Rockets",
                UpgradeType.Missile,
                cost: 3,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 1,
                    requiresToken: typeof(FocusToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.ProtonRocketsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ProtonRocketsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckProtonRocketsAbility;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after attack dice are rolled
            HostShip.OnDefenceStartAsDefender += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned()
        {
            HostShip.OnDefenceStartAsDefender -= DeactivateAbilityPlanned;
            HostShip.AfterGotNumberOfAttackDice -= CheckProtonRocketsAbility;
        }

        private void CheckProtonRocketsAbility(ref int diceCount)
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                diceCount += Mathf.Min(Combat.Attacker.State.Agility, 3);
            }
        }
    }
}