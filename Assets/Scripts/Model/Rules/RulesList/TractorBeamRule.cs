using Ship;
using Tokens;
using UnityEngine;
using BoardTools;
using GameModes;
using System;
using Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Editions;
using ActionsList;

namespace RulesList
{
    public class TractorBeamRule
    {
        static bool RuleIsInitialized = false;

        public TractorBeamRule ()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnTokenIsAssignedGlobal += CheckForTractorBeam;
                GenericShip.OnTokenIsRemovedGlobal += CheckForTractorBeamRemoval;
                RuleIsInitialized = true;
            }
        }

        private void CheckForTractorBeam(GenericShip ship, Type tokenType)
        {
            if (tokenType != typeof(TractorBeamToken)) 
            {
                return;
            }

            if (ShouldDecreaseAgility(ship)) ship.ChangeAgilityBy(-1);

            if (IsTractorBeamReposition(ship))
            {
                TractorBeamToken token = (TractorBeamToken)ship.Tokens.GetToken(typeof(TractorBeamToken));
                token.Assigner.PerformTractorBeamReposition(ship);
            }
        }

        public static bool IsTractorBeamReposition(GenericShip ship)
        {
            int tractorBeamTokensCount = ship.Tokens.GetAllTokens().Count(n => n is TractorBeamToken);
            return (tractorBeamTokensCount == Edition.Current.NegativeTokensToAffectShip[ship.ShipInfo.BaseSize]);
        }

        private bool ShouldDecreaseAgility(GenericShip ship)
        {
            bool result = true;

            if (Edition.Current is SecondEdition)
            {
                int tractorBeamTokensCount = ship.Tokens.CountTokensByType(typeof(TractorBeamToken));
                if (tractorBeamTokensCount > 1) result = false;
            }

            return result;
        }

        public static void PerfromManualTractorBeamReposition(GenericShip ship, GenericPlayer assinger)
        {
            SubPhases.TractorBeamPlanningSubPhase newPhase = (SubPhases.TractorBeamPlanningSubPhase)Phases.StartTemporarySubPhaseNew(
                "Perform tractor beam effect",
                typeof(SubPhases.TractorBeamPlanningSubPhase),
                Triggers.FinishTrigger
            );
            newPhase.Assigner = assinger;
            newPhase.TheShip = ship;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Perform tractor beam",
                TriggerType = TriggerTypes.OnTokenIsAssigned,
                TriggerOwner = assinger.PlayerNo,
                EventHandler = delegate {
                    newPhase.Start();
                }
            });
        }

        private void CheckForTractorBeamRemoval(GenericShip ship, Type tokenType)
        {
            if (tokenType != typeof(TractorBeamToken)) 
            {
                return;
            }

            if (ShouldIncreaseAgility(ship)) ship.ChangeAgilityBy(+1);
        }

        private bool ShouldIncreaseAgility(GenericShip ship)
        {
            bool result = true;

            if (Edition.Current is SecondEdition)
            {
                int tractorBeamTokensCount = ship.Tokens.CountTokensByType(typeof(TractorBeamToken));
                if (tractorBeamTokensCount > 0) result = false;
            }

            return result;
        }
    }
}

namespace SubPhases
{
    public class TractorBeamPlanningSubPhase : GenericSubPhase
    {
        public GenericPlayer Assigner;
        private Action selectedPlanningAction;
        private bool canBoost = true;

        public override void Start()
        {
            Name = "Tractor Beam planning";
            IsTemporary = true;
            UpdateHelpInfo();

            CheckCanBoost();
        }

        private void InitializeBostPlanning(BoostPlanningSubPhase boostPlanning)
        {
            boostPlanning.TheShip = TheShip;
            boostPlanning.Name = "Tractor beam boost";
            boostPlanning.IsTemporary = true;
            boostPlanning.SelectedBoostHelper = "Straight 1";
            boostPlanning.IsTractorBeamBoost = true;
            boostPlanning.InitializeRendering();
        }

        private void CheckCanBoost()
        {
            BoostPlanningSubPhase boostPlanning = new BoostPlanningSubPhase ();
            InitializeBostPlanning(boostPlanning);
            boostPlanning.TryConfirmBoostPosition(CheckCanBoostCallback);
        }

        private void CheckCanBoostCallback(bool canBoostResult)
        {
            this.canBoost = canBoostResult;
            RegisterTractorPlanning();
        }

        public void RegisterTractorPlanning()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select tractor beam direction",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Assigner.PlayerNo,
                EventHandler = delegate {
                    StartSelectTemplateSubphase();
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ExecutePlanning);
        }

        private void ExecutePlanning()
        {
            if (selectedPlanningAction != null)
            {
                selectedPlanningAction();
            }
            else
            {
                Next();
            }
        }

        private void PerfromBrTemplatePlanning(Direction direction)
        {
            BarrelRollAction stubAction = new BarrelRollAction{ HostShip = TheShip };

            BarrelRollPlanningSubPhase brPlanning = (BarrelRollPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Select position",
                typeof(BarrelRollPlanningSubPhase),
                delegate {
                    FinishTractorBeamMovement();
                }
            );
            brPlanning.Name = "Select position";
            brPlanning.TheShip = TheShip;
            brPlanning.IsTemporary = true;
            brPlanning.Controller = Assigner;
            brPlanning.HostAction = stubAction;

            brPlanning.IsTractorBeamBarrelRoll = true;
            brPlanning.SelectTemplate(
                new ManeuverTemplate(
                    Movement.ManeuverBearing.Straight,
                    Movement.ManeuverDirection.Forward,
                    Movement.ManeuverSpeed.Speed1,
                    isSideTemplate: TheShip.ShipInfo.BaseSize != BaseSize.Small
                ),
                direction
            );

            Phases.UpdateHelpInfo();
            brPlanning.PerfromTemplatePlanning();
        }

