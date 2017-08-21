using System;
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
            Type = UpgradeSlot.Torpedoes;

            Name = "Proton Torpedoes";
            ShortName = "Proton Torp.";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/e/eb/Proton-torpedoes.png";
            Cost = 4;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            ActionsList.ProtonTorpedoesAction action = new ActionsList.ProtonTorpedoesAction();
            action.Host = host;
            action.ImageUrl = ImageUrl;
            action.AddDiceModification();

            host.AddAvailableAction(action);
        }

        public override bool IsShotAvailable(Ship.GenericShip anotherShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, anotherShip);

            int range = shotInfo.Range;
            if (range < MinRange) return false;
            if (range > MaxRange) return false;

            if (!shotInfo.InArc) return false;

            if (!Actions.HasTargetLockOn(Host, anotherShip)) return false;

            return result;
        }

        public override void PayAttackCost(Action callBack)
        {
            char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);
            Discard();

            Combat.Attacker.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, letter);
        }

    }

}

namespace ActionsList
{ 

    public class ProtonTorpedoesAction : GenericAction
    {
        public Ship.GenericShip Host;

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

            if (Combat.SecondaryWeapon == null)
            {
                result = false;
            }
            else
            {
                if (Combat.SecondaryWeapon.Name != Name) result = false;
            }

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
            Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
            callBack();
        }

    }

}
