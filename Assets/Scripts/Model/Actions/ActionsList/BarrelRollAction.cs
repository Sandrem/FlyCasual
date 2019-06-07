using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
using GameModes;
using System;
using System.Linq;
using Editions;
using Obstacles;
using ActionsList;
using SubPhases;
using Actions;
using Movement;

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
            if (Selection.ThisShip.Owner.UsesHotacAiRules)
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();

                BarrelRollPlanningSubPhase subphase = Phases.StartTemporarySubPhaseNew<BarrelRollPlanningSubPhase>(
                    "Barrel Roll",
                    Phases.CurrentSubPhase.CallBack
                );
                subphase.HostAction = this;
                subphase.Start();
            }
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            if (hasSecondChance)
            {
                (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).PerfromTemplatePlanning();
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
                Phases.GoBack();
            }
        }

    }

}

namespace SubPhases
{

    public class BarrelRollPlanningSubPhase : GenericSubPhase
    {
        public GenericAction HostAction { get; set; }

        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.PressNext }; } }

        private List<ManeuverTemplate> AvailableBarrelRollTemplates = new List<ManeuverTemplate>();
        public GameObject TemporaryShipBase;
        public GameObject BarrelRollTemplate;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        private ManeuverTemplate SelectedTemplate;
        private Direction SelectedDirectionPrimary;
        private Direction SelectedDirectionSecondary;
        private Direction SelectedShift;

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
            List<ManeuverTemplate> allowedTemplates = Selection.ThisShip.GetAvailableBarrelRollTemplates();

            foreach (ManeuverTemplate barrelRollTemplate in allowedTemplates)
            {
                AvailableBarrelRollTemplates.Add(barrelRollTemplate);
            }

            AskToSelectTemplate(PerfromTemplatePlanning);
        }

        private void AskToSelectTemplate(Action callback)
        {
            if (AvailableBarrelRollTemplates.Count > 0)
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

            foreach (ManeuverTemplate template in AvailableBarrelRollTemplates)
            {
                if (template.Bearing == ManeuverBearing.Straight)
                {
                    selectBarrelRollTemplate.AddDecision(
                        "Left " + template.NameNoDirection,
                        (EventHandler)delegate {
                            SelectTemplate(template, Direction.Left);
                            DecisionSubPhase.ConfirmDecision();
                        }
                    );

                    selectBarrelRollTemplate.AddDecision(
                        "Right " + template.NameNoDirection,
                        (EventHandler)delegate {
                            SelectTemplate(template, Direction.Right);
                            DecisionSubPhase.ConfirmDecision();
                        }
                    );
                }

                if (template.Bearing == ManeuverBearing.Bank)
                {
                    selectBarrelRollTemplate.AddDecision(
                        "Left " + template.NameNoDirection + " Forward",
                        (EventHandler)delegate {
                            SelectTemplate(template, Direction.Left, Direction.Top);
                            DecisionSubPhase.ConfirmDecision();
                        }
                    );

                    selectBarrelRollTemplate.AddDecision(
                        "Right " + template.NameNoDirection + " Forward",
                        (EventHandler)delegate {
                            SelectTemplate(template, Direction.Right, Direction.Top);
                            DecisionSubPhase.ConfirmDecision();
                        }
                    );

                    selectBarrelRollTemplate.AddDecision(
                        "Left " + template.NameNoDirection + " Backwards",
                        (EventHandler)delegate {
                            SelectTemplate(template, Direction.Left, Direction.Bottom);
                            DecisionSubPhase.ConfirmDecision();
                        }
                    );

                    selectBarrelRollTemplate.AddDecision(
                        "Right " + template.NameNoDirection + " Backwards",
                        (EventHandler)delegate {
                            SelectTemplate(template, Direction.Right, Direction.Bottom);
                            DecisionSubPhase.ConfirmDecision();
                        }
                    );
                }
            }

            selectBarrelRollTemplate.InfoText = "Barrel Roll: Select template and direction";

            selectBarrelRollTemplate.DefaultDecisionName = selectBarrelRollTemplate.GetDecisions().First().Name;

            selectBarrelRollTemplate.RequiredPlayer = Controller.PlayerNo;

            selectBarrelRollTemplate.Start();
        }

        public void SelectTemplate(ManeuverTemplate template, Direction directionPrimary, Direction directionSecondary = Direction.None)
        {
            SelectedTemplate = template;
            SelectedDirectionPrimary = directionPrimary;
            SelectedDirectionSecondary = directionSecondary;
        }

        public void PerfromTemplatePlanning()
        {
            Edition.Current.BarrelRollTemplatePlanning();
        }

        //OLD
        public void PerfromTemplatePlanningFirstEdition()
        {
            PerfromTemplatePlanningSecondEdition();

            /*useMobileControls = Application.isMobilePlatform;

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
                IsReadyForCommands = true;
            }*/
        }

        public void PerfromTemplatePlanningSecondEdition()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Barrel Roll position",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Controller.PlayerNo,
                EventHandler = AskBarrelRollPosition
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, TryConfirmBarrelRollPosition);
        }

        private void AskBarrelRollPosition(object sender, System.EventArgs e)
        {
            BarrelRollPositionDecisionSubPhase selectBarrelRollPosition = (BarrelRollPositionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                 Name,
                 typeof(BarrelRollPositionDecisionSubPhase),
                 Triggers.FinishTrigger
            );

            selectBarrelRollPosition.AddDecision("Forward",     delegate { SetBarrelRollPosition(Direction.Top); },     isCentered: true);
            selectBarrelRollPosition.AddDecision("Center",      delegate { SetBarrelRollPosition(Direction.None); },    isCentered: true);
            selectBarrelRollPosition.AddDecision("Backwards",   delegate { SetBarrelRollPosition(Direction.Bottom); },  isCentered: true);

            selectBarrelRollPosition.InfoText = "Barrel Roll: Select position";

            selectBarrelRollPosition.DefaultDecisionName = "Center";

            selectBarrelRollPosition.RequiredPlayer = Controller.PlayerNo;

            selectBarrelRollPosition.ShowSkipButton = true;
            selectBarrelRollPosition.OnSkipButtonIsPressedOverwrite = FailBarrelRoll;

            selectBarrelRollPosition.Start();
        }

        private void SetBarrelRollPosition(Direction direction)
        {
            SelectedShift = direction;

            ShowBarrelRollTemplate();
            ShowTemporaryShipBase();

            DecisionSubPhase.ConfirmDecision();
        }

        private void ShowBarrelRollTemplate()
        {
            SelectedTemplate.ApplyTemplate(
                TheShip,
                (SelectedDirectionPrimary == Direction.Left) ? TheShip.GetLeft() : TheShip.GetRight(),
                SelectedDirectionPrimary
            );
        }

        private void ShowTemporaryShipBase()
        {
            if (TemporaryShipBase == null)
            {
                GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
                TemporaryShipBase = MonoBehaviour.Instantiate(
                    prefab,
                    SelectedTemplate.GetFinalPosition(),
                    SelectedTemplate.GetFinalRotation(),
                    Board.GetBoard()
                );

                int directionModifier = (SelectedDirectionPrimary == Direction.Left) ? -1 : 1;

                float finalShift = 0;
                switch (SelectedShift)
                {
                    case Direction.Top:
                        finalShift += Board.BoardIntoWorld(TheShip.ShipBase.SHIPSTAND_SIZE_CM / 4);
                        break;
                    case Direction.Bottom:
                        finalShift -= Board.BoardIntoWorld(TheShip.ShipBase.SHIPSTAND_SIZE_CM / 4);
                        break;
                    default:
                        break;
                }

                TemporaryShipBase.transform.localEulerAngles += new Vector3(0, directionModifier * -90, 0);
                TemporaryShipBase.transform.position += new Vector3(
                    directionModifier * Board.BoardIntoWorld(TheShip.ShipBase.SHIPSTAND_SIZE_CM / 2),
                    0,
                    Board.BoardIntoWorld(TheShip.ShipBase.SHIPSTAND_SIZE_CM / 2) + finalShift
                );

                TemporaryShipBase.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = TheShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
                TemporaryShipBase.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
            }
        }

        protected class BarrelRollDirectionDecisionSubPhase : DecisionSubPhase { }

        protected class BarrelRollPositionDecisionSubPhase : DecisionSubPhase { }

        public void CancelBarrelRoll(List<ActionFailReason> barrelRollProblems)
        {
            FinishBarrelRollPreparations();

            Rules.Actions.ActionIsFailed(TheShip, HostAction, barrelRollProblems, hasSecondChance: true);
        }

        private void FinishBarrelRollPreparations()
        {
            TheShip.IsLandedOnObstacle = false;
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;

            if (TemporaryShipBase != null) MonoBehaviour.Destroy(TemporaryShipBase);
            if (SelectedTemplate != null) SelectedTemplate.DestroyTemplate();
        }

        //OLD
        public void TryConfirmBarrelRollNetwork(string templateName, Vector3 shipPosition, Vector3 movementTemplatePosition)
        {
            /*StopDrag();

            SelectTemplate((ActionsHolder.BarrelRollTemplateVariants) Enum.Parse(typeof(ActionsHolder.BarrelRollTemplateVariants), templateName));

            ShowBarrelRollTemplate();
            BarrelRollTemplate.transform.position = movementTemplatePosition;

            ShowTemporaryShipBase();
            TemporaryShipBase.transform.position = shipPosition;
            TemporaryShipBase.transform.rotation = GetCurrentBarrelRollHelperTemplateFinisherBasePositionGO().transform.rotation;

            TryConfirmBarrelRollPosition();*/
        }

        public void TryConfirmBarrelRollPosition()
        {
            return;

            //TODO: Continue from here

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
                updatesCount = 0;
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

            List<ActionFailReason> barrelRollProblems = CheckBarrelRollProblems();

            if (barrelRollProblems.Count == 0)
            {
                CheckMines();
                TheShip.ObstaclesLanded = new List<GenericObstacle>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
                TheShip.ObstaclesHit = new List<GenericObstacle>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
                obstaclesStayDetectorMovementTemplate.OverlappedAsteroidsNow
                    .Where((a) => !TheShip.ObstaclesHit.Contains(a)).ToList()
                    .ForEach(TheShip.ObstaclesHit.Add);
                GameMode.CurrentGameMode.StartBarrelRollExecution();
            }
            else
            {
                GameMode.CurrentGameMode.CancelBarrelRoll(barrelRollProblems);
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

        public List<ActionFailReason> CheckBarrelRollProblems()
        {
            List<ActionFailReason> failReasons = new List<ActionFailReason>();

            if (obstaclesStayDetectorBase.OverlapsShipNow)
            {
                Messages.ShowError("Barrel Roll would cause this ship to overlap another ship");
                failReasons.Add(ActionFailReason.Bumped);
            }
            else if (!TheShip.IsIgnoreObstacles && !TheShip.IsIgnoreObstaclesDuringBarrelRoll && !IsTractorBeamBarrelRoll
                && (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow))
            {
                Messages.ShowError("Barrel Roll would cause this ship to overlap an obstacle");
                failReasons.Add(ActionFailReason.ObstacleHit);
            }
            else if (obstaclesStayDetectorBase.OffTheBoardNow || obstaclesStayDetectorMovementTemplate.OffTheBoardNow)
            {
                Messages.ShowError("Barrel Roll would cause this ship to leave the battlefield");
                failReasons.Add(ActionFailReason.OffTheBoard);
            }

            return failReasons;
        }

        public void StartBarrelRollExecution()
        {
            Pause();

            TheShip.ToggleShipStandAndPeg(false);
            BarrelRollTemplate.SetActive(false);

            BarrelRollExecutionSubPhase executionSubphase = (BarrelRollExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
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

        public override void Next()
        {
            FinishBarrelRollPreparations();
            Phases.CurrentSubPhase = PreviousSubPhase;
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

        private void FailBarrelRoll()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, new List<ActionFailReason> { ActionFailReason.Bumped, ActionFailReason.ObstacleHit, ActionFailReason.OffTheBoard });
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
