﻿using Ship;
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

        private void PerfromBrTemplatePlanning(ActionsHolder.BarrelRollTemplateVariants template)
        {
            BarrelRollAction stubAction = new BarrelRollAction{ HostShip = TheShip };

            BarrelRollPlanningSubPhase brPlanning = (BarrelRollPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Select position",
                typeof(BarrelRollPlanningSubPhase),
                delegate {
                    FinishTractorBeamMovement(stubAction);
                }
            );
            brPlanning.Name = "Select position";
            brPlanning.TheShip = TheShip;
            brPlanning.IsTemporary = true;
            brPlanning.Controller = Assigner;
            brPlanning.HostAction = stubAction;

            brPlanning.IsTractorBeamBarrelRoll = true;
            brPlanning.SelectTemplate(template);

            Phases.UpdateHelpInfo();
            brPlanning.PerfromTemplatePlanning();
        }

        private void PerfromLeftBrTemplatePlanning()
        {
            PerfromBrTemplatePlanning(ActionsHolder.BarrelRollTemplateVariants.Straight1Left);
        }

        private void PerfromRightBrTemplatePlanning()
        {
            PerfromBrTemplatePlanning(ActionsHolder.BarrelRollTemplateVariants.Straight1Right);
        }

        private void PerfromStraightTemplatePlanning()
        {
            BoostAction stubAction = new BoostAction() { HostShip = TheShip };

            BoostPlanningSubPhase boostPlanning = (BoostPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Boost",
                typeof(BoostPlanningSubPhase),
                delegate {
                    FinishTractorBeamMovement(stubAction);
                }
            );
            boostPlanning.HostAction = stubAction;
            InitializeBostPlanning(boostPlanning);
            Phases.UpdateHelpInfo();
            boostPlanning.TryPerformBoost();
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

            selectTractorDirection.InfoText = "Select tractor beam direction for " + TheShip.PilotInfo.PilotName;
            selectTractorDirection.DefaultDecisionName = selectTractorDirection.GetDecisions().First().Name;
            selectTractorDirection.RequiredPlayer = Assigner.PlayerNo;
            selectTractorDirection.ShowSkipButton = true;

            selectTractorDirection.Start();
        }

        private void FinishTractorBeamMovement(ActionsList.GenericAction action)
        {
            TheShip.CallActionIsTaken(action, delegate {
                // ^ CallActionIsTaken to support interaction with black one, etc
                Rules.AsteroidHit.CheckDamage(TheShip);
                Triggers.ResolveTriggers(TriggerTypes.OnMovementFinish, Next);
            });
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
            if ((prevPhase is BarrelRollPlanningSubPhase) && (prevPhase as BarrelRollPlanningSubPhase).CheckBarrelRollProblems().Count > 0) {
                RegisterTractorPlanning();
            }
        }

        protected class TractorBeamDirectionDecisionSubPhase : DecisionSubPhase { }
    }
}
