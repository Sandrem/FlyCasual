using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arcs;
using BoardTools;
using Ship;
using Upgrade;

namespace ActionsList
{

    public class RotateArcAction : GenericAction
    {
        Dictionary<ArcFacing, List<GenericShip>> EnemiesInArcHolder;

        public RotateArcAction()
        {
            Name = DiceModificationName = "Rotate Arc";
        }

        public override void ActionTake()
        {
            var subphase = Phases.StartTemporarySubPhaseNew<SubPhases.RotateArcDecisionSubPhase>(
                "Rotate Arc decision",
                Phases.CurrentSubPhase.CallBack
            );
            subphase.EnemiesInArcHolder = HostShip.SectorsInfo.GetEnemiesInAllSectors();
            subphase.Start();

        }

        public override int GetActionPriority()
        {
            bool hasVeteranTurretGunner = false;

            // Check if this ship has Veteran Turret Gunner equipped.
            foreach (GenericUpgrade potentialTurretGunner in Selection.ThisShip.UpgradeBar.GetUpgradesAll())
            {
                if (potentialTurretGunner.NameCanonical == "veteranturretgunner")
                {
                    hasVeteranTurretGunner = true;
                    break;
                }
            }
            if (hasVeteranTurretGunner == true)
            {
                // Do a full weapons check to see if our turret weapons have targets.  If they don't, we'll want to rotate the turret so we can use Veteran Turret Gunner.
                int numTargetsInTurret = 0;
                foreach (IShipWeapon currentWeapon in Selection.ThisShip.GetAllWeapons())
                {
                    if (currentWeapon.WeaponType == WeaponTypes.Turret)
                    {
                        foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(Selection.ThisShip.Owner.PlayerNo)).Ships)
                        {
                            ShotInfo shotInfo = new ShotInfo(Selection.ThisShip, anotherShip.Value, currentWeapon);
                            if ((shotInfo.Range <= currentWeapon.WeaponInfo.MaxRange) && shotInfo.Range >= currentWeapon.WeaponInfo.MinRange && (shotInfo.IsShotAvailable))
                            {
                                // We found at least one target for the turret.
                                numTargetsInTurret++;
                                break;
                            }
                        }
                    }
                    if (numTargetsInTurret > 0)
                    {
                        // We only need one target in the turret arc to pass this test.
                        break;
                    }

                }
                if (numTargetsInTurret == 0)
                {
                    // We have Veteran Turret Gunner, but our turret(s) don't have a target.  Rotate a turret.
                    return 100;
                }
            }
            else if (ActionsHolder.HasTarget(Selection.ThisShip)) return 0;
            else
            {
                EnemiesInArcHolder = HostShip.SectorsInfo.GetEnemiesInAllSectors();
                foreach (ArcFacing arcFacing in HostShip.GetAvailableArcFacings())
                {
                    if (EnemiesInArcHolder[arcFacing].Count > 0)
                    {
                        // We found at least one target in one of our arcs, and we don't have someone in arc already.
                        return 100;
                    }
                }
            }

            return 1;
        }
    }

}

namespace SubPhases
{
    public class RotateArcDecisionSubPhase : DecisionSubPhase
    {
        public Dictionary<ArcFacing, List<GenericShip>> EnemiesInArcHolder = new Dictionary<ArcFacing, List<GenericShip>>();

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Rotate Arc";

            if (Selection.ThisShip.ArcsInfo.Arcs.Any(a => a is ArcSingleTurret))
            {
                foreach (ArcFacing facing in Selection.ThisShip.GetAvailableArcFacings())
                {
                    AddSingleTurretRotationDecision(facing);
                }
            }
            else if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.DoubleTurret))
            {
                AddDecision("Front-Rear", delegate { ChangeMobileDualArcFacing(ArcFacing.Front); });
                AddDecision("Left-Right", delegate { ChangeMobileDualArcFacing(ArcFacing.Left); });
            }

            DefaultDecisionName = GetDefaultDecision();

            callBack();
        }

        private void AddSingleTurretRotationDecision(ArcFacing facing)
        {
            AddDecision(
                facing.ToString(), 
                delegate {
                    ChangeMobileArcFacing(facing);
                },
                isCentered: (facing == ArcFacing.Front || facing == ArcFacing.Rear)
            );
        }

        private string GetDefaultDecision()
        {
            string facing = "";

            if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.SingleTurret))
            {
                if (EnemiesInArcHolder.Any(n => n.Value.Count > 0))
                {
                    int maxCount = 0;
                    ArcFacing maxFacing = Selection.ThisShip.ArcsInfo.GetArc<ArcSingleTurret>().Facing;

                    List<ArcFacing> availableArcFacings = Selection.ThisShip.GetAvailableArcFacings();
                    foreach (var sectorInfo in EnemiesInArcHolder)
                    {
                        if (sectorInfo.Value.Count > maxCount && availableArcFacings.Contains(sectorInfo.Key))
                        {
                            maxCount = sectorInfo.Value.Count;
                            maxFacing = sectorInfo.Key;
                        }
                    }
                    facing = maxFacing.ToString();
                }
                else
                {
                    if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.Front))
                    {
                        facing = "Rear";
                    }
                    else
                    {
                        facing = "Front";
                    }
                }
            }
            else if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.DoubleTurret))
            {
                if (EnemiesInArcHolder.Any(n => n.Value.Count > 0))
                {
                    int enemiesInFrontRearArc = 0;
                    int enemiesInLeftRearArc = 0;
                    if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.Front))
                    {
                        enemiesInFrontRearArc = EnemiesInArcHolder
                            .Where(a => a.Key == ArcFacing.Front || a.Key == ArcFacing.Rear)
                            .Sum(a => a.Value.Count);
                        enemiesInLeftRearArc = EnemiesInArcHolder
                            .Where(a => a.Key == ArcFacing.Front || a.Key == ArcFacing.Left || a.Key == ArcFacing.Right)
                            .Sum(a => a.Value.Count);
                    }
                    else
                    {
                        enemiesInFrontRearArc = EnemiesInArcHolder
                            .Where(a => a.Key == ArcFacing.Front || a.Key == ArcFacing.Rear)
                            .Sum(a => a.Value.Count);
                        enemiesInLeftRearArc = EnemiesInArcHolder
                            .Where(a => a.Key == ArcFacing.Left || a.Key == ArcFacing.Right)
                            .Sum(a => a.Value.Count);
                    }

                    facing = (enemiesInFrontRearArc >= enemiesInLeftRearArc) ? "Front-Rear" : "Left-Right";
                }
                else
                {
                    if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.Front))
                    {
                        facing = "Left-Right";
                    }
                    else
                    {
                        facing = "Front-Rear";
                    }
                }
            }

            return facing;
        }

        private void ChangeMobileArcFacing(ArcFacing facing)
        {
            Selection.ThisShip.ArcsInfo.GetArc<ArcSingleTurret>().RotateArc(facing);
            ConfirmDecision();
        }

        private void ChangeMobileDualArcFacing(ArcFacing facing)
        {
            Selection.ThisShip.ArcsInfo.GetArc<ArcDualTurretA>().RotateArc(facing);
            ConfirmDecision();
        }

    }

}
