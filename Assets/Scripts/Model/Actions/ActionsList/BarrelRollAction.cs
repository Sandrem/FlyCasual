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
            Phases.GoBack();
            Phases.CurrentSubPhase.CallBack();
        }

    }

}

namespace SubPhases
{

    public class BarrelRollPlanningSubPhase : GenericSubPhase
    {
        //Saves forward-center-bottom temporary ship bases and their collisions
        private class BarrelRollShiftData
        {
            public GameObject TemporaryShipBase { get; private set; }
            public Direction Direction { get; private set; }
            public ObstaclesStayDetectorForced Collider { get; private set; }

            public BarrelRollShiftData(Direction direction, GameObject temporaryShipBase)
            {
                Direction = direction;
                TemporaryShipBase = temporaryShipBase;
            }

            public IEnumerator CheckCollisions()
            {
                Collider = TemporaryShipBase.GetComponentInChildren<ObstaclesStayDetectorForced>();
                Collider.TheShip = (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).TheShip;
                Collider.ReCheckCollisionsStart();

                yield return Tools.WaitForFrames(3);
            }
        }

        public GenericAction HostAction { get; set; }

        public override List<GameCommandTypes> AllowedGameCommandTypes {
            get {
                return new List<GameCommandTypes>() { GameCommandTypes.PressNext };
            }
        }

        private List<ManeuverTemplate> AvailableBarrelRollTemplates = new List<ManeuverTemplate>();

        List<BarrelRollShiftData> BarrelRollShiftVariants = new List<BarrelRollShiftData>();
        public ObstaclesStayDetectorForced TemporaryBaseCollider {
            get {
                return BarrelRollShiftVariants.First(n => n.Direction == SelectedShift).Collider;
            }
        }
        public GameObject TemporaryShipBaseFinal;

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

        public List<ActionFailReason> BarrelRollProblems { get; private set; } = new List<ActionFailReason>();

        public bool inReposition;

        public override void Start()
        {
            Name = "Barrel Roll planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollPlanning();
        }

        // Core

        private void StartBarrelRollPlanning()
        {
            AskToSelectTemplate(PerfromTemplatePlanning);
        }

        public void PerfromTemplatePlanning()
        {
            Edition.Current.BarrelRollTemplatePlanning();
        }

        public void PerfromTemplatePlanningSecondEdition()
        {
            GameManagerScript.Instance.StartCoroutine(
                CheckCollisionsOfTemporaryElements(AskBarrelRollShift)
            );
        }

