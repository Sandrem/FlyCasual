using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;

namespace ActionsList
{

    public class DecloakAction : GenericAction
    {
        public DecloakAction() {
            Name = "Decloak";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();

            Phases.StartTemporarySubPhase(
                "Decloak",
                typeof(SubPhases.DecloakPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

    }

}

namespace SubPhases
{

    public class DecloakPlanningSubPhase : GenericSubPhase
    {
        private int updatesCount = 0;

        public GameObject ShipStand;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        Dictionary<string, Vector3> AvailableDecloakDirections = new Dictionary<string, Vector3>();
        public string SelectedDecloakHelper;

        public float helperDirection;
        public bool inReposition;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Decloak planning";
            IsTemporary = true;
            UpdateHelpInfo();

            //Game.UI.ShowSkipButton();

            StartDecloakPlanning();
        }

        public void StartDecloakPlanning()
        {
            ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), BoardManager.GetBoard());
            ShipStand.transform.Find("ShipStandTemplate").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = Selection.ThisShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
            obstaclesStayDetectorBase = ShipStand.GetComponentInChildren<ObstaclesStayDetectorForced>();

            // TODO: 2 ways of collisions!!!

            MovementTemplates.CurrentTemplate = MovementTemplates.GetMovement2Ruler();
            MovementTemplates.CurrentTemplate.position = Selection.ThisShip.TransformPoint(new Vector3(0.5f, 0, -0.25f));

            foreach (Transform decloakHelper in Selection.ThisShip.GetDecloakHelper())
            {
                AvailableDecloakDirections.Add(decloakHelper.name, decloakHelper.Find("Finisher").position);
            }

            Roster.SetRaycastTargets(false);
            inReposition = true;
        }

        public override void Update()
        {
            if (inReposition)
            {
                SelectDecloakHelper();
            }
        }

