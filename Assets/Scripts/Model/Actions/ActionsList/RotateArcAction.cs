using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arcs;
using BoardTools;
using Ship;

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
            if (ActionsHolder.HasTarget(Selection.ThisShip)) return 0;

            EnemiesInArcHolder = HostShip.SectorsInfo.GetEnemiesInAllSectors();
            foreach (var arcInfo in EnemiesInArcHolder)
            {
                if (arcInfo.Value.Count > 0) return 100;
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
                    ArcFacing maxFacing = ArcFacing.None;

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
