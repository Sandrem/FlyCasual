using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arcs;
using BoardTools;
using Ship;
using UnityEngine;
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
            subphase.Start();
        }

        public override int GetActionPriority()
        {
            int priority = 0;

            //If we don't have a target to shoot in current arcs, but have a target to shoot in available turret sector - rotate arc
            if (!ActionsHolder.HasTarget(Selection.ThisShip))
            {
                EnemiesInArcHolder = HostShip.SectorsInfo.GetEnemiesInAllSectors();
                foreach (ArcFacing arcFacing in HostShip.GetAvailableArcFacings())
                {
                    if (EnemiesInArcHolder[arcFacing].Count > 0)
                    {
                        priority = 100;
                    }
                }
            }

            return priority;
        }
    }

}

namespace SubPhases
{
    public class RotateArcDecisionSubPhase : DecisionSubPhase
    {
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
            string chosenFacing = "";

            Dictionary<ArcFacing, List<GenericShip>> enemiesInArcHolder = Selection.ThisShip.SectorsInfo.GetEnemiesInAllSectors();

            Dictionary<ArcFacing, int> singleTurretPriorities = new Dictionary<ArcFacing, int>();
            foreach (ArcFacing facing in Selection.ThisShip.GetAvailableArcFacings())
            {
                int priority = 0;

                //Ignore static arcs with same facing
                if (!Selection.ThisShip.ArcsInfo.Arcs.Any(a => a.Facing == facing && (!(a is ArcSingleTurret))))
                {
                    priority = enemiesInArcHolder[facing].Count;
                }
                
                Selection.ThisShip.Ai.CallGetRotateArcFacingPriority(facing, ref priority);
                singleTurretPriorities.Add(facing, priority);
            }

            // For single turret
            if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.SingleTurret))
            {
                ArcFacing currentFacing = Selection.ThisShip.ArcsInfo.GetArc<ArcSingleTurret>().Facing;

                //if no enemies in all sectors, prioritize default values
                if (!singleTurretPriorities.Any(n => n.Value > 0))
                {
                    if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.Front))
                    {
                        singleTurretPriorities[ArcFacing.Rear] += 1;
                    }
                    else
                    {
                        singleTurretPriorities[ArcFacing.Front] += 1;
                    }
                }

                //Get facing with highest priority that is not current facing
                ArcFacing bestFacing = singleTurretPriorities.FirstOrDefault(a => a.Key != currentFacing && a.Value == singleTurretPriorities.Max(b => b.Value)).Key;
                chosenFacing = bestFacing.ToString();
            }
            // For double turret
            else if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.DoubleTurret))
            {
                ArcFacing currentFacing = Selection.ThisShip.ArcsInfo.GetArc<ArcDualTurretA>().Facing;

                string currentFacingString = "None";
                if (currentFacing == ArcFacing.Front || currentFacing == ArcFacing.Rear)
                {
                    currentFacingString = "Front-Rear";
                }
                else if (currentFacing == ArcFacing.Left || currentFacing == ArcFacing.Right)
                {
                    currentFacingString = "Left-Right";
                }

                Dictionary<string, int> doubleTurretPriorities = new Dictionary<string, int>
                {
                    { "Front-Rear", singleTurretPriorities[ArcFacing.Front] + singleTurretPriorities[ArcFacing.Rear] },
                    { "Left-Right", singleTurretPriorities[ArcFacing.Left] + singleTurretPriorities[ArcFacing.Right] }
                };

                if (!doubleTurretPriorities.Any(n => n.Value > 0))
                {
                    if (Selection.ThisShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType.Front))
                    {
                        doubleTurretPriorities["Left-Right"] += 1;
                    }
                    else
                    {
                        doubleTurretPriorities["Front-Rear"] += 1;
                    }
                }

                chosenFacing = doubleTurretPriorities.FirstOrDefault(a => a.Key != currentFacingString && a.Value == doubleTurretPriorities.Max(b => b.Value)).Key;
            }

            return chosenFacing;
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
