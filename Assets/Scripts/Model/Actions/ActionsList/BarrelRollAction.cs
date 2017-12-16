using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;
using GameModes;

namespace ActionsList
{

    public class BarrelRollAction : GenericAction
    {
        public BarrelRollAction() {
            Name = "Barrel Roll";
        }

        public override void ActionTake()
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

namespace SubPhases
{

    public class BarrelRollPlanningSubPhase : GenericSubPhase
    {
        private int updatesCount = 0;

        public GameObject ShipStand;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        public float helperDirection;
        public bool inReposition;

        private float barrelRollDistance;
        private float barrelRollTemplateDistance;

        private float battelRollTemplateLimitTop;
        private float battelRollTemplateLimitBottom;

        public override void Start()
        {
            Name = "Barrel Roll planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollPlanning();
        }

        public void StartBarrelRollPlanning()
        {
            GameObject prefab = (GameObject)Resources.Load(Selection.ThisShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            ShipStand = MonoBehaviour.Instantiate(prefab, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), BoardManager.GetBoard());
            ShipStand.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = Selection.ThisShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
            ShipStand.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorBase = ShipStand.GetComponentInChildren<ObstaclesStayDetectorForced>();
            if (Selection.ThisShip.Owner.GetType() != typeof(Players.HumanPlayer)) ShipStand.SetActive(false);

            barrelRollDistance = (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 2f : 2.5f;
            barrelRollTemplateDistance = (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 0.5f : 1.25f;
            battelRollTemplateLimitTop = (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 0.25f : 1f;
            battelRollTemplateLimitBottom = (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 0.75f : 2f;

            MovementTemplates.CurrentTemplate = MovementTemplates.GetMovement1Ruler();
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HumanPlayer)) MovementTemplates.CurrentTemplate.position = Selection.ThisShip.TransformPoint(new Vector3(barrelRollTemplateDistance, 0, -battelRollTemplateLimitTop));
            obstaclesStayDetectorMovementTemplate = MovementTemplates.CurrentTemplate.GetComponentInChildren<ObstaclesStayDetectorForced>();

            Roster.SetRaycastTargets(false);
            TurnOnDragging();
        }

        private void TurnOnDragging()
        {
            if (Selection.ThisShip.Owner.GetType()==typeof(Players.HumanPlayer)) inReposition = true;
        }

        public override void Update()
        {
            if (inReposition)
            {
                PerfromDrag();
            }
        }

        public override void Pause()
        {
            inReposition = false;
        }

        public override void Resume()
        {
            TurnOnDragging();
        }

        private void PerfromDrag()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (ShipStand != null)
                {
                    ShipStand.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
                    ApplyBarrelRollRepositionLimits();
                }
            }
        }

        private void ApplyBarrelRollRepositionLimits()
        {
            Vector3 newPosition = Selection.ThisShip.InverseTransformPoint(ShipStand.transform.position);
            Vector3 fixedPositionRel = newPosition;

            if (newPosition.z > Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE);
            }

            if (newPosition.z < -Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, -Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE);
            }

            if (newPosition.x > 0f)
            {
                fixedPositionRel = new Vector3(barrelRollDistance, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = 1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles + new Vector3(0, (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 180 : 90, 0);
            }

            if (newPosition.x < 0f)
            {
                fixedPositionRel = new Vector3(-barrelRollDistance, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = -1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles + new Vector3(0, (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 0 : 90, 0);
            }

            Vector3 helperPositionRel = Selection.ThisShip.InverseTransformPoint(MovementTemplates.CurrentTemplate.position);
            helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, helperPositionRel.z);

            if (helperPositionRel.z + battelRollTemplateLimitTop > fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - battelRollTemplateLimitTop);
            }

            if (helperPositionRel.z + battelRollTemplateLimitBottom < fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - battelRollTemplateLimitBottom);
            }

            Vector3 helperPositionAbs = Selection.ThisShip.TransformPoint(helperPositionRel);
            MovementTemplates.CurrentTemplate.position = helperPositionAbs;

            Vector3 fixedPositionAbs = Selection.ThisShip.TransformPoint(fixedPositionRel);
            ShipStand.transform.position = fixedPositionAbs;
        }

