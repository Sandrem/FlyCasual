using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Upgrade
{

    public class GenericSecondaryWeapon : GenericUpgrade, IShipWeapon
    {
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public int AttackValue { get; set; }
        public bool CanShootOutsideArc { get; set; }

        public bool RequiresFocusToShoot;
        public bool RequiresTargetLockOnTargetToShoot;
        public bool SpendsTargetLockOnTargetToShoot;

        public bool IsDiscardedForShot;

        public bool IsTwinAttack;

        public GenericSecondaryWeapon() : base()
        {

        }

        public virtual bool IsShotAvailable(Ship.GenericShip targetShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            int range;
            if (!CanShootOutsideArc)
            {
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, targetShip, this);
                range = shotInfo.Range;

                if (!shotInfo.InShotAngle) return false;
            }
            else
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Host, targetShip);
                range = distanceInfo.Range;
            }

            if (range < MinRange) return false;
            if (range > MaxRange) return false;

            if (RequiresTargetLockOnTargetToShoot)
            {
                if (!Actions.HasTargetLockOn(Host, targetShip)) return false;
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
            if (IsDiscardedForShot) Discard();

            if (RequiresTargetLockOnTargetToShoot)
            {
                char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);

                if (SpendsTargetLockOnTargetToShoot)
                {
                    Combat.Attacker.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, letter);
                }
                else
                {
                    callBack();
                }
            }
            else
            {
                callBack();
            }

        }

    }

}
