using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace BoardTools
{

    public class ShotInfo : GenericShipDistanceInfo
    {
        public bool IsShotAvailable { get; private set; }
        public bool InArc { get { return InArcInfo.Any(n => n.Value == true); } }
        public bool InPrimaryArc { get { return InArcByType(ArcType.Front); } }

        public RangeHolder NearestFailedDistance;

        private Dictionary<GenericArc, bool> InArcInfo { get; set; }
        private Dictionary<GenericArc, bool> InSectorInfo { get; set; }

        public bool IsObstructedByAsteroid { get; private set; }
        public bool IsObstructedByBombToken { get; private set; }

        public IShipWeapon Weapon { get; private set; }

        public float DistanceReal { get { return MinDistance.DistanceReal; } }

        private Action CallBack;
        private int updatesCount;
        private GameObject FiringLine;

        public List<GenericArc> ShotAvailableFromArcs { get; private set; }

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

        internal bool InArcByType(object bullseye)
        {
            throw new NotImplementedException();
        }

        public static event EventHandlerShipShipWeaponInt OnRangeIsMeasured;

        /// <summary>
        /// Creates a new ShotInfo from ship1 (attacker) to ship2 (defender)
        /// </summary>
        /// <param name="ship1">The attacker.</param>
        /// <param name="ship2">The defender.</param>
        /// <param name="weapon">The weapon. If null, ship1's primary weapon will be used.</param>
        public ShotInfo(GenericShip ship1, GenericShip ship2, IShipWeapon weapon) : base(ship1, ship2)
        {
            Weapon = weapon ?? ship1.PrimaryWeapons.First();

            ShotAvailableFromArcs = new List<GenericArc>();

            CheckRange();
            CheckFailed();
        }

        // TODO: Temporary, Remove later
        public ShotInfo(GenericShip ship1, GenericShip ship2, List<PrimaryWeaponClass> weapons) : base(ship1, ship2)
        {
            Weapon = (weapons != null) ? weapons.First() : ship1.PrimaryWeapons.First();

            ShotAvailableFromArcs = new List<GenericArc>();

            CheckRange();
            CheckFailed();
        }

        private void CheckRange()
        {
            InArcInfo = new Dictionary<GenericArc, bool>();
            InSectorInfo = new Dictionary<GenericArc, bool>();

            List<ArcType> WeaponArcRestrictions = new List<ArcType>(Weapon.WeaponInfo.ArcRestrictions);
            if (WeaponArcRestrictions.Contains(ArcType.DoubleTurret))
            {
                WeaponArcRestrictions.RemoveAll(a => a == ArcType.DoubleTurret);
                WeaponArcRestrictions.Add(ArcType.SingleTurret);
            }

            foreach (var arc in Ship1.ArcsInfo.Arcs)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(Ship1, Ship2, arc);
                InArcInfo.Add(arc, shotInfoArc.InArc);
            }

            List<GenericArc> sectorsAndTurrets = new List<GenericArc>();
            sectorsAndTurrets.AddRange(Ship1.SectorsInfo.Arcs);
            sectorsAndTurrets.AddRange(Ship1.ArcsInfo.Arcs.Where(a => a.ArcType == ArcType.SingleTurret));
            foreach (var arc in sectorsAndTurrets)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(Ship1, Ship2, arc);
                InSectorInfo.Add(arc, shotInfoArc.InArc);

                if (WeaponArcRestrictions.Count > 0 && !WeaponArcRestrictions.Contains(arc.ArcType))
                    continue;

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

                    if (!(arc is ArcBullseye)) ShotAvailableFromArcs.Add(arc);
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

            // For 360 arcs
            if (Weapon.WeaponInfo.CanShootOutsideArc)
            {
                DistanceInfo distInfo = new DistanceInfo(Ship1, Ship2);
                
                if (distInfo.Range < 4)
                {
                    MinDistance = distInfo.MinDistance;
                    IsShotAvailable = true;
                }
                else
                {
                    NearestFailedDistance = distInfo.MinDistance;
                }
            }
        }

        private void CheckFailed()
        {
            if (MinDistance == null) MinDistance = NearestFailedDistance;
        }

        public bool InArcByType(ArcType arcType)
        {
            Dictionary<GenericArc, bool> filteredInfo = InArcInfo.Where(a => a.Key.ArcType == arcType).ToDictionary(a => a.Key, a => a.Value);
            if (filteredInfo == null || filteredInfo.Count == 0) return false;

            foreach (var arcInfo in filteredInfo)
            {
                if (arcInfo.Value) return true;
            }

            return false;
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


