using Upgrade;
using Ship;
using Arcs;
using System.Linq;
using ActionsList;
using Actions;
using BoardTools;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class VeteranTurretGunner : GenericUpgrade
    {
        public VeteranTurretGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Veteran Turret Gunner",
                UpgradeType.Gunner,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.VeteranTurretGunnerAbility),
                restriction: new ActionBarRestriction(typeof(RotateArcAction)),
                seImageNumber: 52
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(423, 17),
                new Vector2(150, 150)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VeteranTurretGunnerAbility : GenericAbility
    {
        // After you perform a primary attack, you may perform a bonus turret arc
        // attack using a turret arc you did not already attack from this round.

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
            HostShip.Ai.OnGetWeaponPriority += ModifyWeaponPriority;
            HostShip.Ai.OnGetActionPriority += ModifyRotateArcActionPriority;
            HostShip.Ai.OnGetRotateArcFacingPriority += ModifyRotateArcFacingPriority;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
            HostShip.Ai.OnGetWeaponPriority -= ModifyWeaponPriority;
            HostShip.Ai.OnGetActionPriority -= ModifyRotateArcActionPriority;
            HostShip.Ai.OnGetRotateArcFacingPriority -= ModifyRotateArcFacingPriority;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.ShotInfo.Weapon.WeaponType != WeaponTypes.PrimaryWeapon) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            bool availableArcsArePresent = HostShip.ArcsInfo.Arcs.Any(a => a.ArcType == ArcType.SingleTurret && !a.WasUsedForAttackThisRound);
            if (availableArcsArePresent)
            {
                HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
            else
            {
                Messages.ShowError(HostUpgrade.UpgradeInfo.Name + " does not have any valid arcs to use");
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseGunnerAbility);
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishAdditionalAttack,
                    IsUnusedTurretArcShot,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform a bonus turret arc attack using another turret arc",
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool IsUnusedTurretArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, defender, weapon);
            if (!shotInfo.ShotAvailableFromArcs.Any(a => a.ArcType == ArcType.SingleTurret && !a.WasUsedForAttackThisRound))
            {
                if (!isSilent) Messages.ShowError("Your attack must use a turret arc you have not already attacked from this round");
                return false;
            }

            if (!weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret))
            {
                if (!isSilent) Messages.ShowError("Your attack must use a turret arc");
                return false;
            }

            return true;
        }

        private void ModifyWeaponPriority(GenericShip targetShip, IShipWeapon weapon, ref int priority)
        {
            //If this is first attack, and ship can trigger VTG - priorize primary non-turret weapon
            if
            (
                !HostShip.IsAttackPerformed
                && weapon.WeaponType == WeaponTypes.PrimaryWeapon
                && !weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret)
                && CanAttackTargetWithPrimaryWeapon(targetShip)
                && CanAttackTargetWithTurret(targetShip)
            )
            {
                priority += 2000;
            }
        }

        private bool CanAttackTargetWithTurret(GenericShip targetShip)
        {
            foreach (GenericUpgrade turretUpgrade in Selection.ThisShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                IShipWeapon turretWeapon = turretUpgrade as IShipWeapon;
                if (turretWeapon.WeaponType == WeaponTypes.Turret)
                {
                    ShotInfo turretShot = new ShotInfo(HostShip, targetShip, turretWeapon);
                    if (turretShot.IsShotAvailable)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CanAttackTargetWithPrimaryWeapon(GenericShip targetShip)
        {
            //AI tries to check non-turret weapon first
            IShipWeapon weapon = HostShip.PrimaryWeapons.FirstOrDefault(w => !w.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret));
            if (weapon == null) weapon = HostShip.PrimaryWeapons.First();
            ShotInfo primaryShot = new ShotInfo(HostShip, targetShip, weapon);
            return primaryShot.IsShotAvailable;
        }

        private void ModifyRotateArcActionPriority(GenericAction action, ref int priority)
        {
            if (action is RotateArcAction)
            {
                // Rotate arc if ship has fixed arcs, has a target in it, but doesn't have a turret pointer in that sector
                List<GenericArc> fixedArcs = HostShip.ArcsInfo.Arcs
                                                            .Where(a => (!(a is ArcSingleTurret || a is OutOfArc)))
                                                            .ToList();

                if (fixedArcs.Count > 0 && HasTargetForWeapons(fixedArcs))
                {
                    List<ArcSingleTurret> singleTurretArcs = HostShip.ArcsInfo.Arcs
                                                            .Where(a => a is ArcSingleTurret)
                                                            .Select(a => a as ArcSingleTurret)
                                                            .ToList();

                    if (!singleTurretArcs.Any(sta => fixedArcs.Any(fa => fa.Facing == sta.Facing)))
                    {
                        priority += 100;
                    }
                }
            }
        }

        private bool HasTargetForWeapons(List<GenericArc> arcs)
        {
            foreach (GenericArc arc in arcs)
            {
                foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
                {
                    ShotInfoArc shotInfoArc = new ShotInfoArc(HostShip, enemyShip, arc);
                    if (shotInfoArc.IsShotAvailable) return true;
                }
            }

            return false;
        }

        private void ModifyRotateArcFacingPriority(ArcFacing facing, ref int priority)
        {
            if (facing == ArcFacing.Front && NoArcInFrontSector())
            {
                priority += (IsEnemyInFrontSector()) ? 100 : 5;
            }
        }

        private bool NoArcInFrontSector()
        {
            return !HostShip.ArcsInfo.Arcs.Any(a => a.ArcType == ArcType.SingleTurret && a.Facing == ArcFacing.Front);
        }

        private bool IsEnemyInFrontSector()
        {
            return HostShip.Owner.EnemyShips.Any(e => HostShip.SectorsInfo.IsShipInSector(e.Value, ArcType.Front));
        }
    }
}