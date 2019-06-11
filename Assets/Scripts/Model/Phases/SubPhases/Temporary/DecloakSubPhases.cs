using GameModes;
using Obstacles;
using Editions;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Actions;
using ActionsList;
using BoardTools;

namespace SubPhases
{

    public class DecloakDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Perform decloak?";

            DecisionOwner = Selection.ThisShip.Owner;

            AddDecision("Yes", Decloak);
            AddDecision("No", SkipDecloak);

            AddTooltip("Yes", "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/Decloak.png");

            DefaultDecisionName = "No";

            callBack();
        }

        private void Decloak(object sender, System.EventArgs e)
        {
            Phases.CurrentSubPhase.Pause();
            UI.CallHideTooltip();

            Phases.StartTemporarySubPhaseOld(
                "Decloak",
                typeof(DecloakPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

        private void SkipDecloak(object sender, System.EventArgs e)
        {
            UI.CallHideTooltip();
            CallBack();
        }

    }

}

namespace SubPhases
{
    public class DecloakPlanningSubPhase: BarrelRollPlanningSubPhase
    {
        public override void Start()
        {
            Name = "Decloak planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollPlanning();
        }

        public override void PerfromTemplatePlanning()
        {
            Edition.Current.DecloakTemplatePlanning();
        }

        protected override void GenerateListOfAvailableTemplates()
        {
            List<ManeuverTemplate> allowedTemplates = Selection.ThisShip.GetAvailableDecloakBarrelRollTemplates();

            foreach (ManeuverTemplate barrelRollTemplate in allowedTemplates)
            {
                AvailableBarrelRollTemplates.Add(barrelRollTemplate);
            }
        }

        protected override void GameModeStartRepositionExecution()
        {
            GameMode.CurrentGameMode.StartDecloakExecution(Selection.ThisShip);
        }

        protected override void GameModeCancelReposition()
        {
            GameMode.CurrentGameMode.CancelDecloak(BarrelRollProblems);
        }

        protected override void StartBarrelRollExecutionSubphase()
        {
            Pause();

            TheShip.ToggleShipStandAndPeg(false);

            BarrelRollExecutionSubPhase executionSubphase = (DecloakExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Barrel Roll execution",
                typeof(DecloakExecutionSubPhase),
                CallBack
            );

            executionSubphase.TheShip = TheShip;
            executionSubphase.TemporaryShipBase = TemporaryShipBaseFinal;
            executionSubphase.Direction = SelectedDirectionPrimary;
            executionSubphase.IsTractorBeamBarrelRoll = IsTractorBeamBarrelRoll;

            executionSubphase.Start();
        }
    }

    public class DecloakExecutionSubPhase : BarrelRollExecutionSubPhase
    {
        protected override void FinishBarrelRollAnimationPart2()
        {
            Phases.FinishSubPhase(typeof(DecloakExecutionSubPhase));
            Selection.ThisShip.Tokens.SpendToken(typeof(Tokens.CloakToken), FinishDecloakAnimationPart3);
        }

        private void FinishDecloakAnimationPart3()
        {
            Selection.ThisShip.CallDecloak(CallBack);
        }

        protected override void GameModeFinishReposition()
        {
            GameMode.CurrentGameMode.FinishDecloak();
        }
    }
}