        private void SelectDecloakHelper()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                ShowNearestDecloakHelper(GetNearestDecloakHelper(new Vector3(hit.point.x, 0f, hit.point.z)));
            }
        }

        private void ShowNearestDecloakHelper(string name)
        {
            if (SelectedDecloakHelper != name)
            {
                if (name == "Forward")
                {
                    MovementTemplates.CurrentTemplate.gameObject.SetActive(false);

                    Selection.ThisShip.GetDecloakHelper().Find(name).gameObject.SetActive(true);

                    Transform newBase = Selection.ThisShip.GetDecloakHelper().Find(name + "/Finisher/BasePosition");
                    ShipStand.transform.position = newBase.position;
                    ShipStand.transform.rotation = newBase.rotation;

                    obstaclesStayDetectorMovementTemplate = Selection.ThisShip.GetDecloakHelper().Find(name).GetComponentInChildren<ObstaclesStayDetectorForced>();

                }
                else
                {
                    if (!string.IsNullOrEmpty(SelectedDecloakHelper))
                    {
                        Selection.ThisShip.GetDecloakHelper().Find(SelectedDecloakHelper).gameObject.SetActive(false);
                    }

                    MovementTemplates.CurrentTemplate.gameObject.SetActive(true);
                    obstaclesStayDetectorMovementTemplate = MovementTemplates.CurrentTemplate.GetComponentInChildren<ObstaclesStayDetectorForced>();

                    PerfromDrag();
                }

                SelectedDecloakHelper = name;
            }
            else
            {
                if (name != "Forward")
                {
                    PerfromDrag();
                }
            }
        }

        private string GetNearestDecloakHelper(Vector3 point)
        {
            float minDistance = float.MaxValue;
            KeyValuePair<string, Vector3> nearestDecloakHelper = new KeyValuePair<string, Vector3>();

            foreach (var decloakDirection in AvailableDecloakDirections)
            {
                if (string.IsNullOrEmpty(nearestDecloakHelper.Key))
                {
                    nearestDecloakHelper = decloakDirection;
                    minDistance = Vector3.Distance(point, decloakDirection.Value);
                    continue;
                }
                else
                {
                    float currentDistance = Vector3.Distance(point, decloakDirection.Value);
                    if (currentDistance < minDistance)
                    {
                        nearestDecloakHelper = decloakDirection;
                        minDistance = currentDistance;
                    }
                }
            }

            return nearestDecloakHelper.Key;
        }

        public override void Pause()
        {
            inReposition = false;
        }

        public override void Resume()
        {
            inReposition = true;
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
                    ApplyDecloakRepositionLimits();
                }
            }
        }

        private void ApplyDecloakRepositionLimits()
        {
            Vector3 newPosition = Selection.ThisShip.InverseTransformPoint(ShipStand.transform.position);
            Vector3 fixedPositionRel = newPosition;

            if (newPosition.z > 0.5f)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, 0.5f);
            }

            if (newPosition.z < -0.5f)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, -0.5f);
            }

            if (newPosition.x > 0f)
            {
                fixedPositionRel = new Vector3(3, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = 1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles + new Vector3(0, 180, 0);
            }

            if (newPosition.x < 0f)
            {
                fixedPositionRel = new Vector3(-3, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = -1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles;
            }

            Vector3 helperPositionRel = Selection.ThisShip.InverseTransformPoint(MovementTemplates.CurrentTemplate.position);
            helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, helperPositionRel.z);

            if (helperPositionRel.z + 0.25f > fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - 0.25f);
            }

            if (helperPositionRel.z + 0.75f < fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - 0.75f);
            }

            Vector3 helperPositionAbs = Selection.ThisShip.TransformPoint(helperPositionRel);
            MovementTemplates.CurrentTemplate.position = helperPositionAbs;

            Vector3 fixedPositionAbs = Selection.ThisShip.TransformPoint(fixedPositionRel);
            ShipStand.transform.position = fixedPositionAbs;
        }

        public override void ProcessClick()
        {
            StopPlanning();
            TryConfirmDecloakPosition();
        }

        private void StartDecloakExecution(Ship.GenericShip ship)
        {
            Selection.ThisShip.ToggleShipStandAndPeg(false);
            MovementTemplates.CurrentTemplate.gameObject.SetActive(false);

            //Game.UI.HideSkipButton();

            Phases.StartTemporarySubPhase(
                "Decloak execution",
                typeof(DecloakExecutionSubPhase),
                CallBack
            );
        }

        private void CancelDecloak()
        {
            Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.DecloakAction));
            Selection.ThisShip.IsLandedOnObstacle = false;
            inReposition = false;
            MonoBehaviour.Destroy(ShipStand);
            Game.Movement.CollidedWith = null;

            MovementTemplates.HideLastMovementRuler();
            MovementTemplates.CurrentTemplate.gameObject.SetActive(true);

            PreviousSubPhase.Resume();
        }

        private void StopPlanning()
        {
            Roster.SetRaycastTargets(true);
            inReposition = false;
        }

        private void TryConfirmDecloakPosition()
        {
            obstaclesStayDetectorBase.ReCheckCollisionsStart();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsStart();
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
                StartDecloakExecution(Selection.ThisShip);
            }
            else
            {
                CancelDecloak();
            }

            HidePlanningTemplates();
        }

        private void HidePlanningTemplates()
        {
            Selection.ThisShip.GetDecloakHelper().Find(SelectedDecloakHelper).gameObject.SetActive(false);
        }

        private bool IsDecloakAllowed()
        {
            bool allow = true;

            if (obstaclesStayDetectorBase.OverlapsShipNow || obstaclesStayDetectorMovementTemplate.OverlapsShipNow)
            {
                Messages.ShowError("Cannot overlap another ship");
                allow = false;
            }
            else if (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow)
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

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            return false;
        }

        public override void SkipButton()
        {
            StopPlanning();
            CancelDecloak();
            HidePlanningTemplates();
        }

    }

    public class DecloakExecutionSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;

        private bool performingAnimation;

        private GameObject ShipStand;
        private float helperDirection;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Decloak execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartDecloakExecution();
        }

        private void StartDecloakExecution()
        {
            ShipStand = (PreviousSubPhase as DecloakPlanningSubPhase).ShipStand;
            helperDirection = (PreviousSubPhase as DecloakPlanningSubPhase).helperDirection;

            progressCurrent = 0;
            progressTarget = Vector3.Distance(Selection.ThisShip.GetPosition(), ShipStand.transform.position);

            Sounds.PlayFly();

            performingAnimation = true;
        }

        public override void Update()
        {
            if (performingAnimation) DoDecloakAnimation();
        }

        private void DoDecloakAnimation()
        {
            float progressStep = 0.5f * Time.deltaTime * Options.AnimationSpeed;
            progressStep = Mathf.Min(progressStep, progressTarget-progressCurrent);
            progressCurrent += progressStep;

            Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), ShipStand.transform.position, progressStep));
            Selection.ThisShip.RotateModelDuringBarrelRoll(progressCurrent / progressTarget, helperDirection);
            Selection.ThisShip.MoveUpwards(progressCurrent / progressTarget);
            if (progressCurrent >= progressTarget)
            {
                FinishDecloakAnimation();
            }
        }

        private void FinishDecloakAnimation()
        {
            performingAnimation = false;

            MonoBehaviour.Destroy(ShipStand);
            Game.Movement.CollidedWith = null;

            MovementTemplates.HideLastMovementRuler();
            MovementTemplates.CurrentTemplate.gameObject.SetActive(true);

            Selection.ThisShip.ToggleShipStandAndPeg(true);
            Selection.ThisShip.FinishPosition(delegate() { });

            Phases.FinishSubPhase(typeof(DecloakExecutionSubPhase));

            CallBack();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }

    }

}
