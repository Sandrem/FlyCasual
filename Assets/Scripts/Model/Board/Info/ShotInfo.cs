using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Arcs;
using Upgrade;

namespace BoardTools
{

    public class ShotInfo : GenericShipDistanceInfo
    {
        public bool IsShotAvailable { get; private set; }
        public bool InArc { get { return InArcInfo.Any(n => n.Value == true); } }
        public bool InPrimaryArc { get { return InArcByType(ArcTypes.Primary); } }

        public RangeHolder NearestFailedDistance;

        private Dictionary<ArcTypes, bool> InArcInfo { get; set; }

        public bool IsObstructedByAsteroid { get; private set; }
        public bool IsObstructedByBombToken { get; private set; }

        public IShipWeapon Weapon { get; private set; }

        public float DistanceReal { get { return MinDistance.DistanceReal; } }

        private Action CallBack;
        private int updatesCount;
        private GameObject FiringLine;

        public new int Range
        {
            get
            {
                int range = (MinDistance != null) ? MinDistance.Range : NearestFailedDistance.Range;
                if (OnRangeIsMeasured != null) OnRangeIsMeasured(Ship1, Ship2, Weapon, ref range);
                return range;
            }
        }

        //EVENTS
        public delegate void EventHandlerShipShipWeaponInt(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range);
        public static event EventHandlerShipShipWeaponInt OnRangeIsMeasured;

        public ShotInfo(GenericShip ship1, GenericShip ship2, IShipWeapon weapon) : base(ship1, ship2)
        {
            CheckRange();
        }

        private void CheckRange()
        {
            InArcInfo = new Dictionary<ArcTypes, bool>();

            foreach (var arc in Ship1.ArcInfo.Arcs)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(Ship1, Ship2, arc);
                InArcInfo.Add(arc.ArcType, shotInfoArc.InArc);

                WeaponTypes weaponType = (Weapon is GenericSecondaryWeapon) ? (Weapon as GenericSecondaryWeapon).WeaponType : WeaponTypes.PrimaryWeapon;

                if (arc.ShotPermissions.CanShootByWeaponType(weaponType))
                {
                    if (shotInfoArc.IsShotAvailable)
                    {
                        if (IsShotAvailable == false)
                        {
                            MinDistance = shotInfoArc.MinDistance;
                        }
                        else
                        {
                            if (shotInfoArc.MinDistance.DistanceReal < MinDistance.DistanceReal) MinDistance = shotInfoArc.MinDistance;
                        }

                        IsShotAvailable = true;
                    }

                    if (NearestFailedDistance == null)
                    {
                        NearestFailedDistance = shotInfoArc.MinDistance;
                    }
                    else if (shotInfoArc.MinDistance.DistanceReal < NearestFailedDistance.DistanceReal)
                    {
                        NearestFailedDistance = shotInfoArc.MinDistance;
                    }
                }
            }
        }

        public bool InArcByType(ArcTypes arcType)
        {
            if (!InArcInfo.ContainsKey(arcType)) return false;

            return InArcInfo[arcType];
        }

        // TODO: CHANGE

        public void CheckObstruction(System.Action callBack)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/FiringLine", typeof(GameObject));
            float SIZE_ANY = 91.44f;

            FiringLine = MonoBehaviour.Instantiate(prefab, Board.GetBoard());
            FiringLine.transform.position = MinDistance.Point1;
            FiringLine.transform.LookAt(MinDistance.Point2);
            FiringLine.transform.localScale = new Vector3(1, 1, Vector3.Distance(MinDistance.Point1, MinDistance.Point2) * SIZE_ANY / 100);
            FiringLine.SetActive(true);
            FiringLine.GetComponentInChildren<ObstaclesFiringLineDetector>().PointStart = MinDistance.Point1;
            FiringLine.GetComponentInChildren<ObstaclesFiringLineDetector>().PointEnd = MinDistance.Point2;

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
            ObstaclesFiringLineDetector obstacleDetector = FiringLine.GetComponentInChildren<ObstaclesFiringLineDetector>();
            if (obstacleDetector.IsObstructedByAsteroid)
            {
                IsObstructedByAsteroid = true;
            }
            if (obstacleDetector.IsObstructedByBombToken)
            {
                IsObstructedByBombToken = true;
            }

            MonoBehaviour.Destroy(FiringLine);

            CallBack();
        }
    }
}


