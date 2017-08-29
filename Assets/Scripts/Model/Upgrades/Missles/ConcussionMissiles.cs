using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ConcussionMissiles : GenericSecondaryWeapon
    {
        public ConcussionMissiles() : base()
        {
            Type = UpgradeType.Missiles;

            Name = "Concussion Missiles";
            ShortName = "Conc. Missiles";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/7/70/Concussion_Missiles.png";
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
            ActionsList.ConcussionMissilesAction action = new ActionsList.ConcussionMissilesAction()
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

    public class ConcussionMissilesAction : GenericAction
    {

        public ConcussionMissilesAction()
        {
            Name = EffectName = "Concussion Missiles";
        }

        public void AddDiceModification()
        {
            Host.AfterGenerateAvailableActionEffectsList += ConcussionMissilesAddDiceModification;
        }

        private void ConcussionMissilesAddDiceModification(Ship.GenericShip ship)
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
                int attackBlanks = Combat.DiceRollAttack.Blanks;
                if (attackBlanks > 0)
                {
                    if ((attackBlanks == 1) && (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
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
            Combat.CurentDiceRoll.ChangeOne(DiceSide.Blank, DiceSide.Success);
            callBack();
        }

    }

}
