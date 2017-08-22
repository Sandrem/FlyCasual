using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class GenericSecondaryWeapon : GenericUpgrade
    {
        public int MinRange;
        public int MaxRange;
        public int AttackValue;

        public bool RequiresFocusToShoot;
        public bool RequiresTargetLockOnTargetToShoot;
        public bool SpendsTargetLockOnTargetToShoot;

        public bool IsDiscardedForShot;

        public GenericSecondaryWeapon() : base()
        {

        }

        public virtual bool IsShotAvailable(Ship.GenericShip anotherShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, anotherShip);

            int range = shotInfo.Range;
            if (range < MinRange) return false;
            if (range > MaxRange) return false;

            if (!shotInfo.InArc) return false;

            if (RequiresTargetLockOnTargetToShoot)
            {
                if (!Actions.HasTargetLockOn(Host, anotherShip)) return false;
            }

            if (RequiresFocusToShoot)
            {
                if (!Host.HasToken(typeof(Tokens.FocusToken))) return false;
            }

            return result;
        }

        public virtual int GetAttackValue()
        {
            return AttackValue;
        }

        public virtual void PayAttackCost(Action callBack)
        {
            char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);

            if (IsDiscardedForShot) Discard();

            if (SpendsTargetLockOnTargetToShoot)
            {
                Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);
                Combat.Attacker.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, letter);
            }
        }

    }

}
