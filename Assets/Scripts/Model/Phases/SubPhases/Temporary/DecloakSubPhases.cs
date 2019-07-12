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
using Movement;
using System.Collections;

namespace SubPhases
{

    public class DecloakDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(Action callBack)
        {
            DescriptionShort = "Do you want to perform decloak?";

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
    public class DecloakPlanningSubPhase : BarrelRollPlanningSubPhase
    {
        public string SelectedBoostHelper { get; private set; }
        public GameObject TemporaryShipBase { get; private set; }

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
                AvailableRepositionTemplates.Add(barrelRollTemplate);
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

        protected override void StartRepositionExecutionSubphase()
        {
            Pause();

            TheShip.ToggleShipStandAndPeg(false);

            if (SelectedDirectionPrimary == Direction.Top)
            {
                DecloakBoostExecutionSubPhase execution = (DecloakBoostExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
                    "Boost execution",
                    typeof(DecloakBoostExecutionSubPhase),
                    CallBack
                );
                execution.TheShip = TheShip;
                execution.IsTractorBeamBoost = IsTractorBeamBarrelRoll;
                execution.SelectedBoostHelper = SelectedBoostHelper;
                execution.Start();
            }
            else
            {
                BarrelRollExecutionSubPhase executionSubphase = (DecloakBarrelRollExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
                    "Barrel Roll execution",
                    typeof(DecloakBarrelRollExecutionSubPhase),
                    CallBack
                );

                executionSubphase.TheShip = TheShip;
                executionSubphase.TemporaryShipBase = TemporaryShipBaseFinal;
                executionSubphase.Direction = SelectedDirectionPrimary;
                executionSubphase.IsTractorBeamBarrelRoll = IsTractorBeamBarrelRoll;

                executionSubphase.Start();
            }
        }

        protected override void GenerateSelectTemplateDecisions(DecisionSubPhase subphase)
        {
            foreach (ManeuverTemplate template in AvailableRepositionTemplates)
            {
                if (template.Bearing == ManeuverBearing.Straight)
                {
                    subphase.AddDecision(
                        "Boost " + template.NameNoDirection,
                        (EventHandler)delegate
                        {
                            SelectTemplate(template, Direction.Top);
                            DecisionSubPhase.ConfirmDecision();
                        },
                        isCentered: true
                    );
                }
            }

            foreach (ManeuverTemplate template in AvailableRepositionTemplates)
            {
                if (template.Bearing == ManeuverBearing.Bank)
                {
                    subphase.AddDecision(
                        "Boost " + template.NameNoDirection + " " + ((template.Direction == ManeuverDirection.Left) ? "Left" : "Right"),
                        (EventHandler)delegate
                        {
                            SelectTemplate(template, Direction.Top, (template.Direction == ManeuverDirection.Left) ? Direction.Left : Direction.Right);
                            DecisionSubPhase.ConfirmDecision();
                        }
                    );
                }
            }

            base.GenerateSelectTemplateDecisions(subphase);
        }

        protected override IEnumerator CheckCollisionsOfTemporaryElements(Action callback)
        {
            if (SelectedDirectionPrimary == Direction.Top)
            {
                ShowBoosterHelper();
            }
            else
            {
                yield return base.CheckCollisionsOfTemporaryElements(callback);
            }
        }

        private void ShowBoosterHelper()
        {
            SelectedBoostHelper = SelectedTemplate.Bearing
                + " " + ((SelectedTemplate.Speed == ManeuverSpeed.Speed1) ? "1" : "2")
                + ((SelectedDirectionSecondary == Direction.None) ? "" : " " + SelectedDirectionSecondary.ToString());

            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(true);

            Transform newBase = TheShip.GetBoosterHelper().Find(SelectedBoostHelper + "/Finisher/BasePosition");

            GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            TemporaryShipBase = MonoBehaviour.Instantiate(prefab, TheShip.GetPosition(), TheShip.GetRotation(), Board.GetBoard());
            TemporaryShipBase.transform.position = new Vector3(newBase.position.x, 0, newBase.position.z);
            TemporaryShipBase.transform.rotation = newBase.rotation;

            ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate = TheShip.GetBoosterHelper().Find(SelectedBoostHelper).GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorMovementTemplate.TheShip = TheShip;

            // TODO: Check collisions

            // TODO: Check boost problems and mines

            HidePlanningTemplates();

            GameMode.CurrentGameMode.StartDecloakExecution(Selection.ThisShip);
        }

        private void HidePlanningTemplates()
        {
            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(false);
            MonoBehaviour.Destroy(TemporaryShipBase);

            Roster.SetRaycastTargets(true);
        }
    }

    public class DecloakBarrelRollExecutionSubPhase : BarrelRollExecutionSubPhase
    {
        protected override void FinishBarrelRollAnimationPart2()
        {
            Phases.FinishSubPhase(typeof(DecloakBarrelRollExecutionSubPhase));
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

    public class DecloakBoostExecutionSubPhase : BoostExecutionSubPhase
    {
        public override void FinishBoost()
        {
            GameMode.CurrentGameMode.FinishBoost();
        }

        protected override void FinishBoostAnimation()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();

            Selection.ThisShip.ToggleShipStandAndPeg(true);
            Selection.ThisShip.Tokens.SpendToken(typeof(Tokens.CloakToken), FinishDecloakAnimationPart3);
        }

        private void FinishDecloakAnimationPart3()
        {
            Selection.ThisShip.CallDecloak(CallBack);
        }
    }
}