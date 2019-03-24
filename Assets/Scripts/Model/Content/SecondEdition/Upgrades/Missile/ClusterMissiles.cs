using BoardTools;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ClusterMissiles : GenericSpecialWeapon
    {
        public ClusterMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cluster Missiles",
                UpgradeType.Missile,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 4
                ),
                abilityType: typeof(Abilities.SecondEdition.CluseterMissilesAbility),
                seImageNumber: 37
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CluseterMissilesAbility : GenericAbility
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
            (HostUpgrade as UpgradesList.SecondEdition.ClusterMissiles).WeaponInfo.RequiresToken = null;

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " can perform a second Cluster Missiles attack.");

            Combat.StartSelectAttackTarget(
                HostShip,
                FinishAdditionalAttack,
                IsClusterMissilesShotToNeighbour,
                HostUpgrade.UpgradeInfo.Name,
                "You may perform a second Cluster Missiles attack.",
                HostUpgrade
            );
        }

        private bool IsClusterMissilesShotToNeighbour(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;
            string TargetingFailure = "The attack can be performed.";
            if (weapon == HostUpgrade)
            {
                if (defender.ShipId != OriginalDefender.ShipId)
                {
                    DistanceInfo distInfo = new DistanceInfo(OriginalDefender, defender);
                    if (distInfo.Range < 2)
                    {
                        result = true;
                    }
                    else
                    {
                        TargetingFailure = "The attack cannot be performed. The new target is further than range 2 from the attacker.";
                    }
                }
                else
                {
                    TargetingFailure = "The attack cannot be performed. You cannot attack the original target of Cluster Missiles with the second attack.";
                }
            }
            else
            {
                TargetingFailure = "The attack cannot be performed: Weapon " + HostUpgrade.UpgradeInfo.Name + " is not equipped."; 
            }

            if (result == false && !isSilent) Messages.ShowErrorToHuman(TargetingFailure);

            return result;
        }

        private void FinishAdditionalAttack()
        {
            (HostUpgrade as UpgradesList.SecondEdition.ClusterMissiles).WeaponInfo.RequiresToken = typeof(BlueTargetLockToken);

            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            Selection.ThisShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }
    }
}