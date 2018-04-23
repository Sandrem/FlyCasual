using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;
using GameModes;
using System;
using System.Linq;

namespace ActionsList
{

    public class BarrelRollAction : GenericAction
    {
        public BarrelRollAction()
        {
            Name = "Barrel Roll";
        }

        public override void ActionTake()
        {
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HotacAiPlayer))
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();

                Phases.StartTemporarySubPhaseOld(
                    "Barrel Roll",
                    typeof(SubPhases.BarrelRollPlanningSubPhase),
                    Phases.CurrentSubPhase.CallBack
                );
            }
        }

    }

}

namespace SubPhases
{

    public class BarrelRollPlanningSubPhase : GenericSubPhase
    {
        bool useMobileControls;

        List<Actions.BarrelRollTemplates> availableTemplates = new List<Actions.BarrelRollTemplates>();
        Actions.BarrelRollTemplateVariants selectedTemplateVariant;
        public GameObject TemporaryShipBase;
        public GameObject BarrelRollTemplate;
        ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        public bool IsTractorBeamBarrelRoll = false;

        private Players.GenericPlayer controller;
        public Players.GenericPlayer Controller {
            get {
                return controller ?? TheShip.Owner;
            }
            set {
                controller = value;
            }   
        }

        private float templateWidth;

        private int updatesCount = 0;

        public bool inReposition;

        public float HelperDirection;

        public override void Start()
        {
            Name = "Barrel Roll planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollPlanning();
        }

        private void StartBarrelRollPlanning()
        {
            GenerateListOfAvailableTemplates();
            AskToSelectTemplate(PerfromTemplatePlanning);
        }

        protected void GenerateListOfAvailableTemplates()
        {
            availableTemplates = TheShip.GetAvailableBarrelRollTemplates();
        }

        private void AskToSelectTemplate(Action callback)
        {
            if (availableTemplates.Count > 0)
            {
                RegisterDirectionDecisionTrigger(callback);
            }
            else
            {
                Console.Write("No available templates for Barrel Roll!", LogTypes.Errors, true, "red");
            }                
        }

