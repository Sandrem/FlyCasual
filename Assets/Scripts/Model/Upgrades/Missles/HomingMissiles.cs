using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class HomingMissiles : GenericSecondaryWeapon
    {
        public HomingMissiles() : base()
        {
            Type = UpgradeType.Missile;

            Name = "Homing Missiles";
            Cost = 5;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;

            IsDiscardedForShot = true;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            Host.OnAttackStartAsAttacker += CheckHomingMissilesAbility;
        }

        private void CheckHomingMissilesAbility()
        {
            if ((Combat.AttackStep == CombatStep.Attack) && (Combat.Attacker == Host))
            {
                if (Combat.ChosenWeapon == this)
                {
                    ApplyHomingMissilesAbility();
                }
            }
        }

        private void ApplyHomingMissilesAbility()
        {
            Combat.Defender.OnTryAddAvailableActionEffect += UseHomingMissilesRestriction;
            Combat.Defender.AssignToken(new Conditions.HomingMissilesCondition(Combat.Defender), delegate { });

            Host.OnAttackFinish += RemoveHomingMissilesAbility;
        }

        private void UseHomingMissilesRestriction(ActionsList.GenericAction action, ref bool canBeUsed)
        {
            if (action.IsSpendEvade)
            {
                Messages.ShowErrorToHuman("Homing Missiles: Cannot spend evade");
                canBeUsed = false;
            }
        }

        private void RemoveHomingMissilesAbility(GenericShip ship)
        {
            Combat.Defender.OnTryAddAvailableActionEffect -= UseHomingMissilesRestriction;
            Combat.Defender.RemoveCondition(typeof(Conditions.HomingMissilesCondition));

            Host.OnAttackFinish -= RemoveHomingMissilesAbility;
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