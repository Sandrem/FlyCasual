using GameModes;
using Obstacles;
using RuleSets;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubPhases
{

    public class DecloakDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
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

    public class DecloakPlanningSubPhase : GenericSubPhase
    {
        private int updatesCount = 0;

        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        Dictionary<string, Vector3> AvailableDecloakDirections = new Dictionary<string, Vector3>();
        public string SelectedDecloakHelper;

        public float helperDirection;
        public bool inReposition;

        List<Actions.DecloakTemplates> availableTemplates = new List<Actions.DecloakTemplates>();
        Actions.DecloakTemplateVariants selectedTemplateVariant;
        public GameObject DecloakTemplate;
        public float HelperDirection;
        public GameObject TemporaryShipBase;

        public override void Start()
        {
            Name = "Decloak planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartDecloakPlanning();
        }

        public void StartDecloakPlanning()
        {
            GenerateListOfAvailableTemplates();
            AskToSelectTemplate(PerfromTemplatePlanning);
        }

        public void PerfromTemplatePlanning()
        {
            RuleSet.Instance.DecloakTemplatePlanning();
        }

        public void PerfromTemplatePlanningFirstEdition()
        {
            // Temporary
            PerfromTemplatePlanningSecondEdition();
        }

        public void PerfromTemplatePlanningSecondEdition()
        {
            if (IsBoostTemplate(selectedTemplateVariant))
            {
                ShowBarrelRollTemplate();
                ShowTemporaryShipBase();
                ConfirmPosition();
            }
            else
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Decloak position",
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    EventHandler = AskDecloakPosition
                });

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ConfirmPosition);
            }
        }

        private bool IsBoostTemplate(Actions.DecloakTemplateVariants selectedTemplateVariant)
        {
            return selectedTemplateVariant == Actions.DecloakTemplateVariants.Straight2Forward
                || selectedTemplateVariant == Actions.DecloakTemplateVariants.Bank2ForwardLeft
                || selectedTemplateVariant == Actions.DecloakTemplateVariants.Bank2ForwardRight;
        }

        private void ConfirmPosition()
        {
            if (TemporaryShipBase == null)
            {
                PerfromTemporaryShipBasePlanning();
            }
            else
            {
                GameMode.CurrentGameMode.TryConfirmDecloakPosition(TemporaryShipBase.transform.position, selectedTemplateVariant.ToString(), DecloakTemplate.transform.position, DecloakTemplate.transform.eulerAngles);
            }
        }

        private void PerfromTemporaryShipBasePlanning()
        {
            ShowTemporaryShipBase();

            StartReposition();
        }

        private void StartReposition()
        {
            Roster.SetRaycastTargets(false);
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HumanPlayer))
            {
                inReposition = true;
            }
        }

        private void AskDecloakPosition(object sender, System.EventArgs e)
        {
            DecloakPositionDecisionSubPhase selectBarrelRollPosition = (DecloakPositionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                 Name,
                 typeof(DecloakPositionDecisionSubPhase),
                 Triggers.FinishTrigger
            );

            selectBarrelRollPosition.AddDecision("Forward", delegate { SetBarrelRollPosition(0.5f); }, isCentered: true);
            selectBarrelRollPosition.AddDecision("Center", delegate { SetBarrelRollPosition(0); }, isCentered: true);
            selectBarrelRollPosition.AddDecision("Backwards", delegate { SetBarrelRollPosition(-0.5f); }, isCentered: true);

            selectBarrelRollPosition.InfoText = "Decloak: Select position";

            selectBarrelRollPosition.DefaultDecisionName = "Center";

            selectBarrelRollPosition.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBarrelRollPosition.Start();
        }

        private void SetBarrelRollPosition(float position)
        {
            ShowBarrelRollTemplate();
            ShowTemporaryShipBase();

            ProcessTemporaryShipBaseSlider(position);

            DecisionSubPhase.ConfirmDecision();
        }

        private void ShowBarrelRollTemplate()
        {
            GameObject template = GetCurrentDecloakHelperTemplateGO();
            if (Selection.ThisShip.Owner.GetType() != typeof(Players.NetworkOpponentPlayer)) template.SetActive(true);
            HelperDirection = GetDirectionModifier(selectedTemplateVariant);
            obstaclesStayDetectorMovementTemplate = template.GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorMovementTemplate.TheShip = Selection.ThisShip;
        }

        private void ShowTemporaryShipBase()
        {
            if (TemporaryShipBase == null)
            {
                GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
                TemporaryShipBase = MonoBehaviour.Instantiate(prefab, this.GetCurrentDecloakHelperTemplateFinisherBasePositionGO().transform.position, this.GetCurrentDecloakHelperTemplateFinisherBasePositionGO().transform.rotation, BoardTools.Board.GetBoard());
                TemporaryShipBase.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = TheShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
                TemporaryShipBase.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
                obstaclesStayDetectorBase = TemporaryShipBase.GetComponentInChildren<ObstaclesStayDetectorForced>();
                obstaclesStayDetectorBase.TheShip = TheShip;
            }
        }

        public void ProcessTemplatePositionSlider(float value)
        {
            Vector3 newPositionRel = Vector3.zero;

            newPositionRel.x = HelperDirection * TheShip.ShipBase.HALF_OF_SHIPSTAND_SIZE;
            newPositionRel.z = value;

            Vector3 newPositionAbs = TheShip.TransformPoint(newPositionRel);

            DecloakTemplate.transform.position = newPositionAbs;
        }

        public void ProcessTemporaryShipBaseSlider(float value)
        {
            GameObject finisherBase = GetCurrentDecloakHelperTemplateFinisherBasePositionGO();
            Vector3 positionAbs = finisherBase.transform.TransformPoint(new Vector3(0, 0, value*1.18f));
            TemporaryShipBase.transform.position = positionAbs;
        }

        private GameObject GetCurrentDecloakHelperTemplateFinisherBasePositionGO()
        {
            return GetCurrentDecloakHelperTemplateFinisherGO().transform.Find("BasePosition").gameObject;
        }

        private GameObject GetCurrentDecloakHelperTemplateFinisherGO()
        {
            return GetCurrentDecloakHelperTemplateGO().transform.Find("Finisher").gameObject;
        }

        private float GetDirectionModifier(Actions.DecloakTemplateVariants templateVariant)
        {
            return (templateVariant.ToString().Contains("Left")) ? -1 : 1;
        }

        protected void GenerateListOfAvailableTemplates()
        {
            availableTemplates = Selection.ThisShip.GetAvailableDecloakTemplates();
        }

        private void AskToSelectTemplate(Action callback)
        {
            if (availableTemplates.Count > 0)
            {
                RegisterDirectionDecisionTrigger(callback);
            }
            else
            {
                Console.Write("No available templates for Decloak!", LogTypes.Errors, true, "red");
            }
        }

        private void RegisterDirectionDecisionTrigger(Action callback)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select direction and template",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = StartSelectTemplateSubphase
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        protected void StartSelectTemplateSubphase(object sender, System.EventArgs e)
        {
            DecloakDirectionDecisionSubPhase selectDecloakTemplate = (DecloakDirectionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(DecloakDirectionDecisionSubPhase),
                Triggers.FinishTrigger
            );

            foreach (var template in availableTemplates)
            {
                switch (template)
                {
                    case Actions.DecloakTemplates.Straight2:
                        selectDecloakTemplate.AddDecision("Forward Straight 2", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Straight2Forward); DecisionSubPhase.ConfirmDecision();}, isCentered: true);
                        selectDecloakTemplate.AddDecision("Left Straight 2", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Straight2Left); DecisionSubPhase.ConfirmDecision(); });
                        selectDecloakTemplate.AddDecision("Right Straight 2", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Straight2Right); DecisionSubPhase.ConfirmDecision(); });
                        break;
                    case Actions.DecloakTemplates.Bank2:
                        selectDecloakTemplate.AddDecision("Forward Bank 2 Left", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Bank2ForwardLeft); DecisionSubPhase.ConfirmDecision(); });
                        selectDecloakTemplate.AddDecision("Forward Bank 2 Right", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Bank2ForwardRight); DecisionSubPhase.ConfirmDecision(); });
                        selectDecloakTemplate.AddDecision("Left Bank 2 Forward", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Bank2LeftForward); DecisionSubPhase.ConfirmDecision(); });
                        selectDecloakTemplate.AddDecision("Right Bank 2 Forward", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Bank2RightForward); DecisionSubPhase.ConfirmDecision(); });
                        selectDecloakTemplate.AddDecision("Left Bank 2 Backwards", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Bank2LeftBackwards); DecisionSubPhase.ConfirmDecision(); });
                        selectDecloakTemplate.AddDecision("Right Bank 2 Backwards", delegate { SelectTemplate(Actions.DecloakTemplateVariants.Bank2RightBackwards); DecisionSubPhase.ConfirmDecision(); });
                        break;
                    default:
                        break;
                }
            }

            selectDecloakTemplate.InfoText = "Decloak: Select template and direction";

            selectDecloakTemplate.DefaultDecisionName = selectDecloakTemplate.GetDecisions().First().Name;

            selectDecloakTemplate.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectDecloakTemplate.Start();
        }

        public void SelectTemplate(Actions.DecloakTemplateVariants templateVariant)
        {
            selectedTemplateVariant = templateVariant;
            DecloakTemplate = GetCurrentDecloakHelperTemplateGO();
        }

        private GameObject GetCurrentDecloakHelperTemplateGO()
        {
            return Selection.ThisShip.GetDecloakHelper().Find(selectedTemplateVariant.ToString()).gameObject;
        }

        public void StartDecloakExecution(GenericShip ship)
        {
            Pause();

            Selection.ThisShip.ToggleShipStandAndPeg(false);
            DecloakTemplate.SetActive(false);

            DecloakExecutionSubPhase executionSubphase = Phases.StartTemporarySubPhaseNew<DecloakExecutionSubPhase>(
                "Decloak execution",
                CallBack
            );

            executionSubphase.TemporaryShipBase = TemporaryShipBase;
            executionSubphase.HelperDirection = HelperDirection;

            executionSubphase.Start();
        }

        public void CancelDecloak()
        {
            StopPlanning();

            Selection.ThisShip.IsLandedOnObstacle = false;
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;

            MonoBehaviour.Destroy(TemporaryShipBase);
            DecloakTemplate.SetActive(false);

            RuleSet.Instance.ActionIsFailed(TheShip, typeof(ActionsList.CloakAction));
        }

        private void StopPlanning()
        {
            Roster.SetRaycastTargets(true);
            inReposition = false;
        }

        public void TryConfirmDecloakNetwork(Vector3 shipPosition, string decloakHelper, Vector3 movementTemplatePosition, Vector3 movementTemplateAngles)
        {
            TryConfirmDecloakPosition();
        }

        public void TryConfirmDecloakPosition()
        {
            DecloakTemplate.SetActive(true);

            obstaclesStayDetectorBase.ReCheckCollisionsStart();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsStart();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(UpdateColisionDetection);
        }

        private bool UpdateColisionDetection()
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                GetResults();
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults()
        {
            obstaclesStayDetectorBase.ReCheckCollisionsFinish();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsFinish();

            if (IsDecloakAllowed())
            {
                CheckMines();
                Selection.ThisShip.LandedOnObstacles = new List<GenericObstacle>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
                GameMode.CurrentGameMode.StartDecloakExecution(Selection.ThisShip);
            }
            else
            {
                GameMode.CurrentGameMode.CancelDecloak();
            }
        }

        private void CheckMines()
        {
            foreach (var mineCollider in obstaclesStayDetectorMovementTemplate.OverlapedMinesNow)
            {
                GameObject mineObject = mineCollider.transform.parent.gameObject;
                if (!Selection.ThisShip.MinesHit.Contains(mineObject)) Selection.ThisShip.MinesHit.Add(mineObject);
            }
        }

        private bool IsDecloakAllowed()
        {
            bool allow = true;

            if (obstaclesStayDetectorBase.OverlapsShipNow)
            {
                Messages.ShowError("Cannot overlap another ship");
                allow = false;
            }
            else if ((!Selection.ThisShip.IsIgnoreObstacles) && (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow))
            {
                Messages.ShowError("Cannot overlap asteroid");
                allow = false;
            }
            else if (obstaclesStayDetectorBase.OffTheBoardNow || obstaclesStayDetectorMovementTemplate.OffTheBoardNow)
            {
                Messages.ShowError("Cannot leave the battlefield");
                allow = false;
            }

            return allow;
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            return false;
        }

        public override void SkipButton()
        {
            CancelDecloak();
        }

        protected class DecloakDirectionDecisionSubPhase : DecisionSubPhase { }

        protected class DecloakPositionDecisionSubPhase : DecisionSubPhase { }

    }

    public class DecloakExecutionSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;

        private float initialRotation;
        private float plannedRotation;

        private bool performingAnimation;

        public GameObject TemporaryShipBase;
        public float HelperDirection;

        public override void Start()
        {
            Name = "Decloak execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartDecloakExecution();
        }

        private void StartDecloakExecution()
        {
            Rules.Collision.ClearBumps(Selection.ThisShip);

            progressCurrent = 0;
            progressTarget = Vector3.Distance(Selection.ThisShip.GetPosition(), TemporaryShipBase.transform.position);

            initialRotation = (TheShip.GetAngles().y < 180) ? TheShip.GetAngles().y : -(360 - TheShip.GetAngles().y);
            plannedRotation = (TemporaryShipBase.transform.eulerAngles.y - initialRotation < 180) ? TemporaryShipBase.transform.eulerAngles.y : -(360 - TemporaryShipBase.transform.eulerAngles.y);

            Sounds.PlayFly();

            performingAnimation = true;
        }

        public override void Update()
        {
            if (performingAnimation) DoDecloakAnimation();
        }

        private void DoDecloakAnimation()
        {
            float progressStep = 2.5f * Time.deltaTime * Options.AnimationSpeed;
            progressStep = Mathf.Min(progressStep, progressTarget - progressCurrent);
            progressCurrent += progressStep;

            Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), TemporaryShipBase.transform.position, progressStep));

            Selection.ThisShip.RotateModelDuringBarrelRoll(progressCurrent / progressTarget, HelperDirection);
            TheShip.SetRotationHelper2Angles(new Vector3(0, progressCurrent / progressTarget * (plannedRotation - initialRotation), 0));
            Selection.ThisShip.MoveUpwards(progressCurrent / progressTarget);

            if (progressCurrent >= progressTarget)
            {
                performingAnimation = false;
                GameMode.CurrentGameMode.FinishDecloak();
            }
        }

        public void FinishDecloakAnimation()
        {
            performingAnimation = false;

            TheShip.ApplyRotationHelpers();
            TheShip.ResetRotationHelpers();
            TheShip.SetAngles(TemporaryShipBase.transform.eulerAngles);

            MonoBehaviour.Destroy(TemporaryShipBase);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;

            MovementTemplates.HideLastMovementRuler();
            MovementTemplates.CurrentTemplate.gameObject.SetActive(true);

            Selection.ThisShip.ToggleShipStandAndPeg(true);
            Selection.ThisShip.FinishPosition(FinishDecloakAnimationPart2);
        }

        private void FinishDecloakAnimationPart2()
        {
            Phases.FinishSubPhase(typeof(DecloakExecutionSubPhase));

            Selection.ThisShip.Tokens.SpendToken(typeof(Tokens.CloakToken), FinishDecloakAnimationPart3);
        }

        private void FinishDecloakAnimationPart3()
        {
            Selection.ThisShip.CallDecloak(CallBack);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

    }

}