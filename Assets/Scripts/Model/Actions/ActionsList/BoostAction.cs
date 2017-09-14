using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;

namespace ActionsList
{

    public class BoostAction : GenericAction
    {
        public BoostAction() {
            Name = "Boost";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/BoostAction.png";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();
            Phases.StartTemporarySubPhase(
                "Boost",
                typeof(SubPhases.BoostPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

    }

}

namespace SubPhases
{

    public class BoostPlanningSubPhase : GenericSubPhase
    {
        public GameObject ShipStand;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        public bool inReposition;

        private int updatesCount = 0;

        Dictionary<string, Vector3> AvailableBoostDirections = new Dictionary<string, Vector3>();
        public string SelectedBoostHelper;

        public override void Start()
        {
            Name = "Boost planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostPlanning();
        }

        public void StartBoostPlanning()
        {
            foreach (Transform boostHelper in Selection.ThisShip.GetBoosterHelper())
            {
                AvailableBoostDirections.Add(boostHelper.name, boostHelper.Find("Finisher").position);
            }

            GameObject prefab = (GameObject)Resources.Load(Selection.ThisShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            ShipStand = MonoBehaviour.Instantiate(prefab, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), BoardManager.GetBoard());
            ShipStand.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = Selection.ThisShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
            obstaclesStayDetectorBase = ShipStand.GetComponentInChildren<ObstaclesStayDetectorForced>();
            Roster.SetRaycastTargets(false);

            inReposition = true;
        }

        public override void Update()
        {
            if (inReposition)
            {
                SelectBoosterHelper();
            }
        }

        public override void Pause()
        {

        }

        public override void Resume()
        {

        }

        private void SelectBoosterHelper()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                ShowNearestBoosterHelper(GetNearestBoosterHelper(new Vector3(hit.point.x, 0f, hit.point.z)));
            }
        }

        private void ShowNearestBoosterHelper(string name)
        {
            // TODO: hide template

            if (SelectedBoostHelper != name)
            {
                if (!string.IsNullOrEmpty(SelectedBoostHelper))
                {
                    Selection.ThisShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(false);
                }
                Selection.ThisShip.GetBoosterHelper().Find(name).gameObject.SetActive(true);

                Transform newBase = Selection.ThisShip.GetBoosterHelper().Find(name + "/Finisher/BasePosition");
                ShipStand.transform.position = newBase.position;
                ShipStand.transform.rotation = newBase.rotation;

                obstaclesStayDetectorMovementTemplate = Selection.ThisShip.GetBoosterHelper().Find(name).GetComponentInChildren<ObstaclesStayDetectorForced>();

                SelectedBoostHelper = name;
            }
        }

        private string GetNearestBoosterHelper(Vector3 point)
        {
            float minDistance = float.MaxValue;
            KeyValuePair<string, Vector3> nearestBoosterHelper = new KeyValuePair<string, Vector3>();

            foreach (var boostDirection in AvailableBoostDirections)
            {
                if (string.IsNullOrEmpty(nearestBoosterHelper.Key))
                {
                    nearestBoosterHelper = boostDirection;
                    minDistance = Vector3.Distance(point, boostDirection.Value);
                    continue;
                }
                else
                {
                    float currentDistance = Vector3.Distance(point, boostDirection.Value);
                    if (currentDistance < minDistance)
                    {
                        nearestBoosterHelper = boostDirection;
                        minDistance = currentDistance;
                    }
                }
            }

            return nearestBoosterHelper.Key;
        }

        public override void ProcessClick()
        {
            StopPlanning();
            TryConfirmBoostPosition();
        }

        private void StartBoostExecution(Ship.GenericShip ship)
        {
            Phases.StartTemporarySubPhase(
                "Boost execution",
                typeof(BoostExecutionSubPhase),
                CallBack
            );
        }

        private void CancelBoost()
        {
            Selection.ThisShip.IsLandedOnObstacle = false;
            inReposition = false;
            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            PreviousSubPhase.Resume();
        }

        private void StopPlanning()
        {
            inReposition = false;
        }

        private void HidePlanningTemplates()
        {
            Selection.ThisShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(false);
            MonoBehaviour.Destroy(ShipStand);

            Roster.SetRaycastTargets(true);
        }

        private void TryConfirmBoostPosition()
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

            if (IsBoostAllowed())
            {
                CheckMines();
                StartBoostExecution(Selection.ThisShip);
            }
            else
            {
                CancelBoost();
            }

            HidePlanningTemplates();
        }

        private void CheckMines()
        {
            foreach (var mineCollider in obstaclesStayDetectorMovementTemplate.OverlapedMines)
            {
                GameObject mineObject = mineCollider.transform.parent.gameObject;
                if (!Selection.ThisShip.MinesHit.Contains(mineObject)) Selection.ThisShip.MinesHit.Add(mineObject);
            }
        }

        private bool IsBoostAllowed()
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

    }

    public class BoostExecutionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Boost execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostExecution();
        }

        private void StartBoostExecution()
        {
            Movement.GenericMovement boostMovement;
            switch ((PreviousSubPhase as BoostPlanningSubPhase).SelectedBoostHelper)
            {
                case "Straight1":
                    boostMovement = new Movement.StraightBoost(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.None);
                    break;
                case "Bank1Left":
                    boostMovement = new Movement.BankBoost(1, Movement.ManeuverDirection.Left, Movement.ManeuverBearing.Bank, Movement.ManeuverColor.None);
                    break;
                case "Bank1Right":
                    boostMovement = new Movement.BankBoost(1, Movement.ManeuverDirection.Right, Movement.ManeuverBearing.Bank, Movement.ManeuverColor.None);
                    break;
                default:
                    boostMovement = new Movement.StraightBoost(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.None);
                    break;
            }

            MovementTemplates.ApplyMovementRuler(Selection.ThisShip, boostMovement);

            //TEMPORARY
            boostMovement.Perform();
            Sounds.PlayFly();
        }

        private void FinishBoostAnimation()
        {
            Selection.ThisShip.FinishPosition(FinishBoostAnimationPart2);
        }

        private void FinishBoostAnimationPart2()
        {
            Phases.FinishSubPhase(typeof(BoostExecutionSubPhase));
            CallBack();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
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
