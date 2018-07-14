﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;

namespace UpgradesList
{

    public class ConcussionMissiles : GenericSecondaryWeapon
    {
        public ConcussionMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Concussion Missiles";
            Cost = 4;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;

            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new ConcussionMissilesAbility());
        }
    }
}

namespace Abilities
{
    public class ConcussionMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddConcussionMissilesDiceModification;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnGenerateDiceModifications -= AddConcussionMissilesDiceModification;
        }

        private void AddConcussionMissilesDiceModification(GenericShip host)
        {
            ConcussionMissilesAction action = new ConcussionMissilesAction()
            {
                Host = host,
                ImageUrl = HostUpgrade.ImageUrl,
                Source = HostUpgrade
            };

            host.AddAvailableDiceModification(action);
        }
    }
}

namespace ActionsList
{ 

    public class ConcussionMissilesAction : GenericAction
    {

        public ConcussionMissilesAction()
        {
            Name = DiceModificationName = "Concussion Missiles";
        }

        public void AddDiceModification()
        {
            Host.OnGenerateDiceModifications += ConcussionMissilesAddDiceModification;
        }

        private void ConcussionMissilesAddDiceModification(GenericShip ship)
        {
            ship.AddAvailableDiceModification(this);
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.ChosenWeapon != Source) result = false;

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackBlanks = Combat.DiceRollAttack.Blanks;
                if (attackBlanks > 0)
                {
                    if ((attackBlanks == 1) && (Combat.Attacker.GetAvailableDiceModifications().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
                    {
                        result = 100;
                    }
                    else
                    {
                        result = 55;
                    }
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            callBack();
        }

    }

}
