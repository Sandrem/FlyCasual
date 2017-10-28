using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Tokens;

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

        public virtual bool IsShotAvailable(GenericShip targetShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            int range;
            if (!CanShootOutsideArc)
            {
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, targetShip, this);
                range = shotInfo.Range;

                if (!shotInfo.InShotAngle) return false;

                if (!shotInfo.CanShootSecondaryWeapon) return false;
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
                List<GenericToken> waysToPay = new List<GenericToken>();

                char letter = Actions.GetTargetLocksLetterPair(Host, targetShip);
                GenericToken targetLockToken = Host.GetToken(typeof(BlueTargetLockToken), letter);
                if (targetLockToken != null) waysToPay.Add(targetLockToken);

                Host.CallOnGenerateAvailableAttackPaymentList(waysToPay);

                if (waysToPay.Count == 0) return false;
            }

            if (RequiresFocusToShoot)
            {
                if (!Host.HasToken(typeof(FocusToken))) return false;
            }

            return result;
        }

        public virtual int GetAttackValue()
        {
            return AttackValue;
        }

        public virtual void PayAttackCost(Action callBack)
        {
            PayDiscardCost(delegate { PayTokenCost(callBack); });
        }

        private void PayDiscardCost(Action callBack)
        {
            if (IsDiscardedForShot)
                {
                    TryDiscard(callBack);
                }
            else
            {
                callBack();
            };
        }

        private void PayTokenCost(Action callBack)
        {
            if (RequiresTargetLockOnTargetToShoot)
            {
                if (SpendsTargetLockOnTargetToShoot)
                {
                    List<GenericToken> waysToPay = new List<GenericToken>();

                    char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
                    GenericToken targetLockToken = Combat.Attacker.GetToken(typeof(BlueTargetLockToken), letter);
                    if (targetLockToken != null) waysToPay.Add(targetLockToken);

                    Combat.Attacker.CallOnGenerateAvailableAttackPaymentList(waysToPay);

                    if (waysToPay.Count == 1)
                    {
                        Combat.Attacker.SpendToken(
                            waysToPay[0].GetType(),
                            callBack,
                            (waysToPay[0] as BlueTargetLockToken != null) ? (waysToPay[0] as BlueTargetLockToken).Letter : ' '
                        );
                    }
                    else
                    {
                        Phases.StartTemporarySubPhase(
                            "Choose how to pay attack cost",
                            typeof(SubPhases.PayAttackCostDecisionSubPhase),
                            callBack
                        );
                    }
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

namespace SubPhases
{

    public class PayAttackCostDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Choose how to pay attack cost";

            List<GenericToken> waysToPay = new List<GenericToken>();

            char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            GenericToken targetLockToken = Combat.Attacker.GetToken(typeof(BlueTargetLockToken), letter);
            if (targetLockToken != null) waysToPay.Add(targetLockToken);

            Combat.Attacker.CallOnGenerateAvailableAttackPaymentList(waysToPay);

            foreach (var wayToPay in waysToPay)
            {
                if (wayToPay.GetType() == typeof(BlueTargetLockToken)) { 
                    AddDecision(
                        "Target Lock token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
                if (wayToPay.GetType() == typeof(FocusToken))
                {
                    AddDecision(
                        "Focus token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
            }

            defaultDecision = GetDecisions().First().Key;
        }

        private void PayCost(GenericToken token)
        {
            Combat.Attacker.SpendToken(
                token.GetType(),
                ConfirmDecision,
                (token as BlueTargetLockToken != null) ? (token as BlueTargetLockToken).Letter : ' '
            );
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
