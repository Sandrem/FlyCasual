using Ship;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Board
{

    public class ShipShotDistanceInformation : GeneralShipDistanceInformation
    {
        public bool IsObstructedByAsteroid { get; private set; }
        public bool IsObstructedByBombToken { get; private set; }
        public List<GameObject> FiringLines { get; private set; }
        private System.Action CallBack;

        public bool InShotAngle { get; private set; }
        public bool InPrimaryArc { get; private set; }
        public bool InBullseyeArc { get; private set; }
        public bool InMobileArc { get; private set; }
		public bool InRearAuxArc { get; private set; }
        public bool InArc { get; private set; }
        public bool CanShootPrimaryWeapon { get; private set; }
        public bool CanShootTorpedoes { get; private set; }
        public bool CanShootMissiles { get; private set; }
        public bool CanShootCannon { get; private set; }
        public bool CanShootTurret { get; private set; }

        public new int Range
        {
            get
            {
                int distance = Mathf.Max(1, Mathf.CeilToInt(Distance / DISTANCE_1));

                if (OnRangeIsMeasured != null) OnRangeIsMeasured(ThisShip, AnotherShip, ChosenWeapon, ref distance);

                return distance;
            }
        }

        IShipWeapon ChosenWeapon { get; set; }

        private List<List<Vector3>> parallelPointsList;

        private int updatesCount = 0;

        //EVENTS
        public delegate void EventHandlerShipShipWeaponInt(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range);
        public static event EventHandlerShipShipWeaponInt OnRangeIsMeasured;

        public ShipShotDistanceInformation(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon = null) : base(thisShip, anotherShip)
        {
            ChosenWeapon = chosenWeapon ?? thisShip.PrimaryWeapon;
            
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
            Dictionary <string, Vector3> shootingPoints = (ThisShip.GetAllWeapons().Count(n => n.CanShootOutsideArc) == 0) ? ThisShip.ArcInfo.GetArcsPoints() : ThisShip.ShipBase.GetStandPoints();

            // TODO: change to use geometry instead of dots

            float distance = float.MaxValue;

            foreach (var pointThis in shootingPoints)
            {
                foreach (var pointAnother in AnotherShip.ShipBase.GetStandPoints())
                {
                    // TODO: check this part
                    Vector3 vectorToTarget = pointAnother.Value - pointThis.Value;
                    float angle = Vector3.SignedAngle(vectorToTarget, vectorFacing, Vector3.up);

                    // TODO: Different checks for primary arc and 360 arc

                    if (ChosenWeapon.CanShootOutsideArc || ChosenWeapon.Host.ArcInfo.InAttackAngle(pointThis.Key, angle))
                    {
                        InShotAngle = true;

                        if (ChosenWeapon.Host.ArcInfo.InArc(pointThis.Key, angle))
                        {
                            InArc = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.InPrimaryArc(pointThis.Key, angle))
                        {
                            InPrimaryArc = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.InBullseyeArc(pointThis.Key, angle))
                        {
                            InBullseyeArc = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.InMobileArc(pointThis.Key, angle))
                        {
                            InMobileArc = true;
                        }

						if (ChosenWeapon.Host.ArcInfo.InRearAuxArc(pointThis.Key, angle))
						{
							InRearAuxArc = true;
						}

                        if (ChosenWeapon.Host.ArcInfo.CanShootPrimaryWeapon(pointThis.Key, angle))
                        {
                            CanShootPrimaryWeapon = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.CanShootTorpedoes(pointThis.Key, angle))
                        {
                            CanShootTorpedoes = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.CanShootMissiles(pointThis.Key, angle))
                        {
                            CanShootMissiles = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.CanShootCannon(pointThis.Key, angle))
                        {
                            CanShootCannon = true;
                        }

                        if (ChosenWeapon.Host.ArcInfo.CanShootTurret(pointThis.Key, angle))
                        {
                            CanShootTurret = true;
                        }

                        distance = Vector3.Distance(pointThis.Value, pointAnother.Value);
                        if (distance < Distance - PRECISION)
                        {
                            parallelPointsList = new List<List<Vector3>>();

                            Distance = distance;

                            ThisShipNearestPoint = pointThis.Value;
                            AnotherShipNearestPoint = pointAnother.Value;

                            parallelPointsList.Add(new List<Vector3>() { pointThis.Value, pointAnother.Value });
                        }
                        else if (Mathf.Abs(Distance - distance) < PRECISION)
                        {
                            parallelPointsList.Add(new List<Vector3>() { pointThis.Value, pointAnother.Value });
                        }
                    }
                }
            }
        }

        public void CheckFirelineCollisions(System.Action callBack)
        {
            if (DebugManager.DebugBoard) Debug.Log("Obstacle checker is launched: " + ThisShip + " vs " + AnotherShip);

            FiringLines = new List<GameObject>();
            GameObject prefab = (GameObject)Resources.Load("Prefabs/FiringLine", typeof(GameObject));
            float SIZE_ANY = 91.44f;

            foreach (var parallelPoints in parallelPointsList)
            {
                GameObject FiringLine = MonoBehaviour.Instantiate(prefab, parallelPoints[0], Quaternion.LookRotation(parallelPoints[1]-parallelPoints[0]), BoardManager.GetBoard());
                FiringLine.transform.localScale = new Vector3(1, 1, Vector3.Distance(parallelPoints[0], parallelPoints[1]) * SIZE_ANY / 100);
                FiringLine.SetActive(true);
                FiringLine.GetComponentInChildren<ObstaclesFiringLineDetector>().PointStart = parallelPoints[0];
                FiringLine.GetComponentInChildren<ObstaclesFiringLineDetector>().PointEnd = parallelPoints[1];
                FiringLines.Add(FiringLine);
            }

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
            bool isObstructedByAsteroid = false;
            bool isObstructedByBombToken = false;

            List<GameObject> FiringLinesCopy = new List<GameObject>(FiringLines);
            foreach (var firingLine in FiringLinesCopy)
            {
                ObstaclesFiringLineDetector obstacleDetector = firingLine.GetComponentInChildren<ObstaclesFiringLineDetector>();
                if (obstacleDetector.IsObstructedByAsteroid)
                {
                    isObstructedByAsteroid = true;
                    FiringLines.Remove(firingLine);
                }
                if (obstacleDetector.IsObstructedByBombToken)
                {
                    isObstructedByBombToken = true;
                    FiringLines.Remove(firingLine);
                }
            }

            // TODO: If asteroid or bomb - ask what to use
            if (FiringLines.Count == 0)
            {
                if (isObstructedByAsteroid) IsObstructedByAsteroid = true;
                if (isObstructedByBombToken) IsObstructedByBombToken = true;
            }
            else
            {
                IsObstructedByAsteroid = false;
                IsObstructedByBombToken = false;

                ThisShipNearestPoint = FiringLines[0].GetComponentInChildren<ObstaclesFiringLineDetector>().PointStart;
                AnotherShipNearestPoint = FiringLines[0].GetComponentInChildren<ObstaclesFiringLineDetector>().PointEnd;
            }

            //TODO: middle result

            foreach (var fireline in FiringLinesCopy)
            {
                MonoBehaviour.Destroy(fireline);
            }

            CallBack();
        }

    }

}