        public override void ProcessClick()
        {
            StopDrag();

            GameMode.CurrentGameMode.TryConfirmBarrelRollPosition(ShipStand.transform.position, MovementTemplates.CurrentTemplate.position);
        }

        public void StartBarrelRollExecution(Ship.GenericShip ship)
        {
            Pause();

            Selection.ThisShip.ToggleShipStandAndPeg(false);
            MovementTemplates.CurrentTemplate.gameObject.SetActive(false);

            Phases.StartTemporarySubPhaseOld(
                "Barrel Roll execution",
                typeof(BarrelRollExecutionSubPhase),
                CallBack
            );
        }

        public void CancelBarrelRoll()
        {
            Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.BarrelRollAction));
            Selection.ThisShip.IsLandedOnObstacle = false;
            inReposition = false;
            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            PreviousSubPhase.Resume();
        }

        private void StopDrag()
        {
            Roster.SetRaycastTargets(true);
            inReposition = false;
        }

        public void TryConfirmBarrelRollNetwork(Vector3 shipPosition, Vector3 movementTemplatePosition)
        {
            ShipStand.SetActive(true);
            StopDrag();

            ShipStand.transform.position = shipPosition;

            Vector3 newPosition = Selection.ThisShip.InverseTransformPoint(ShipStand.transform.position);
            if (newPosition.x > 0f)
            {
                helperDirection = 1;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles + new Vector3(0, (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 180 : 90, 0);
            }
            if (newPosition.x < 0f)
            {
                helperDirection = -1;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles + new Vector3(0, (Selection.ThisShip.ShipBaseSize == Ship.BaseSize.Small) ? 0 : 90, 0);
            }
            MovementTemplates.CurrentTemplate.position = movementTemplatePosition;

            TryConfirmBarrelRollPosition();
        }

        public void TryConfirmBarrelRollPosition()
        {
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
                Selection.ThisShip.IsLandedOnObstacle = obstaclesStayDetectorBase.OverlapsAsteroidNow;
                GameMode.CurrentGameMode.StartBarrelRollExecution(Selection.ThisShip);
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
                if (!Selection.ThisShip.MinesHit.Contains(mineObject)) Selection.ThisShip.MinesHit.Add(mineObject);
            }
        }

        private bool IsBarrelRollAllowed()
        {
            bool allow = true;

            if (obstaclesStayDetectorBase.OverlapsShipNow || obstaclesStayDetectorMovementTemplate.OverlapsShipNow)
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

        private bool performingAnimation;

        private GameObject ShipStand;
        private float helperDirection;

        public override void Start()
        {
            Name = "Barrel Roll execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollExecution();
        }

        private void StartBarrelRollExecution()
        {
            ShipStand = (PreviousSubPhase as BarrelRollPlanningSubPhase).ShipStand;
            helperDirection = (PreviousSubPhase as BarrelRollPlanningSubPhase).helperDirection;

            progressCurrent = 0;
            progressTarget = Vector3.Distance(Selection.ThisShip.GetPosition(), ShipStand.transform.position);

            Sounds.PlayFly();

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

            Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), ShipStand.transform.position, progressStep));
            Selection.ThisShip.RotateModelDuringBarrelRoll(progressCurrent / progressTarget, helperDirection);
            Selection.ThisShip.MoveUpwards(progressCurrent / progressTarget);
            if (progressCurrent >= progressTarget)
            {
                performingAnimation = false;
                GameMode.CurrentGameMode.FinishBarrelRoll();
            }
        }

        public void FinishBarrelRollAnimation()
        {
            performingAnimation = false;

            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;

            MovementTemplates.HideLastMovementRuler();
            MovementTemplates.CurrentTemplate.gameObject.SetActive(true);

            Selection.ThisShip.ToggleShipStandAndPeg(true);
            Selection.ThisShip.FinishPosition(FinishBarrelRollAnimationPart2);
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
