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
        public bool inReposition;

        Dictionary<string, Vector3> AvailableBoostDirections = new Dictionary<string, Vector3>();
        private string selectedBoostHelper;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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

            ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), BoardManager.GetBoard());
            ShipStand.transform.Find("ShipStandTemplate").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = Selection.ThisShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
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

            if (selectedBoostHelper != name)
            {
                if (!string.IsNullOrEmpty(selectedBoostHelper))
                {
                    Selection.ThisShip.GetBoosterHelper().Find(selectedBoostHelper).gameObject.SetActive(false);
                }
                Selection.ThisShip.GetBoosterHelper().Find(name).gameObject.SetActive(true);

                Transform newBase = Selection.ThisShip.GetBoosterHelper().Find(name + "/Finisher/BasePosition");
                ShipStand.transform.position = newBase.position;
                ShipStand.transform.rotation = newBase.rotation;

                selectedBoostHelper = name;
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
            TryConfirmPosition(Selection.ThisShip);
        }

        private bool TryConfirmPosition(Ship.GenericShip ship)
        {
            StopPlanning();

            bool result = false;

            result = TryConfirmBoostPosition(ship);

            if (result)
            {
                StartBoostExecution(ship);
            }
            else
            {
                CancelBoost();
            }

            return result;
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
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            PreviousSubPhase.Resume();
        }

        private void StopPlanning()
        {
            inReposition = false;

            Selection.ThisShip.GetBoosterHelper().Find(selectedBoostHelper).gameObject.SetActive(false);
            MonoBehaviour.Destroy(ShipStand);

            Roster.SetRaycastTargets(true);
        }

        private bool TryConfirmBoostPosition(Ship.GenericShip ship)
        {
            bool allow = true;

            /*if (Game.Movement.CollidedWith != null)
            {
                Messages.ShowError("Cannot collide with another ships");
                allow = false;
            }
            else if (ship.IsLandedOnObstacle)
            {
                Messages.ShowError("Cannot land on Asteroid");
                allow = false;
            }
            else if (!BoardManager.ShipStandIsInside(ShipStand, BoardManager.BoardTransform.Find("Playmat")))
            {
                Messages.ShowError("Cannot leave the battlefield");
                allow = false;
            }*/

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
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Boost execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostExecution();
        }

        private void StartBoostExecution()
        {
            Movement.GenericMovement boostMovement = new Movement.StraightBoost(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.None);

            //TEMPORARY
            boostMovement.Perform();
            Sounds.PlayFly();
        }

        private void FinishBoostAnimation()
        {
            Selection.ThisShip.FinishPosition(delegate() { });

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