        private void PerfromLeftBrTemplatePlanning()
        {
            PerfromBrTemplatePlanning(Direction.Left);
        }

        private void PerfromRightBrTemplatePlanning()
        {
            PerfromBrTemplatePlanning(Direction.Right);
        }

        private void PerfromStraightTemplatePlanning()
        {
            BoostAction stubAction = new BoostAction() { HostShip = TheShip };

            BoostPlanningSubPhase boostPlanning = (BoostPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Boost",
                typeof(BoostPlanningSubPhase),
                delegate {
                    FinishTractorBeamMovement();
                }
            );
            boostPlanning.HostAction = stubAction;
            InitializeBostPlanning(boostPlanning);
            Phases.UpdateHelpInfo();
            boostPlanning.TryConfirmBoostPosition();
        }

        private void StartSelectTemplateSubphase()
        {
            selectedPlanningAction = null;

            TractorBeamDirectionDecisionSubPhase selectTractorDirection = (TractorBeamDirectionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(TractorBeamDirectionDecisionSubPhase),
                Triggers.FinishTrigger
            );

            if (canBoost)
            {
                selectTractorDirection.AddDecision(
                    "Straight",
                    delegate {
                        selectedPlanningAction = PerfromStraightTemplatePlanning;
                        DecisionSubPhase.ConfirmDecision();
                    },
                    isCentered: true
                );
            }

            selectTractorDirection.AddDecision("Left", delegate {
                selectedPlanningAction = PerfromLeftBrTemplatePlanning;
                DecisionSubPhase.ConfirmDecision();
            });

            selectTractorDirection.AddDecision("Right", delegate {
                selectedPlanningAction = PerfromRightBrTemplatePlanning;
                DecisionSubPhase.ConfirmDecision();
            });

            selectTractorDirection.DescriptionShort = "Tractor beam";
            selectTractorDirection.DescriptionLong = "Select direction for " + TheShip.PilotInfo.PilotName;

            selectTractorDirection.DefaultDecisionName = selectTractorDirection.GetDecisions().First().Name;
            selectTractorDirection.RequiredPlayer = Assigner.PlayerNo;
            selectTractorDirection.ShowSkipButton = true;

            selectTractorDirection.Start();
        }

        private void FinishTractorBeamMovement()
        {
            if (Assigner == TheShip.Owner)
            {
                CheckObstacles();
                return;
            }

            var selectRotateDecision = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(Name, Triggers.FinishTrigger);

            selectRotateDecision.AddDecision("Left", delegate {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                RotateTractoredShip(Direction.Left, CheckObstacles);
            });

            selectRotateDecision.AddDecision("Right", delegate {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                RotateTractoredShip(Direction.Right, CheckObstacles);
            });

            selectRotateDecision.AddDecision("Skip", delegate {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                CheckObstacles();
            });

            selectRotateDecision.DescriptionShort = "Tractor beam";
            selectRotateDecision.DescriptionLong = "You may rotate tractored ship 90 degrees";

            selectRotateDecision.DefaultDecisionName = SelectAIRotateDecision(TheShip);
            selectRotateDecision.RequiredPlayer = TheShip.Owner.PlayerNo;
            selectRotateDecision.ShowSkipButton = true;

            selectRotateDecision.Start();
        }

        private string SelectAIRotateDecision(GenericShip ship)
        {
            var stressPriority = ship.GetAIStressPriority();

            if (!ActionsHolder.HasTarget(ship) || stressPriority >= 50)
            {
                var enemies = ship.SectorsInfo.GetEnemiesInAllSectors();
                var frontPriority = enemies[Arcs.ArcFacing.Front].Sum(s => s.PilotInfo.Cost);
                var leftPriority = enemies[Arcs.ArcFacing.Left].Sum(s => s.PilotInfo.Cost) + stressPriority;
                var rightPriority = enemies[Arcs.ArcFacing.Right].Sum(s => s.PilotInfo.Cost) + stressPriority;

                if (leftPriority > 0 && leftPriority > rightPriority && leftPriority > frontPriority)
                    return "Left";
                if (rightPriority > 0 && rightPriority > frontPriority)
                    return "Right";
            }

            return "Skip";
        }

        private void RotateTractoredShip(Direction direction, Action callback)
        {
            //We need to change Selection.ThisShip before rotating. Making sure that we always change back afterwards
            var selectedShip = Selection.ThisShip;
            Selection.ThisShip = TheShip;

            Action resetSelection = () => 
            {
                Selection.ThisShip = selectedShip;
                callback();
            };

            Action assignStress = () =>
            {
                TheShip.Tokens.AssignToken(typeof(StressToken), resetSelection);
            };
            
            if (direction == Direction.Left) TheShip.Rotate90Counterclockwise(assignStress);
            else if (direction == Direction.Right) TheShip.Rotate90Clockwise(assignStress);
            else resetSelection();
        }

        private void CheckObstacles()
        {
            Rules.AsteroidHit.CheckHits(TheShip);
            Rules.AsteroidLanded.CheckLandedOnObstacle(TheShip);
            Triggers.ResolveTriggers(TriggerTypes.OnMovementFinish, Next);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
        }

        public override void Resume()
        {
            var prevPhase = Phases.CurrentSubPhase;
            Phases.CurrentSubPhase = this;
            UpdateHelpInfo();
            // TODO: Check barrel roll problems
            /*if ((prevPhase is BarrelRollPlanningSubPhase) && (prevPhase as BarrelRollPlanningSubPhase).CheckBarrelRollProblems().Count > 0) {
                RegisterTractorPlanning();
            }*/
        }

        protected class TractorBeamDirectionDecisionSubPhase : DecisionSubPhase { }
    }
}