        private void AskBarrelRollShift()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Barrel Roll position",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Controller.PlayerNo,
                EventHandler = StartAskBarrelRollShiftSubphase
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ConfirmBarrelRollPosition);
        }

        public void ConfirmBarrelRollPosition()
        {
            CheckMines();
            SyncCollisions(TemporaryBaseCollider);
            DestroyTemporaryElements();

            GameMode.CurrentGameMode.StartBarrelRollExecution();
        }

        public void StartBarrelRollExecution()
        {
            StartBarrelRollExecutionSubphase();
        }

        // Subs

        private void AskToSelectTemplate(Action callback)
        {
            GenerateListOfAvailableTemplates();

            if (AvailableBarrelRollTemplates.Count > 0)
            {
                RegisterDirectionDecisionTrigger(callback);
            }
            else
            {
                Console.Write("No available templates for Barrel Roll!", LogTypes.Errors, true, "red");
            }                
        }

        private void GenerateListOfAvailableTemplates()
        {
            List<ManeuverTemplate> allowedTemplates = Selection.ThisShip.GetAvailableBarrelRollTemplates();

            foreach (ManeuverTemplate barrelRollTemplate in allowedTemplates)
            {
                AvailableBarrelRollTemplates.Add(barrelRollTemplate);
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

            // Straight templates
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
            }

            // Bank templates
            ManeuverTemplate bank1Left = AvailableBarrelRollTemplates.FirstOrDefault(n => n.Bearing == ManeuverBearing.Bank && n.Speed == ManeuverSpeed.Speed1 && n.Direction == ManeuverDirection.Left);
            ManeuverTemplate bank1Right = AvailableBarrelRollTemplates.FirstOrDefault(n => n.Bearing == ManeuverBearing.Bank && n.Speed == ManeuverSpeed.Speed1 && n.Direction == ManeuverDirection.Right);

            if (bank1Left != null && bank1Right != null)
            {
                selectBarrelRollTemplate.AddDecision(
                    "Left " + bank1Right.NameNoDirection + " Forward",
                    (EventHandler)delegate
                    {
                        SelectTemplate(bank1Right, Direction.Left, Direction.Top);
                        DecisionSubPhase.ConfirmDecision();
                    }
                );

                selectBarrelRollTemplate.AddDecision(
                    "Right " + bank1Left.NameNoDirection + " Forward",
                    (EventHandler)delegate
                    {
                        SelectTemplate(bank1Left, Direction.Right, Direction.Top);
                        DecisionSubPhase.ConfirmDecision();
                    }
                );

                selectBarrelRollTemplate.AddDecision(
                    "Left " + bank1Left.NameNoDirection + " Backwards",
                    (EventHandler)delegate
                    {
                        SelectTemplate(bank1Left, Direction.Left, Direction.Bottom);
                        DecisionSubPhase.ConfirmDecision();
                    }
                );

                selectBarrelRollTemplate.AddDecision(
                    "Right " + bank1Right.NameNoDirection + " Backwards",
                    (EventHandler)delegate
                    {
                        SelectTemplate(bank1Right, Direction.Right, Direction.Bottom);
                        DecisionSubPhase.ConfirmDecision();
                    }
                );
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

        private IEnumerator CheckCollisionsOfTemporaryElements(Action callback)
        {
            yield return CheckTemplate();

            if (!IsColliderDataAllowed(SelectedTemplate.Collider))
            {
                CancelBarrelRoll();
            }
            else
            {
                yield return CheckPotentialFinalPositions();

                if (IsPotentialFinalPositionsAnyAllowed())
                {
                    callback();
                }
                else
                {
                    CancelBarrelRoll();
                }
            }
        }

        private void CancelBarrelRoll()
        {
            DestroyTemporaryElements(isAll: true);
            ShowInformationAboutProblems();

            GameMode.CurrentGameMode.CancelBarrelRoll(BarrelRollProblems);
        }

        private void ShowInformationAboutProblems()
        {
            foreach (var problem in BarrelRollProblems)
            {
                switch (problem)
                {
                    case ActionFailReason.Bumped:
                        Messages.ShowError("Barrel Roll would cause this ship to overlap another ship");
                        break;
                    case ActionFailReason.OffTheBoard:
                        Messages.ShowError("Barrel Roll would cause this ship to leave the battlefield");
                        break;
                    case ActionFailReason.ObstacleHit:
                        Messages.ShowError("Barrel Roll would cause this ship to overlap an obstacle");
                        break;
                    default:
                        break;
                }
            }
        }

        private IEnumerator CheckTemplate()
        {
            ShowBarrelRollTemplate();

            SelectedTemplate.Collider.TheShip = TheShip;
            SelectedTemplate.Collider.ReCheckCollisionsStart();

            yield return Tools.WaitForFrames(3);
        }

        private bool IsColliderDataAllowed(ObstaclesStayDetectorForced collider, bool isBaseFinalPosition = false)
        {
            if (collider.OverlapsShipNow && isBaseFinalPosition)
            {
                BarrelRollProblems.Add(ActionFailReason.Bumped);
            }
            else if (!TheShip.IsIgnoreObstacles && !TheShip.IsIgnoreObstaclesDuringBarrelRoll && !IsTractorBeamBarrelRoll && collider.OverlapsAsteroidNow)
            {
                BarrelRollProblems.Add(ActionFailReason.ObstacleHit);
            }
            else if (collider.OffTheBoardNow)
            {
                BarrelRollProblems.Add(ActionFailReason.OffTheBoard);
            }

            return BarrelRollProblems.Count == 0;
        }

        private IEnumerator CheckPotentialFinalPositions()
        {
            List<Direction> directions = new List<Direction>() {
                Direction.Top,
                Direction.None,
                Direction.Bottom
            };

            foreach (var direction in directions)
            {
                BarrelRollShiftData currentData = new BarrelRollShiftData(
                    direction,
                    ShowTemporaryShipBase(direction, isVisible: false)
                );

                BarrelRollShiftVariants.Add(currentData);
                yield return currentData.CheckCollisions();
            }
        }

        private bool IsPotentialFinalPositionsAnyAllowed()
        {
            bool isAllowed = false;
            foreach (BarrelRollShiftData barrelRollData in BarrelRollShiftVariants)
            {
                BarrelRollProblems = new List<ActionFailReason>();
                if (IsColliderDataAllowed(barrelRollData.Collider))
                {
                    isAllowed = true;
                }
            }
            return isAllowed;
        }

        private void StartAskBarrelRollShiftSubphase(object sender, System.EventArgs e)
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

            selectBarrelRollPosition.ShowSkipButton = false;
            selectBarrelRollPosition.OnNextButtonIsPressed = DecisionSubPhase.ConfirmDecision;

            selectBarrelRollPosition.Start();
        }

        private void SetBarrelRollPosition(Direction direction)
        {
            SelectedShift = direction;

            foreach (var barrelRollShiftVariant in BarrelRollShiftVariants)
            {
                ToggleTemporaryShipBaseVisibility(
                    barrelRollShiftVariant.TemporaryShipBase,
                    barrelRollShiftVariant.Direction == SelectedShift
                );
            }

            if (!IsColliderDataAllowed(TemporaryBaseCollider))
            {
                Messages.ShowError("This final position is not valid, choose another position");
                UI.HideNextButton();
            }
            else
            {
                UI.ShowNextButton();
                UI.HighlightNextButton();
            }

            DecisionSubPhase.ResetInput();
        }

        private void ShowBarrelRollTemplate()
        {
            SelectedTemplate.ApplyTemplate(
                TheShip,
                (SelectedDirectionPrimary == Direction.Left) ? TheShip.GetLeft() : TheShip.GetRight(),
                SelectedDirectionPrimary
            );
        }

        private GameObject ShowTemporaryShipBase(Direction shiftDirection, bool isVisible = true)
        {
            GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            GameObject temporaryShipBase = MonoBehaviour.Instantiate(
                prefab,
                SelectedTemplate.GetFinalPosition(),
                SelectedTemplate.GetFinalRotation(),
                Board.GetBoard()
            );

            int directionModifier = (SelectedDirectionPrimary == Direction.Left) ? -1 : 1;

            float finalShift = 0;
            switch (shiftDirection)
            {
                case Direction.Top:
                    finalShift += (SelectedTemplate.IsSideTemplate)? 0.5f : 0.25f;
                    break;
                case Direction.Bottom:
                    finalShift -= (SelectedTemplate.IsSideTemplate) ? 0.5f : 0.25f;
                    break;
                default:
                    break;
            }

            temporaryShipBase.transform.localEulerAngles += new Vector3(0, directionModifier * -90, 0);

            Vector3 shift = new Vector3(
                directionModifier * TheShip.ShipBase.HALF_OF_SHIPSTAND_SIZE,
                0,
                TheShip.ShipBase.HALF_OF_SHIPSTAND_SIZE + finalShift
            );
            Vector3 absPosition = temporaryShipBase.transform.TransformPoint(shift);

            temporaryShipBase.transform.position = absPosition;

            temporaryShipBase.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = TheShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
            temporaryShipBase.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();

            ToggleTemporaryShipBaseVisibility(temporaryShipBase, isVisible);

            return temporaryShipBase;
        }

        private void ToggleTemporaryShipBaseVisibility(GameObject shipBase, bool isVisible)
        {
            foreach (Renderer renderer in shipBase.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = isVisible;
            }
        }

        protected class BarrelRollDirectionDecisionSubPhase : DecisionSubPhase { }

        protected class BarrelRollPositionDecisionSubPhase : DecisionSubPhase { }

        public void CancelBarrelRoll(List<ActionFailReason> barrelRollProblems)
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, barrelRollProblems);
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

        private void DestroyTemporaryElements(bool isAll = false)
        {
            foreach (var data in BarrelRollShiftVariants)
            {
                if (data.Direction == SelectedShift && !isAll)
                {
                    TemporaryShipBaseFinal = data.TemporaryShipBase;
                }
                else
                {
                    GameObject.Destroy(data.TemporaryShipBase);
                }
            }
            BarrelRollShiftVariants = new List<BarrelRollShiftData>();
            SelectedTemplate.DestroyTemplate();
        }

        private void CheckMines()
        {
            foreach (var mineCollider in SelectedTemplate.Collider.OverlapedMinesNow)
            {
                GameObject mineObject = mineCollider.transform.parent.gameObject;
                if (!TheShip.MinesHit.Contains(mineObject)) TheShip.MinesHit.Add(mineObject);
            }
        }

        private void SyncCollisions(ObstaclesStayDetectorForced collider)
        {
            TheShip.ObstaclesLanded = new List<GenericObstacle>(collider.OverlappedAsteroidsNow);
            TheShip.ObstaclesHit = new List<GenericObstacle>(collider.OverlappedAsteroidsNow);
            collider.OverlappedAsteroidsNow
                .Where((a) => !TheShip.ObstaclesHit.Contains(a)).ToList()
                .ForEach(TheShip.ObstaclesHit.Add);
        }

        private void StartBarrelRollExecutionSubphase()
        {
            Pause();

            TheShip.ToggleShipStandAndPeg(false);

            BarrelRollExecutionSubPhase executionSubphase = (BarrelRollExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Barrel Roll execution",
                typeof(BarrelRollExecutionSubPhase),
                CallBack
            );

            executionSubphase.TheShip = TheShip;
            executionSubphase.TemporaryShipBase = TemporaryShipBaseFinal;
            executionSubphase.Direction = SelectedDirectionPrimary;
            executionSubphase.IsTractorBeamBarrelRoll = IsTractorBeamBarrelRoll;

            executionSubphase.Start();
        }

        public override void Next()
        {
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
        public Direction Direction;

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
                TheShip.RotateModelDuringBarrelRoll(progressCurrent / progressTarget, (Direction == Direction.Left) ? -1 : 1);
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
