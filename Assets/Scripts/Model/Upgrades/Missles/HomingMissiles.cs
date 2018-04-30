using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using ActionsList;

namespace UpgradesList
{
    public class HomingMissiles : GenericSecondaryWeapon
    {
        public HomingMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Homing Missiles";
            Cost = 5;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;

            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new HomingMissilesAbility());
        }
    }
}

namespace Abilities
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
            Combat.Defender.OnTryAddAvailableActionEffect += UseHomingMissilesRestriction;
            Combat.Defender.Tokens.AssignCondition(new Conditions.HomingMissilesCondition(Combat.Defender));

            HostShip.OnAttackFinish += RemoveHomingMissilesAbility;
        }

        private void UseHomingMissilesRestriction(GenericAction action, ref bool canBeUsed)
        {
            if (action.TokensSpend.Contains(typeof(Tokens.EvadeToken)))
            {
                Messages.ShowErrorToHuman("Homing Missiles: Cannot spend evade");
                canBeUsed = false;
            }
        }

        private void RemoveHomingMissilesAbility(GenericShip ship)
        {
            Combat.Defender.OnTryAddAvailableActionEffect -= UseHomingMissilesRestriction;
            Combat.Defender.Tokens.RemoveCondition(typeof(Conditions.HomingMissilesCondition));

            HostShip.OnAttackFinish -= RemoveHomingMissilesAbility;
        }
    }
}

namespace Conditions
{

    public class HomingMissilesCondition : Tokens.GenericToken
    {
        public HomingMissilesCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new UpgradesList.HomingMissiles().ImageUrl;
        }
    }

}