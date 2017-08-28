using Ship;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Board
{

    public class ShipShotDistanceInformation : GeneralShipDistanceInformation
    {
        public bool IsObstructed { get; private set; }
        public List<GameObject> FiringLines { get; private set; }
        private System.Action CallBack;

        public bool InShotAngle { get; private set; }
        public bool InPrimaryArc { get; private set; }
        public bool InArc { get; private set; }

        IShipWeapon ChosenWeapon { get; set; }

        private List<List<Vector3>> parallelPointsList;

        private int updatesCount = 0;

        public ShipShotDistanceInformation(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon) : base(thisShip, anotherShip)
        {
            Debug.Log("ShipShotDistanceInformation is called");

            ChosenWeapon = chosenWeapon;
            CalculateFields();
        }

        protected override void CalculateFields()
        {
            float PRECISION = 0.000008f;

            Distance = float.MaxValue;
            Vector3 vectorFacing = ThisShip.GetFrontFacing();
            InArc = false;

            parallelPointsList = new List<List<Vector3>>();

            // TODO: another types of primaty arcs
            Dictionary <string, Vector3> shootingPoints = (!ChosenWeapon.CanShootOutsideArc) ? ThisShip.GetStandFrontPoints() : ThisShip.GetStandPoints();

            // TODO: change to use geometry insted of dots
            foreach (var objThis in shootingPoints)
            {
                foreach (var objAnother in AnotherShip.GetStandPoints())
                {

                    // TODO: check this part
                    Vector3 vectorToTarget = objAnother.Value - objThis.Value;
                    float angle = Mathf.Abs(Vector3.SignedAngle(vectorToTarget, vectorFacing, Vector3.up));

                    if (ChosenWeapon.CanShootOutsideArc || ChosenWeapon.Host.ArcInfo.InAttackAngle(angle))
                    {
                        InShotAngle = true;

                        if (ChosenWeapon.Host.ArcInfo.InArc(angle))
                        {
                            InArc = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.InPrimaryArc(angle))
                        {
                            InPrimaryArc = true;
                        }

                        float distance = Vector3.Distance(objThis.Value, objAnother.Value);
                        if (distance < Distance - PRECISION)
                        {
                            parallelPointsList = new List<List<Vector3>>();

                            Distance = distance;

                            ThisShipNearestPoint = objThis.Value;
                            AnotherShipNearestPoint = objAnother.Value;

                            parallelPointsList.Add(new List<Vector3>() { objThis.Value, objAnother.Value });
                        }
                        else if (Mathf.Abs(Distance - distance) < PRECISION)
                        {
                            parallelPointsList.Add(new List<Vector3>() { objThis.Value, objAnother.Value });
                        }
                    }
                }
            }

            Debug.Log("InShotAngle: " + InShotAngle);
            Debug.Log("InArc: " + InArc);
            Debug.Log("InPrimaryArc: " + InPrimaryArc);
        }

        public void CheckFirelineCollisions(System.Action callBack)
        {
            if (DebugManager.DebugBoard) Debug.Log("Obstacle checker is launched: " + ThisShip + " vs " + AnotherShip);

            FiringLines = new List<GameObject>();
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            float SIZE_ANY = 91.44f;

            foreach (var parallelPoints in parallelPointsList)
            {
                GameObject FiringLine = MonoBehaviour.Instantiate(Game.PrefabsList.FiringLine, parallelPoints[0], Quaternion.LookRotation(parallelPoints[1]-parallelPoints[0]), BoardManager.GetBoard());
                FiringLine.transform.localScale = new Vector3(1, 1, Vector3.Distance(parallelPoints[0], parallelPoints[1]) * SIZE_ANY / 100);
                FiringLine.SetActive(true);
                FiringLine.GetComponentInChildren<ObstaclesFiringLineDetector>().PointStart = parallelPoints[0];
                FiringLine.GetComponentInChildren<ObstaclesFiringLineDetector>().PointEnd = parallelPoints[1];
                FiringLines.Add(FiringLine);
            }

            Game.Movement.FuncsToUpdate.Add(UpdateColisionDetection);

            CallBack = callBack;
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

            List<GameObject> FiringLinesCopy = new List<GameObject>(FiringLines);
            foreach (var firingLine in FiringLinesCopy)
            {
                if (firingLine.GetComponentInChildren<ObstaclesFiringLineDetector>().IsObstructed)
                {
                    FiringLines.Remove(firingLine);
                }
            }

            if (FiringLines.Count == 0)
            {
                IsObstructed = true;
            }
            else
            {
                IsObstructed = false;

                ThisShipNearestPoint = FiringLines[0].GetComponentInChildren<ObstaclesFiringLineDetector>().PointStart;
                AnotherShipNearestPoint = FiringLines[0].GetComponentInChildren<ObstaclesFiringLineDetector>().PointEnd;
            }

            //TODO: middle result

            foreach (var fireline in FiringLinesCopy)
            {
                MonoBehaviour.Destroy(fireline);
            }

            //TODO: Rework
            Combat.IsObstructed = IsObstructed;

            CallBack();
        }

    }

}
