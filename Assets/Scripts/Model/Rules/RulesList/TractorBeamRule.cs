using Ship;
using Tokens;
using UnityEngine;
using Board;
using GameModes;
using System;
using Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RulesList
{
    public class TractorBeamRule
    {
        public TractorBeamRule ()
        {
            GenericShip.OnTokenIsAssignedGlobal += CheckForTractorBeam;
            GenericShip.OnTokenIsRemovedGlobal += CheckForTractorBeamRemoval;
        }

        private void CheckForTractorBeam(GenericShip ship, System.Type tokenType) 
        {
            if (tokenType != typeof(TractorBeamToken)) 
            {
                return;
            }

            ship.ChangeAgilityBy(-1);

            if (ship.Tokens.CountTokensByType (typeof(TractorBeamToken)) == 1 && ship.ShipBaseSize == Ship.BaseSize.Small) 
            {
                PerformTractorBeamEffect(ship);
            }
        }

        private void PerformTractorBeamEffect(GenericShip ship) 
        {
            TractorBeamToken token = (TractorBeamToken) ship.Tokens.GetToken(typeof(TractorBeamToken));
            SubPhases.TractorBeamPlanningSubPhase newPhase = (SubPhases.TractorBeamPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Perform tractor beam effect",
                typeof(SubPhases.TractorBeamPlanningSubPhase),
                delegate { }
            );
            newPhase.Assigner = token.Assigner;
            newPhase.TheShip = ship;

            Triggers.RegisterTrigger(new Trigger() {
                Name = "Perform tractor beam",
                TriggerType = TriggerTypes.OnTokenIsAssigned,
                TriggerOwner = token.Assigner.PlayerNo,
                EventHandler = delegate {
                    newPhase.Start();    
                }
            });
        }

        private void CheckForTractorBeamRemoval(GenericShip ship, System.Type tokenType) 
        {
            if (tokenType != typeof(TractorBeamToken)) 
            {
                return;
            }
            ship.ChangeAgilityBy(+1);
        }
    }
}

namespace SubPhases
{
    public class TractorBeamPlanningSubPhase : GenericSubPhase
    {
        public GenericPlayer Assigner;

        private Action selectedPlanningAction;

        public override void Start()
        {
            Name = "Tractor Beam planning";
            IsTemporary = true;
            UpdateHelpInfo();

            CheckCanBoost(RegisterTractorPlanning);
        }

        private void InitializeBostPlanning(BoostPlanningSubPhase boostPlanning)
        {
            boostPlanning.TheShip = TheShip;
            boostPlanning.Name = "Tractor beam boost";
            boostPlanning.IsTemporary = true;
            boostPlanning.SelectedBoostHelper = "Straight 1";
            boostPlanning.ObstacleOverlapAllowed = true;
            boostPlanning.InitializeRendering();
        }

        private void CheckCanBoost(System.Action<bool> canBoostCallback)
        {
            BoostPlanningSubPhase boostPlanning = new BoostPlanningSubPhase ();
            InitializeBostPlanning(boostPlanning);
            boostPlanning.TryConfirmBoostPosition(canBoostCallback);
        }

        private void RegisterTractorPlanning(bool canBoost)
        {
            StartSelectTemplateSubphase(canBoost);
        }

        private void PerfromBrTemplatePlanning(Actions.BarrelRollTemplateVariants template)
        {
            SubPhases.BarrelRollPlanningSubPhase newPhase = (SubPhases.BarrelRollPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Select position",
                typeof(SubPhases.BarrelRollPlanningSubPhase),
                delegate {
                    FinishTractorBeamMovement();
                }
            );
            newPhase.Name = "Select position";
            newPhase.TheShip = TheShip;
            newPhase.IsTemporary = true;
            newPhase.Controller = Assigner;
            newPhase.ObstacleOverlapAllowed = true;
            Phases.UpdateHelpInfo();
            newPhase.SelectTemplate(template);
            newPhase.PerfromTemplatePlanning();
        }

        private void PerfromLeftBrTemplatePlanning()
        {
            PerfromBrTemplatePlanning(Actions.BarrelRollTemplateVariants.Straight1Left);
        }

        private void PerfromRightBrTemplatePlanning()
        {
            PerfromBrTemplatePlanning(Actions.BarrelRollTemplateVariants.Straight1Right);
        }

        private void PerfromStraightTemplatePlanning()
        {
            BoostPlanningSubPhase boostPlanning = (SubPhases.BoostPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Boost",
                typeof(SubPhases.BoostPlanningSubPhase),
                delegate {
                    FinishTractorBeamMovement();
                }
            );
            InitializeBostPlanning(boostPlanning);
            Phases.UpdateHelpInfo();
            boostPlanning.TryPerformBoost();
        }

        private void StartSelectTemplateSubphase(bool canBoost)
        {
            TractorBeamDirectionDecisionSubPhase selectTractorDirection = (TractorBeamDirectionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(TractorBeamDirectionDecisionSubPhase),
                delegate { }
            );

            selectTractorDirection.AddDecision("Left", delegate {
                selectedPlanningAction = PerfromLeftBrTemplatePlanning;
                DecisionSubPhase.ConfirmDecision();
                selectedPlanningAction();
            });

            selectTractorDirection.AddDecision("Right", delegate {
                selectedPlanningAction = PerfromRightBrTemplatePlanning;
                DecisionSubPhase.ConfirmDecision();
                selectedPlanningAction();
            });

            if (canBoost)
            {
                selectTractorDirection.AddDecision("Straight", delegate {
                    selectedPlanningAction = PerfromStraightTemplatePlanning;
                    DecisionSubPhase.ConfirmDecision();
                    selectedPlanningAction();
                });
            }

            selectTractorDirection.InfoText = "Select tractor beam direction for " + TheShip.PilotName;
            selectTractorDirection.DefaultDecisionName = selectTractorDirection.GetDecisions().First().Name;
            selectTractorDirection.RequiredPlayer = Assigner.PlayerNo;

            selectTractorDirection.Start();
        }

        private void FinishTractorBeamMovement()
        {
            Rules.AsteroidHit.CheckDamage(TheShip);
            Triggers.ResolveTriggers(TriggerTypes.OnShipMovementFinish, Next);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
            UpdateHelpInfo();
        }

        protected class TractorBeamDirectionDecisionSubPhase : DecisionSubPhase { }
    }
}
