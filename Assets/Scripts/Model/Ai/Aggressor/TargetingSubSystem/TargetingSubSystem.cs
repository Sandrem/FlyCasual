using BoardTools;
using Editions;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;
using Upgrade;

namespace AI.Aggressor
{
    public class AttackDecision
    {
        public GenericShip CurrentShip { get; private set; }
        public GenericShip TargetShip { get; private set; }
        public IShipWeapon Weapon { get; private set; }
        public int Priority { get; private set; }

        public AttackDecision(GenericShip currentShip, GenericShip targetShip, IShipWeapon weapon)
        {
            CurrentShip = currentShip;
            TargetShip = targetShip;
            Weapon = weapon;

            CalculatePriority();
        }

        private void CalculatePriority()
        {
            // Local constants

            const float attackDiceChanceUnmodified = 0.5f;
            const float attackDiceChanceSingleModification = 0.75f;
            const float attackDiceChanceFullModification = 0.938f;

            const float potentialCritsNoReroll = 0.125f;
            const float potentialCritsWithReroll = 0.1875f;

            const float defenceDiceChanceUnmodified = 0.375f;
            const float defenceDiceChanceFocusModification = 0.625f;

            ShotInfo shotInfo = new ShotInfo(CurrentShip, TargetShip, Weapon);

            // Attack dice

            float attackDiceThrown = Weapon.WeaponInfo.AttackValue;
            if (shotInfo.Range <= 1 && Edition.Current.IsWeaponHaveRangeBonus(Weapon)) attackDiceThrown++;

            float attackDiceModifier = 0;
            float criticalHitsModifier = potentialCritsNoReroll;
            if (CurrentShip.Tokens.HasToken<FocusToken>() && ActionsHolder.HasTargetLockOn(CurrentShip, TargetShip))
            {
                attackDiceModifier = attackDiceChanceFullModification;
            }
            else if (CurrentShip.Tokens.HasToken<FocusToken>() || ActionsHolder.HasTargetLockOn(CurrentShip, TargetShip))
            {
                attackDiceModifier = attackDiceChanceSingleModification;
                if (ActionsHolder.HasTargetLockOn(CurrentShip, TargetShip)) criticalHitsModifier = potentialCritsWithReroll;
            }
            else
            {
                attackDiceModifier = attackDiceChanceUnmodified;
            }

            float potentialHits = attackDiceThrown * attackDiceModifier;

            // Defence dice

            float defenceDiceThrown = TargetShip.State.Agility;
            if (shotInfo.Range == 3 && !Edition.Current.IsWeaponHaveRangeBonus(Weapon)) defenceDiceThrown++;
            if (shotInfo.IsObstructedByAsteroid) defenceDiceThrown++;

            float defenceDiceModifier = 0;
            if (TargetShip.Tokens.HasToken<FocusToken>())
            {
                defenceDiceModifier = defenceDiceChanceFocusModification;
            }
            else
            {
                defenceDiceModifier = defenceDiceChanceUnmodified;
            }

            float potentialEvades = defenceDiceThrown * defenceDiceModifier;
            if (TargetShip.Tokens.HasToken<EvadeToken>() && defenceDiceThrown > 0)
            {
                potentialEvades = Math.Min(1, potentialEvades);
            }

            // Results

            float potentialDamage = potentialHits - potentialEvades;
            float potentialCrits = attackDiceThrown * criticalHitsModifier;
            float shipCost = TargetShip.PilotInfo.Cost;
            IShipWeapon currentWeapon;
            GenericUpgrade currentUpgrade = null;

            // Find the upgrade that matches our current weapon.
            foreach (GenericUpgrade upgrade in Selection.ThisShip.UpgradeBar.GetSpecialWeaponsActive())
            {
                if (upgrade is GenericSpecialWeapon)
                {
                    currentWeapon = (upgrade as IShipWeapon);
                    if (currentWeapon.Name == Weapon.Name)
                    {
                        currentUpgrade = upgrade;
                        break;
                    }
                }
            }

            // If our current weapon uses charges and has no charges available, don't use it.
            // Without this, the AI will keep firing munitions-based weapons that have no charges left.
            if (currentUpgrade != null && Weapon.WeaponInfo.UsesCharges == true && currentUpgrade.State.Charges == 0)
            {
                Priority = 0;
            }
            else
            {
                Priority = (int)(potentialDamage * 1000f + potentialCrits * 100f + shipCost);
            }
        }
    }

    public static class TargetingSubSystem
    {
        public static GenericShip CurrentShip { get; private set; }

        public static List<AttackDecision> AttackDecisions;

        public static GenericShip SelectTargetAndWeapon(GenericShip ship)
        {
            CurrentShip = ship;
            AttackDecisions = new List<AttackDecision>();

            foreach (GenericShip enemyShip in GetEnemyShipsAndDistance(CurrentShip, inArcAndRange: true))
            {
                Selection.AnotherShip = enemyShip;
                foreach (IShipWeapon weapon in CurrentShip.GetAllWeapons())
                {
                    if (Rules.TargetIsLegalForShot.IsLegal(CurrentShip, enemyShip, weapon, isSilent: true))
                    {
                        AttackDecisions.Add(new AttackDecision(CurrentShip, enemyShip, weapon));
                    }
                }
            }

            AttackDecision BestAttackDecision = AttackDecisions.OrderByDescending(n => n.Priority).FirstOrDefault();
            if (BestAttackDecision != null)
            {
                Combat.ChosenWeapon = BestAttackDecision.Weapon;
                Combat.ShotInfo = new ShotInfo(CurrentShip, BestAttackDecision.TargetShip, BestAttackDecision.Weapon);

                return BestAttackDecision.TargetShip;
            }
            else
            {
                return null;
            }
        }

        private static List<GenericShip> GetEnemyShipsAndDistance(GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Dictionary<GenericShip, float> results = new Dictionary<GenericShip, float>();

            foreach (GenericShip enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships.Values)
            {
                if (!enemyShip.IsDestroyed)
                {
                    if (ignoreCollided)
                    {
                        if (thisShip.LastShipCollision != null)
                        {
                            if (thisShip.LastShipCollision.ShipId == enemyShip.ShipId)
                            {
                                continue;
                            }
                        }
                        if (enemyShip.LastShipCollision != null)
                        {
                            if (enemyShip.LastShipCollision.ShipId == thisShip.ShipId)
                            {
                                continue;
                            }
                        }
                    }

                    if (inArcAndRange)
                    {
                        DistanceInfo distanceInfo = new DistanceInfo(thisShip, enemyShip);
                        if ((distanceInfo.Range > 3))
                        {
                            continue;
                        }
                    }

                    float distance = Vector3.Distance(thisShip.GetCenter(), enemyShip.GetCenter());
                    results.Add(enemyShip, distance);
                }
            }
            results = results.OrderBy(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            return results.Select(n => n.Key).ToList();
        }
    }
}