        private void RegisterDirectionDecisionTrigger(Action callback)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select direction and template",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Controller.PlayerNo,
                EventHandler = StartSelectTemplateSubphase
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        protected void StartSelectTemplateSubphase(object sender, System.EventArgs e)
        {
            BarrelRollDirectionDecisionSubPhase selectBarrelRollTemplate = (BarrelRollDirectionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(BarrelRollDirectionDecisionSubPhase),
                Triggers.FinishTrigger
            );

            foreach (var template in availableTemplates)
            {
                switch (template)
                {
                    case Actions.BarrelRollTemplates.Straight1:
                        selectBarrelRollTemplate.AddDecision("Left Straight 1", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Straight1Left); DecisionSubPhase.ConfirmDecision(); });
                        selectBarrelRollTemplate.AddDecision("Right Straight 1", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Straight1Right); DecisionSubPhase.ConfirmDecision(); });
                        break;
                    case Actions.BarrelRollTemplates.Bank1:
                        selectBarrelRollTemplate.AddDecision("Left Bank 1 Forward", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Bank1LeftForward); DecisionSubPhase.ConfirmDecision(); });
                        selectBarrelRollTemplate.AddDecision("Right Bank 1 Forward", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Bank1RightForward); DecisionSubPhase.ConfirmDecision(); });
                        selectBarrelRollTemplate.AddDecision("Left Bank 1 Backwards", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Bank1LeftBackwards); DecisionSubPhase.ConfirmDecision(); });
                        selectBarrelRollTemplate.AddDecision("Right Bank 1 Backwards", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Bank1RightBackwards); DecisionSubPhase.ConfirmDecision(); });
                        break;
                    case Actions.BarrelRollTemplates.Straight2:
                        selectBarrelRollTemplate.AddDecision("Left Straight 2", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Straight2Left); DecisionSubPhase.ConfirmDecision(); });
                        selectBarrelRollTemplate.AddDecision("Right Straight 2", delegate { SelectTemplate(Actions.BarrelRollTemplateVariants.Straight2Right); DecisionSubPhase.ConfirmDecision(); });
                        break;
                    default:
                        break;
                }
            }

            selectBarrelRollTemplate.InfoText = "Barrel Roll: Select template and direction";

            selectBarrelRollTemplate.DefaultDecisionName = selectBarrelRollTemplate.GetDecisions().First().Name;

            selectBarrelRollTemplate.RequiredPlayer = Controller.PlayerNo;

            selectBarrelRollTemplate.Start();
        }

        public void SelectTemplate(Actions.BarrelRollTemplateVariants templateVariant)
        {
            selectedTemplateVariant = templateVariant;
            BarrelRollTemplate = GetCurrentBarrelRollHelperTemplateGO();
        }

        public void PerfromTemplatePlanning()
        {
            templateWidth = (TheShip.ShipBaseSize == Ship.BaseSize.Small) ? TheShip.ShipBase.HALF_OF_SHIPSTAND_SIZE : TheShip.ShipBase.HALF_OF_SHIPSTAND_SIZE / 2;
            useMobileControls = Application.isMobilePlatform;

            ShowBarrelRollTemplate();

            if (!useMobileControls)
            {
                StartReposition();
            }
            else
            {
                SliderMenu.ShowSlider(
                    -0.75f * TheShip.ShipBase.SHIPSTAND_SIZE,
                    -0.25f * TheShip.ShipBase.SHIPSTAND_SIZE,
                    -0.5f * TheShip.ShipBase.SHIPSTAND_SIZE,
                    ProcessTemplatePositionSlider
                );
            }
        }

        public void ProcessTemplatePositionSlider(float value)
        {
            Vector3 newPositionRel = Vector3.zero;

            newPositionRel.x = HelperDirection * templateWidth;
            newPositionRel.z = value;

            Vector3 newPositionAbs = TheShip.TransformPoint(newPositionRel);

            BarrelRollTemplate.transform.position = newPositionAbs;
        }

        public override void NextButton()
        {
            if (TemporaryShipBase == null)
            {
                SliderMenu.CloseSlider();

                SliderMenu.ShowSlider(
                    0.5f * 1.18f * TheShip.ShipBase.SHIPSTAND_SIZE,
                    1.5F * 1.18f * TheShip.ShipBase.SHIPSTAND_SIZE,
                    1.0F * 1.18f * TheShip.ShipBase.SHIPSTAND_SIZE,
                    ProcessTemporaryShipBaseSlider
                );
            }
            else
            {
                SliderMenu.CloseSlider();
            }

            ConfirmPosition();
        }

        public void ProcessTemporaryShipBaseSlider(float value)
        {
            GameObject finisher = GetCurrentBarrelRollHelperTemplateFinisherGO();
            Vector3 newPositionRel = Vector3.zero;

            newPositionRel.x = HelperDirection * 1.18f * TheShip.ShipBase.SHIPSTAND_SIZE;
            newPositionRel.z = value;

            Vector3 newPositionAbs = finisher.transform.TransformPoint(newPositionRel);
            TemporaryShipBase.transform.position = newPositionAbs;
        }

        private void ShowBarrelRollTemplate()
        {
            GameObject template = GetCurrentBarrelRollHelperTemplateGO();
            if (Controller.GetType() != typeof(Players.NetworkOpponentPlayer)) template.SetActive(true);
            HelperDirection = GetDirectionModifier(selectedTemplateVariant);
            obstaclesStayDetectorMovementTemplate = template.GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorMovementTemplate.TheShip = TheShip;
        }

        private void StartReposition()
        {
            if (!useMobileControls)
            {
                Roster.SetRaycastTargets(false);
                if (Controller.GetType() == typeof(Players.HumanPlayer))
                {
                    inReposition = true;
                }
            }
        }

        private void StopDrag()
        {
            Roster.SetRaycastTargets(true);
            inReposition = false;
        }

        private GameObject GetCurrentBarrelRollHelperTemplateGO()
        {
            return TheShip.GetBarrelRollHelper().Find(selectedTemplateVariant.ToString()).gameObject;
        }

        private GameObject GetCurrentBarrelRollHelperTemplateFinisherGO()
        {
            return GetCurrentBarrelRollHelperTemplateGO().transform.Find("Finisher").gameObject;
        }

        private GameObject GetCurrentBarrelRollHelperTemplateFinisherBasePositionGO()
        {
            return GetCurrentBarrelRollHelperTemplateFinisherGO().transform.Find("BasePosition").gameObject;
        }

        private void ShowTemporaryShipBase()
        {
            if (TemporaryShipBase == null)
            {
                GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
                TemporaryShipBase = MonoBehaviour.Instantiate(prefab, GetCurrentBarrelRollHelperTemplateFinisherBasePositionGO().transform.position, GetCurrentBarrelRollHelperTemplateFinisherBasePositionGO().transform.rotation, BoardManager.GetBoard());
                TemporaryShipBase.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = TheShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
                TemporaryShipBase.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
                obstaclesStayDetectorBase = TemporaryShipBase.GetComponentInChildren<ObstaclesStayDetectorForced>();
                obstaclesStayDetectorBase.TheShip = TheShip;

                if (useMobileControls) ProcessTemporaryShipBaseSlider(SliderMenu.GetSliderValue());
            }
        }

        public override void Update()
        {
            if (inReposition)
            {
                PerfromDrag();
            }
        }

        private void PerfromDrag()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (TemporaryShipBase == null)
                {
                    BarrelRollTemplate.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
                    ApplyBarrelRollTemplateLimits();
                }
                else
                {
                    TemporaryShipBase.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
                    ApplyTemporaryShipBaseTemplateLimits();
                }
            }
        }

        private void ApplyBarrelRollTemplateLimits()
        {
            Vector3 newPosition = TheShip.InverseTransformPoint(BarrelRollTemplate.transform.position);

            Vector3 fixedPositionRel = newPosition;

            fixedPositionRel.x = HelperDirection * templateWidth;
            fixedPositionRel.z = Mathf.Clamp(fixedPositionRel.z, -0.75f * TheShip.ShipBase.SHIPSTAND_SIZE, -0.25f * TheShip.ShipBase.SHIPSTAND_SIZE);

            Vector3 fixedPositionAbs = TheShip.TransformPoint(fixedPositionRel);

            BarrelRollTemplate.transform.position = fixedPositionAbs;
        }

        private void ApplyTemporaryShipBaseTemplateLimits()
        {
            GameObject finisher = GetCurrentBarrelRollHelperTemplateFinisherGO();
            Vector3 newPosition = finisher.transform.InverseTransformPoint(TemporaryShipBase.transform.position);

            Vector3 fixedPositionRel = newPosition;

            fixedPositionRel.x = HelperDirection * 1.18f * TheShip.ShipBase.SHIPSTAND_SIZE;
            fixedPositionRel.z = Mathf.Clamp(fixedPositionRel.z, 0.5f * 1.18f * TheShip.ShipBase.SHIPSTAND_SIZE, 1.5F * 1.18f * TheShip.ShipBase.SHIPSTAND_SIZE);

            Vector3 fixedPositionAbs = finisher.transform.TransformPoint(fixedPositionRel);
            TemporaryShipBase.transform.position = fixedPositionAbs;
        }

        private float GetDirectionModifier(Actions.BarrelRollTemplateVariants templateVariant)
        {
            return (templateVariant.ToString().Contains("Left")) ? -1 : 1;
        }

        public override void ProcessClick()
        {
            StopDrag();

            if (!useMobileControls) ConfirmPosition();
        }

        private void ConfirmPosition()
        {
            if (TemporaryShipBase == null)
            {
                PerfromTemporaryShipBasePlanning();
            }
            else
            {
                GameMode.CurrentGameMode.TryConfirmBarrelRollPosition(selectedTemplateVariant.ToString(), TemporaryShipBase.transform.position, BarrelRollTemplate.transform.position);
            }
        }

        private void PerfromTemporaryShipBasePlanning()
        {
            ShowTemporaryShipBase();

            StartReposition();
        }

        protected class BarrelRollDirectionDecisionSubPhase : DecisionSubPhase { }

        public override void Pause()
        {
            StopDrag();
        }

        public override void Resume()
        {
            StartReposition();
        }

        public void StartBarrelRollExecution()
        {
            Pause();

            TheShip.ToggleShipStandAndPeg(false);
            BarrelRollTemplate.SetActive(false);

            BarrelRollExecutionSubPhase executionSubphase = (BarrelRollExecutionSubPhase) Phases.StartTemporarySubPhaseNew(
                "Barrel Roll execution",
                typeof(BarrelRollExecutionSubPhase),
                CallBack
            );

            executionSubphase.TheShip = TheShip;
            executionSubphase.TemporaryShipBase = TemporaryShipBase;
            executionSubphase.HelperDirection = HelperDirection;
            executionSubphase.IsTractorBeamBarrelRoll = IsTractorBeamBarrelRoll;

            executionSubphase.Start();
        }

        public void CancelBarrelRoll()
        {
            StopDrag();

            TheShip.RemoveAlreadyExecutedAction(typeof(ActionsList.BarrelRollAction));

            TheShip.IsLandedOnObstacle = false;
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;

            MonoBehaviour.Destroy(TemporaryShipBase);
            BarrelRollTemplate.SetActive(false);

            PreviousSubPhase.Resume();
        }

        public void TryConfirmBarrelRollNetwork(string templateName, Vector3 shipPosition, Vector3 movementTemplatePosition)
        {
            StopDrag();

            SelectTemplate((Actions.BarrelRollTemplateVariants) Enum.Parse(typeof(Actions.BarrelRollTemplateVariants), templateName));

            ShowBarrelRollTemplate();
            BarrelRollTemplate.transform.position = movementTemplatePosition;

            ShowTemporaryShipBase();
            TemporaryShipBase.transform.position = shipPosition;
            TemporaryShipBase.transform.rotation = GetCurrentBarrelRollHelperTemplateFinisherBasePositionGO().transform.rotation;

            TryConfirmBarrelRollPosition();
        }

        public void TryConfirmBarrelRollPosition()
        {
            BarrelRollTemplate.SetActive(true);

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

            if (IsBarrelRollAllowed())
            {
                CheckMines();
                TheShip.IsLandedOnObstacle = obstaclesStayDetectorBase.OverlapsAsteroidNow;
                TheShip.ObstaclesHit = new List<Collider>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
                obstaclesStayDetectorMovementTemplate.OverlappedAsteroidsNow
                    .Where((a) => !TheShip.ObstaclesHit.Contains(a)).ToList()
                    .ForEach(TheShip.ObstaclesHit.Add);
                GameMode.CurrentGameMode.StartBarrelRollExecution();
            }
            else
            {
                GameMode.CurrentGameMode.CancelBarrelRoll();
            }
        }

        private void CheckMines()
        {
            foreach (var mineCollider in obstaclesStayDetectorMovementTemplate.OverlapedMinesNow)
            {
                GameObject mineObject = mineCollider.transform.parent.gameObject;
                if (!TheShip.MinesHit.Contains(mineObject)) TheShip.MinesHit.Add(mineObject);
            }
        }

        public bool IsBarrelRollAllowed()
        {
            bool allow = true;

            if (obstaclesStayDetectorBase.OverlapsShipNow || obstaclesStayDetectorMovementTemplate.OverlapsShipNow)
            {
                Messages.ShowError("Cannot overlap another ship");
                allow = false;
            }
            else if (!TheShip.IsIgnoreObstacles && !IsTractorBeamBarrelRoll
                && (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow))
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

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            return false;
        }

    }

    public class BarrelRollExecutionSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;

        private float initialRotation;
        private float plannedRotation;

        private bool performingAnimation;

        public bool IsTractorBeamBarrelRoll;

        public GameObject TemporaryShipBase;
        public float HelperDirection;

        public override void Start()
        {
            Name = "Barrel Roll execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollExecution();
        }

        private void StartBarrelRollExecution()
        {
            Rules.Collision.ClearBumps(TheShip);

            progressCurrent = 0;
            progressTarget = Vector3.Distance(TheShip.GetPosition(), TemporaryShipBase.transform.position);

            initialRotation = (TheShip.GetAngles().y < 180) ? TheShip.GetAngles().y : -(360 - TheShip.GetAngles().y);
            plannedRotation = (TemporaryShipBase.transform.eulerAngles.y - initialRotation < 180) ? TemporaryShipBase.transform.eulerAngles.y : -(360 - TemporaryShipBase.transform.eulerAngles.y);

            if (!IsTractorBeamBarrelRoll) Sounds.PlayFly(TheShip);

            performingAnimation = true;
        }

        public override void Update()
        {
            if (performingAnimation) DoBarrelRollAnimation();
        }

        private void DoBarrelRollAnimation()
        {
            float progressStep = 2.5f * Time.deltaTime * Options.AnimationSpeed;
            progressStep = Mathf.Min(progressStep, progressTarget-progressCurrent);
            progressCurrent += progressStep;

            TheShip.SetPosition(Vector3.MoveTowards(TheShip.GetPosition(), TemporaryShipBase.transform.position, progressStep));

            if (!IsTractorBeamBarrelRoll)
            {
                TheShip.RotateModelDuringBarrelRoll(progressCurrent / progressTarget, HelperDirection);
                TheShip.SetRotationHelper2Angles(new Vector3(0, progressCurrent / progressTarget * (plannedRotation - initialRotation), 0));
                TheShip.MoveUpwards(progressCurrent / progressTarget);
            }

            if (progressCurrent >= progressTarget)
            {
                performingAnimation = false;
                GameMode.CurrentGameMode.FinishBarrelRoll();
            }
        }

        public void FinishBarrelRollAnimation()
        {
            performingAnimation = false;

            TheShip.ApplyRotationHelpers();
            TheShip.ResetRotationHelpers();
            TheShip.SetAngles(TemporaryShipBase.transform.eulerAngles);

            MonoBehaviour.DestroyImmediate(TemporaryShipBase);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;

            MovementTemplates.HideLastMovementRuler();

            TheShip.ToggleShipStandAndPeg(true);
            TheShip.FinishPosition(FinishBarrelRollAnimationPart2);
        }

        private void FinishBarrelRollAnimationPart2()
        {
            Phases.FinishSubPhase(typeof(BarrelRollExecutionSubPhase));
            CallBack();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

    }

}
