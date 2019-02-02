using BoardTools;
using Editions;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AI.HotAC
{
    public static class TargetForAttackSelector
    {
        private static List<GenericShip> EnemyShips;

        public static GenericShip SelectTargetAndWeapon(GenericShip attacker)
        {
            EnemyShips = GetEnemyShipsAndDistance(attacker, ignoreCollided: true, inArcAndRange: true);

            // Try to attack locked ships first
            List<GenericShip> EnemyShipsLocked = EnemyShips.Where(n => ActionsHolder.HasTargetLockOn(attacker, n)).ToList();
            foreach (GenericShip enemyShip in EnemyShipsLocked)
            {
                if (TryToDeclareTarget(attacker, enemyShip))
                {
                    return enemyShip;
                }
                else
                {
                    EnemyShips.Remove(enemyShip);
                }
            }

            //Then try to attack all ships in range
            foreach (GenericShip enemyShip in EnemyShips)
            {
                if (TryToDeclareTarget(attacker, enemyShip))
                {
                    return enemyShip;
                }
            }

            return null;
        }

        private static List<GenericShip> GetEnemyShipsAndDistance(GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Dictionary<GenericShip, float> results = new Dictionary<GenericShip, float>();

            foreach (GenericShip enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships.Values)
            {
                if (!enemyShip.IsDestroyed && !enemyShip.IsReadyToBeDestroyed)
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

        private static bool TryToDeclareTarget(GenericShip attacker, GenericShip targetShip)
        {
            Selection.AnotherShip = targetShip;

            IShipWeapon chosenWeapon = null;
            Dictionary<IShipWeapon, int> availableWeapons = new Dictionary<IShipWeapon, int>();

            foreach (IShipWeapon weapon in attacker.GetAllWeapons())
            {
                if (Rules.TargetIsLegalForShot.IsLegal(attacker, targetShip, weapon, isSilent: true))
                {
                    availableWeapons.Add(weapon, CalculatePriority(attacker, targetShip, weapon));
                }
            }

            if (availableWeapons.Count > 0)
            {
                chosenWeapon = availableWeapons.OrderByDescending(n => n.Value).Select(n => n.Key).ToList().First();
            }

            if (chosenWeapon != null)
            {
                Combat.ChosenWeapon = chosenWeapon;
                Combat.ShotInfo = new ShotInfo(attacker, Selection.AnotherShip, Combat.ChosenWeapon);
                return true;
            }

            return false;
        }

        private static int CalculatePriority(GenericShip attacker, GenericShip targetShip, IShipWeapon weapon)
        {
            int priority = 0;

            ShotInfo shotInfo = new ShotInfo(attacker, targetShip, weapon);

            priority += weapon.WeaponInfo.AttackValue * 10;

            if (Edition.Current.IsWeaponHaveRangeBonus(weapon))
            {
                if (shotInfo.Range <= 1)
                {
                    priority += 10;
                }
                else if (shotInfo.Range == 3)
                {
                    priority -= 5;
                }
            }

            return priority;
        }
    }
}
