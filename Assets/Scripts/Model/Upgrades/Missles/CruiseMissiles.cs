using Abilities;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class CruiseMissiles : GenericSecondaryWeapon
    {
        public CruiseMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Cruise Missiles";
            Cost = 3;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 1;

            RequiresTargetLockOnTargetToShoot = true;

            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new CruiseMissilesAbility());
        }

    }
}

namespace Abilities
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