using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: Decloak by pilot skill

namespace RulesList
{
    public class CloakRule
    {

        public CloakRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Ship.GenericShip.OnTokenIsAssignedGlobal += CheckIsCloakTokenAssigned;
            Ship.GenericShip.OnTokenIsSpentGlobal += CheckIsCloakTokenSpent;
        }

        private void CheckIsCloakTokenAssigned(Ship.GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.CloakToken))
            {
                PerformCloak(ship);
            }
        }

        private void CheckIsCloakTokenSpent(Ship.GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.CloakToken))
            {
                RemoveCloakEffects(ship);
            }
        }

        private void PerformCloak(Ship.GenericShip ship)
        {
            //TODO: filter action in window instead

            ActionsList.GenericAction cloakAction = null;
            foreach (var action in ship.PrintedActions)
            {
                if (action.GetType() == typeof(ActionsList.CloakAction))
                {
                    cloakAction = action;
                    break;
                }
            }
            ship.PrintedActions.Remove(cloakAction);

            ship.ChangeAgilityBy(+2);
            ship.OnTryPerformAttack += CannotAttackWhileCloaked;
            Phases.OnActivationPhaseStart += RegisterAskDecloak;
        }

        private void RemoveCloakEffects(Ship.GenericShip ship)
        {
            ship.PrintedActions.Add(new ActionsList.CloakAction());
            ship.ChangeAgilityBy(-2);
            ship.OnTryPerformAttack -= CannotAttackWhileCloaked;
            Phases.OnActivationPhaseStart -= RegisterAskDecloak;
        }

        private void CannotAttackWhileCloaked(ref bool result)
        {
            result = false;
        }

        private void RegisterAskDecloak()
        {
            foreach (Ship.GenericShip ship in Roster.AllShips.Values)
            {
                if (ship.HasToken(typeof(Tokens.CloakToken)))
                {
                    Triggers.RegisterTrigger(new Trigger
                    {
                        Name = "Decloak",
                        TriggerType = TriggerTypes.OnActionSubPhaseStart,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = delegate
                        {
                            Selection.ThisShip = ship;
                            AskDecloak();
                        }
                    });
                }
            }
        }

        private void AskDecloak()
        {
            //Phases.CurrentSubPhase.Pause();

            Phases.StartTemporarySubPhaseOld(
                "Decloak Decision",
                typeof(SubPhases.DecloakDecisionSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(SubPhases.DecloakDecisionSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }

    }
}

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

            DefaultDecision = "No";

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

        public GameObject ShipStand;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        Dictionary<string, Vector3> AvailableDecloakDirections = new Dictionary<string, Vector3>();
        public string SelectedDecloakHelper;

        public float helperDirection;
        public bool inReposition;

        public override void Start()
        {
            Name = "Decloak planning";
            IsTemporary = true;
            UpdateHelpInfo();

            //Game.UI.ShowSkipButton();

            StartDecloakPlanning();
        }

        public void StartDecloakPlanning()
        {
            GameObject prefab = (GameObject)Resources.Load(Selection.ThisShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            ShipStand = MonoBehaviour.Instantiate(prefab, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), Board.BoardManager.GetBoard());
            ShipStand.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = Selection.ThisShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
            ShipStand.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorBase = ShipStand.GetComponentInChildren<ObstaclesStayDetectorForced>();

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

            Phases.StartTemporarySubPhaseOld(
                "Decloak execution",
                typeof(DecloakExecutionSubPhase),
                CallBack
            );
        }

        private void CancelDecloak()
        {
            Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.CloakAction));
            Selection.ThisShip.IsLandedOnObstacle = false;
            inReposition = false;
            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
                StartDecloakExecution(Selection.ThisShip);
            }
            else
            {
                CancelDecloak();
            }

            HidePlanningTemplates();
        }

        private void CheckMines()
        {
            foreach (var mineCollider in obstaclesStayDetectorMovementTemplate.OverlapedMinesNow)
            {
                GameObject mineObject = mineCollider.transform.parent.gameObject;
                if (!Selection.ThisShip.MinesHit.Contains(mineObject)) Selection.ThisShip.MinesHit.Add(mineObject);
            }
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
            progressStep = Mathf.Min(progressStep, progressTarget - progressCurrent);
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

            Selection.ThisShip.SpendToken(typeof(Tokens.CloakToken), CallBack);
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

