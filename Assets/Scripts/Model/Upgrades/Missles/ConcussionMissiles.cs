using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ConcussionMissiles : GenericSecondaryWeapon
    {
        public ConcussionMissiles() : base()
        {
            IsHidden = true;

            Type = UpgradeSlot.Missiles;

            Name = "Concussion Missiles";
            ShortName = "Concussion Missiles";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/7/70/Concussion_Missiles.png";
            Cost = 4;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            ActionsList.ClusterMissilesAction action = new ActionsList.ClusterMissilesAction();
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

        public override void PayAttackCost()
        {
            char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            Combat.Attacker.SpendToken(typeof(Tokens.BlueTargetLockToken), letter);
            Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);

            Discard();
        }

    }

}

namespace ActionsList
{ 

    public class ConcussionMissiles : GenericAction
    {
        public Ship.GenericShip Host;

        public ConcussionMissiles()
        {
            Name = EffectName = "Concussion Missiles";

            //AddDiceModification (host requied first);
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
                int attackBlanks = Combat.DiceRollAttack.Blanks;
                if (attackBlanks > 0)
                {
                    if ((attackBlanks == 1) && (!Combat.Attacker.HasToken(typeof(Tokens.FocusToken))))
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
