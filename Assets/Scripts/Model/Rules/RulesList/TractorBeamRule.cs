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
            SubPhases.TractorBeamPlanningSubPhase newPhase = (SubPhases.TractorBeamPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Perform tractor beam effect",
                typeof(SubPhases.TractorBeamPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );

            TractorBeamToken token = (TractorBeamToken) ship.Tokens.GetToken(typeof(TractorBeamToken));
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

            AskToSelectTemplate();
        }

        private void AskToSelectTemplate()
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = "Select direction and template",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Assigner.PlayerNo,
                EventHandler = StartSelectTemplateSubphase
            });
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, delegate {
                // Why is this executing as soon as the selection is shown?
                if (selectedPlanningAction != null) 
                {
                    selectedPlanningAction();
                }
            });
        }

        private void PerfromBrTemplatePlanning(Actions.BarrelRollTemplateVariants template)
        {
            UI.AddTestLogEntry ("Performing br template planning");
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
            SubPhases.BoostPlanningSubPhase newPhase = (SubPhases.BoostPlanningSubPhase) Phases.StartTemporarySubPhaseNew(
                "Boost",
                typeof(SubPhases.BoostPlanningSubPhase),
                delegate {
                    FinishTractorBeamMovement();
                }
            );
            newPhase.TheShip = TheShip;
            newPhase.Name = "Tractor beam boost";
            newPhase.IsTemporary = true;
            newPhase.SelectedBoostHelper = "Straight 1";
            newPhase.ObstacleOverlapAllowed = true;
            Phases.UpdateHelpInfo();
            newPhase.InitializeRendering();
            newPhase.TryPerformBoost();
        }

        private void StartSelectTemplateSubphase(object sender, System.EventArgs e)
        {
            TractorBeamDirectionDecisionSubPhase selectBarrelRollTemplate = (TractorBeamDirectionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(TractorBeamDirectionDecisionSubPhase),
                delegate { }
            );

            selectBarrelRollTemplate.AddDecision("Left", delegate {
                selectedPlanningAction = PerfromLeftBrTemplatePlanning;
                DecisionSubPhase.ConfirmDecision();
                selectedPlanningAction(); // HACK
            });
            selectBarrelRollTemplate.AddDecision("Right", delegate { 
                selectedPlanningAction = PerfromRightBrTemplatePlanning;
                DecisionSubPhase.ConfirmDecision();
                selectedPlanningAction(); // HACK
            });
            selectBarrelRollTemplate.AddDecision("Straight", delegate { 
                selectedPlanningAction = PerfromStraightTemplatePlanning;
                DecisionSubPhase.ConfirmDecision();
                selectedPlanningAction(); // HACK
            });
            selectBarrelRollTemplate.InfoText = "Select tractor beam direction";
            selectBarrelRollTemplate.DefaultDecisionName = selectBarrelRollTemplate.GetDecisions().First().Name;
            selectBarrelRollTemplate.RequiredPlayer = Assigner.PlayerNo;

            selectBarrelRollTemplate.Start();
        }

        private void FinishTractorBeamMovement()
        {
            Rules.AsteroidHit.CheckDamage(TheShip);
            Next();
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
