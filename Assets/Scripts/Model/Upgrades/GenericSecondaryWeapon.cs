﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Tokens;
using BoardTools;
using Upgrade;
using SubPhases;

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
        public bool SpendsFocusToShoot;
        public bool SpendsTargetLockOnTargetToShoot;

        public bool IsDiscardedForShot;

        public bool IsTwinAttack;

        public WeaponTypes WeaponType
        {
            get
            {
                WeaponTypes weaponType = WeaponTypes.PrimaryWeapon;

                if (Types.Contains(UpgradeType.Cannon))
                {
                    weaponType = WeaponTypes.Cannon;
                }
                else if (Types.Contains(UpgradeType.Missile))
                {
                    weaponType = WeaponTypes.Missile;
                }
                else if (Types.Contains(UpgradeType.Torpedo))
                {
                    weaponType = WeaponTypes.Torpedo;
                }
                else if (Types.Contains(UpgradeType.Turret))
                {
                    weaponType = WeaponTypes.Turret;
                }

                return weaponType;
            }
        }

        public GenericSecondaryWeapon() : base()
        {

        }

        public virtual bool IsShotAvailable(GenericShip targetShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            if (UsesCharges && Charges == 0) return false;

            int range;
            if (!CanShootOutsideArc)
            {
                ShotInfo shotInfo = new ShotInfo(Host, targetShip, this);
                range = shotInfo.Range;

                if (!shotInfo.IsShotAvailable) return false;
            }
            else
            {
                DistanceInfo distanceInfo = new DistanceInfo(Host, targetShip);
                range = distanceInfo.Range;
            }

            if (range < MinRange) return false;
            if (range > MaxRange) return false;

            if (RequiresTargetLockOnTargetToShoot)
            {
                List<GenericToken> waysToPay = new List<GenericToken>();

                char letter = Actions.GetTargetLocksLetterPair(Host, targetShip);
                GenericToken targetLockToken = Host.Tokens.GetToken(typeof(BlueTargetLockToken), letter);
                if (targetLockToken != null) waysToPay.Add(targetLockToken);

                Host.CallOnGenerateAvailableAttackPaymentList(waysToPay);

                if (waysToPay.Count == 0) return false;
            }

            if (RequiresFocusToShoot)
            {
                if (!Host.Tokens.HasToken(typeof(FocusToken))) return false;
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
            else if (UsesCharges)
            {
                SpendCharge(callBack);
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
                List<GenericToken> waysToPay = new List<GenericToken>();

                char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
                GenericToken targetLockToken = Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), letter);
                if (targetLockToken != null) waysToPay.Add(targetLockToken);

                Combat.Attacker.CallOnGenerateAvailableAttackPaymentList(waysToPay);

                if (waysToPay.Count == 1)
                {
                    if (SpendsTargetLockOnTargetToShoot || waysToPay.First() is ForceToken)
                    {
                        Combat.Attacker.Tokens.SpendToken(
                            waysToPay.First().GetType(),
                            callBack,
                            (waysToPay.First() as BlueTargetLockToken != null) ? (waysToPay.First() as BlueTargetLockToken).Letter : ' '
                        );
                    }
                    else
                    {
                        callBack();
                     }
                }
                else
                {
                    PayAttackCostDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<PayAttackCostDecisionSubPhase>(
                        "Choose how to pay attack cost",
                        callBack
                    );
                    subphase.Weapon = this;
                    subphase.Start();
                }
            }
            else if (RequiresFocusToShoot && SpendsFocusToShoot)
            {
                Combat.Attacker.Tokens.SpendToken(typeof(FocusToken), callBack);
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
        public GenericSecondaryWeapon Weapon;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Choose how to pay attack cost";

            List<GenericToken> waysToPay = new List<GenericToken>();

            char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            GenericToken targetLockToken = Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), letter);
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
                else if (wayToPay.GetType() == typeof(FocusToken))
                {
                    AddDecision(
                        "Focus token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
                else if (wayToPay.GetType() == typeof(ForceToken))
                {
                    AddDecision(
                        "Force token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void PayCost(GenericToken token)
        {
            if (Weapon.SpendsTargetLockOnTargetToShoot || token is ForceToken)
            {
                Combat.Attacker.Tokens.SpendToken(
                    token.GetType(),
                    ConfirmDecision,
                    (token as BlueTargetLockToken != null) ? (token as BlueTargetLockToken).Letter : ' '
                );
            }
            else
            {
                ConfirmDecision();
            }
        }

    }

}
