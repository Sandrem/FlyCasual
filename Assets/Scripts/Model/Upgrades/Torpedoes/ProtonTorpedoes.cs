﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ProtonTorpedoes : GenericSecondaryWeapon
    {
        public ProtonTorpedoes() : base()
        {
            Type = UpgradeType.Torpedo;

            Name = "Proton Torpedoes";
            Cost = 4;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;
            
            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            AddDiceModification();
        }

        private void AddDiceModification()
        {
            ActionsList.ProtonTorpedoesAction action = new ActionsList.ProtonTorpedoesAction()
            {
                Host = Host,
                ImageUrl = ImageUrl,
                Source = this
            };
            action.AddDiceModification();

            Host.AddAvailableAction(action);
        }

    }

}

namespace ActionsList
{ 

    public class ProtonTorpedoesAction : GenericAction
    {

        public ProtonTorpedoesAction()
        {
            Name = EffectName = "Proton Torpedoes";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public void AddDiceModification()
        {
            Host.AfterGenerateAvailableActionEffectsList += ProtonTorpedoesAddDiceModification;
        }

        private void ProtonTorpedoesAddDiceModification(Ship.GenericShip ship)
        {
            ship.AddAvailableActionEffect(this);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.ChosenWeapon != Source) result = false;

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0) result = 70;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

    }

}
