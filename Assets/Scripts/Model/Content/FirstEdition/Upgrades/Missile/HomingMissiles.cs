using ActionsList;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class HomingMissiles : GenericSpecialWeapon
    {
        public HomingMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Homing Missiles",
                UpgradeType.Missile,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.HomingMissilesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class HomingMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckHomingMissilesAbility;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnAttackStartAsAttacker -= CheckHomingMissilesAbility;
        }

        private void CheckHomingMissilesAbility()
        {
            if ((Combat.AttackStep == CombatStep.Attack) && (Combat.Attacker == HostShip))
            {
                if (Combat.ChosenWeapon == HostUpgrade)
                {
                    ApplyHomingMissilesAbility();
                }
            }
        }

        private void ApplyHomingMissilesAbility()
        {
            Combat.Defender.OnTryAddAvailableDiceModification += UseHomingMissilesRestriction;
            Combat.Defender.Tokens.AssignCondition(typeof(Conditions.HomingMissilesCondition));

            HostShip.OnAttackFinish += RemoveHomingMissilesAbility;
        }

        private void UseHomingMissilesRestriction(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.TokensSpend.Contains(typeof(EvadeToken)))
            {
                Messages.ShowErrorToHuman("Homing Missiles: Cannot spend evade");
                canBeUsed = false;
            }
        }

        private void RemoveHomingMissilesAbility(GenericShip ship)
        {
            Combat.Defender.OnTryAddAvailableDiceModification -= UseHomingMissilesRestriction;
            Combat.Defender.Tokens.RemoveCondition(typeof(Conditions.HomingMissilesCondition));

            HostShip.OnAttackFinish -= RemoveHomingMissilesAbility;
        }
    }
}

namespace Conditions
{

    public class HomingMissilesCondition : GenericToken
    {
        public HomingMissilesCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new UpgradesList.FirstEdition.HomingMissiles().ImageUrl;
        }
    }

}