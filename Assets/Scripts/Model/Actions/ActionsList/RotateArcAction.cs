using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Arcs;
using BoardTools;
using Ship;

namespace ActionsList
{

    public class RotateArcAction : GenericAction
    {
        private static List<ArcFacing> ArcFacings = new List<ArcFacing>() { ArcFacing.Forward, ArcFacing.Left, ArcFacing.Right, ArcFacing.Rear };
        private static Dictionary<ArcFacing, ArcFacing> DualPointerArcs = new Dictionary<ArcFacing, ArcFacing>()
        {
            { ArcFacing.Forward, ArcFacing.Rear},
            { ArcFacing.Left, ArcFacing.Right}
        };
        private Dictionary<ArcFacing, List<GenericShip>> EnemiesInArcHolder = new Dictionary<ArcFacing, List<GenericShip>>();

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
            subphase.EnemiesInArcHolder = EnemiesInArcHolder;
            subphase.Start();

        }

        public override int GetActionPriority()
        {
            if (ActionsHolder.HasTarget(Selection.ThisShip)) return 0;

            GetShipsInAllArcDirections();
            foreach (var arcInfo in EnemiesInArcHolder)
            {
                if (arcInfo.Value.Count > 0) return 100;
            }

            return 1;
        }

        private void GetShipsInAllArcDirections()
        {
            ArcMobile temporaryArc = new ArcMobile(Selection.ThisShip.ShipBase);
            Selection.ThisShip.ArcsInfo.Arcs.Add(temporaryArc);

            EnemiesInArcHolder = new Dictionary<ArcFacing, List<GenericShip>>();

            foreach (var arc in ArcFacings)
            {
                EnemiesInArcHolder.Add(arc, new List<GenericShip>());

                foreach (GenericShip enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(Selection.ThisShip.Owner.PlayerNo)).Ships.Values)
                {
                    temporaryArc.Facing = arc;

                    if (Selection.ThisShip.ShipsBumped.Contains(enemyShip)) continue;
                    if (Selection.ThisShip.ShipBaseArcsType == BaseArcsType.ArcMobileDual && (arc == ArcFacing.Right || arc == ArcFacing.Rear)) continue;

                    ShotInfoArc arcShotInfo = new ShotInfoArc(Selection.ThisShip, enemyShip, temporaryArc);
                    if (arcShotInfo.InArc && arcShotInfo.Range < 4)
                    {
                        EnemiesInArcHolder[arc].Add(enemyShip);
                    }
                    else if (Selection.ThisShip.ShipBaseArcsType == BaseArcsType.ArcMobile) // With primary firing arc
                    {
                        ShotInfoArc arcPrimaryShotInfo = new ShotInfoArc(Selection.ThisShip, enemyShip, Selection.ThisShip.ArcsInfo.GetArc<ArcPrimary>());
                        if (arcPrimaryShotInfo.InArc && arcPrimaryShotInfo.Range < 4)
                        {
                            EnemiesInArcHolder[arc].Add(enemyShip);
                        }
                    }
                    else if (Selection.ThisShip.ShipBaseArcsType == BaseArcsType.ArcMobileDual)
                    {
                        temporaryArc.Facing = DualPointerArcs[arc];
                        ShotInfoArc secondArcShotInfo = new ShotInfoArc(Selection.ThisShip, enemyShip, temporaryArc);
                        if (secondArcShotInfo.InArc && secondArcShotInfo.Range < 4)
                        {
                            EnemiesInArcHolder[arc].Add(enemyShip);
                        }
                    }
                }
            }

            Selection.ThisShip.ArcsInfo.Arcs.Remove(temporaryArc);
        }

    }

}

namespace SubPhases
{

    public class RotateArcDecisionSubPhase : DecisionSubPhase
    {
        private static Dictionary<BaseArcsType, ArcFacing> DefaultArcPositions = new Dictionary<BaseArcsType, ArcFacing>()
        {
            {BaseArcsType.ArcMobile,        ArcFacing.Rear },
            {BaseArcsType.ArcMobileDual,    ArcFacing.Forward },
            {BaseArcsType.ArcMobileOnly,    ArcFacing.Forward },
            {BaseArcsType.ArcMobileTurret,  ArcFacing.Rear },
        };

        private static Dictionary<ArcFacing, string> ArcFacingToString = new Dictionary<ArcFacing, string>()
        {
            { ArcFacing.Forward,    "Front"},
            { ArcFacing.Left,       "Left"},
            { ArcFacing.Right,      "Right" },
            { ArcFacing.Rear,       "Rear" }
        };

        public Dictionary<ArcFacing, List<GenericShip>> EnemiesInArcHolder = new Dictionary<ArcFacing, List<GenericShip>>();

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Rotate Arc";

            if (Selection.ThisShip.ArcsInfo.Arcs.Any(a => a is ArcMobile))
            {
                AddDecision("Front", delegate { ChangeMobileArcFacing(ArcFacing.Forward); }, isCentered: true);
                AddDecision("Left", delegate { ChangeMobileArcFacing(ArcFacing.Left); });
                AddDecision("Right", delegate { ChangeMobileArcFacing(ArcFacing.Right); });
                AddDecision("Rear", delegate { ChangeMobileArcFacing(ArcFacing.Rear); }, isCentered: true);
            }
            else if (Selection.ThisShip.ArcsInfo.Arcs.Any(a => a is ArcMobileDualA))
            {
                AddDecision("Front-Rear", delegate { ChangeMobileDualArcFacing(ArcFacing.Forward); });
                AddDecision("Left-Right", delegate { ChangeMobileDualArcFacing(ArcFacing.Left); });
            }

            DefaultDecisionName = GetDefaultDecision();

            callBack();
        }

        //TODO: Update for AI
        private string GetDefaultDecision()
        {
            //Initial position
            if (EnemiesInArcHolder.Count == 0 || EnemiesInArcHolder.First(n => n.Value.Count == EnemiesInArcHolder.Max(a => a.Value.Count)).Value.Count == 0)
            {
                string defaultPosition = ArcFacingToString[DefaultArcPositions[Selection.ThisShip.ShipBaseArcsType]];
                if (Selection.ThisShip.ShipBaseArcsType == BaseArcsType.ArcMobileDual)
                {
                    defaultPosition = (defaultPosition == "Front") ? "Front-Rear" : "Left-Right";
                }
                return defaultPosition;
            }

            //Regular action
            string facing = ArcFacingToString[EnemiesInArcHolder.First(n => n.Value.Count == EnemiesInArcHolder.Max(a => a.Value.Count)).Key];
            if (Selection.ThisShip.ShipBaseArcsType == BaseArcsType.ArcMobileDual)
            {
                facing = (facing == "Front") ? "Front-Rear" : "Left-Right";
            }
            return facing;
        }

        private void ChangeMobileArcFacing(ArcFacing facing)
        {
            Selection.ThisShip.ArcsInfo.GetArc<ArcMobile>().RotateArc(facing);
            ConfirmDecision();
        }

        private void ChangeMobileDualArcFacing(ArcFacing facing)
        {
            Selection.ThisShip.ArcsInfo.GetArc<ArcMobileDualA>().RotateArc(facing);
            ConfirmDecision();
        }

    }

}
