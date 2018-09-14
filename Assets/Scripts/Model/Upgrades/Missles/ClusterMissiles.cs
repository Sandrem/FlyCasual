using BoardTools;
using RuleSets;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ClusterMissiles : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public ClusterMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Cluster Missiles";
            Cost = 4;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;

            RequiresTargetLockOnTargetToShoot = true;

            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            IsTwinAttack = true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 4;
            Cost = 5;

            SpendsTargetLockOnTargetToShoot = false;
            IsDiscardedForShot = false;
            UsesCharges = true;

            IsTwinAttack = false;
            UpgradeAbilities.Add(new Abilities.SecondEdition.CluseterMissilesAbilitySE());

            SEImageNumber = 37;
        }
    }

}

namespace Abilities
{
    namespace SecondEdition
    {
        public class CluseterMissilesAbilitySE : GenericAbility
        {
            private GenericShip OriginalDefender;

            public override void ActivateAbility()
            {
                HostShip.OnAttackStartAsAttacker += SaveOriginalDefender;
                HostShip.OnAttackFinishAsAttacker += CheckClusterMissilesAbility;
                Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnAttackStartAsAttacker -= SaveOriginalDefender;
                HostShip.OnAttackFinishAsAttacker -= CheckClusterMissilesAbility;
                Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
            }

            private void SaveOriginalDefender()
            {
                OriginalDefender = Combat.Defender;
            }

            private void CheckClusterMissilesAbility(GenericShip ship)
            {
                if (Combat.ChosenWeapon == this.HostUpgrade && !IsAbilityUsed)
                {
                    IsAbilityUsed = true;

                    HostShip.OnCombatCheckExtraAttack += RegisterClusterMissilesAbility;
                }
            }

            private void RegisterClusterMissilesAbility(GenericShip ship)
            {
                HostShip.OnCombatCheckExtraAttack -= RegisterClusterMissilesAbility;

                if (AnotherTargetsPresent()) RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseClusterMissilesAbility);
            }

            private bool AnotherTargetsPresent()
            {
                bool result = false;

                foreach (var ship in OriginalDefender.Owner.Ships.Values)
                {
                    if (ship.ShipId == OriginalDefender.ShipId) continue;

                    DistanceInfo distInfo = new DistanceInfo(ship, OriginalDefender);
                    if (distInfo.Range > 1) continue;

                    ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostUpgrade as IShipWeapon);
                    if (shotInfo.IsShotAvailable) return true;
                }

                return result;
            }

            private void UseClusterMissilesAbility(object sender, System.EventArgs e)
            {
                (HostUpgrade as UpgradesList.ClusterMissiles).RequiresTargetLockOnTargetToShoot = false;

                Messages.ShowInfo(HostShip.PilotName + " can perform second Cluster Missiles attack");

                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsClusterMissilesShotToNeighbour,
                    HostUpgrade.Name,
                    "You may perform second Cluster Missiles attack.",
                    HostUpgrade.ImageUrl
                );
            }

            private bool IsClusterMissilesShotToNeighbour(GenericShip defender, IShipWeapon weapon, bool isSilent)
            {
                bool result = false;

                if (weapon == HostUpgrade && defender.ShipId != OriginalDefender.ShipId)
                {
                    DistanceInfo distInfo = new DistanceInfo(OriginalDefender, defender);
                    if (distInfo.Range < 2)
                    {
                        result = true;
                    }
                }

                if (result == false && !isSilent) Messages.ShowErrorToHuman("Attack cannot be perfromed: Wrong conditions");

                return result;
            }

            private void FinishAdditionalAttack()
            {
                (HostUpgrade as UpgradesList.ClusterMissiles).RequiresTargetLockOnTargetToShoot = true;

                // If attack is skipped, set this flag, otherwise regular attack can be performed second time
                Selection.ThisShip.IsAttackPerformed = true;

                Triggers.FinishTrigger();
            }
        }
    }
}