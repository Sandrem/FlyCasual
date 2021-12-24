using BoardTools;
using Players;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;

namespace Obstacles
{
    public abstract class GenericObstacle : ITargetLockable, IBoardObject
    {
        public string Name { get; set; }
        public string ShortName { get; protected set; }
        public bool IsPlaced { get; set; }
        public GameObject ObstacleGO { get; set; }
        public List<GenericToken> Tokens { get; private set; } = new List<GenericToken>();

        private MeshCollider collider;
        public MeshCollider Collider => collider;
        public BoardObjectType BoardObjectType => BoardObjectType.Obstacle;

        public GenericObstacle(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public abstract string GetTypeName { get; }

        public abstract void OnHit(GenericShip ship);

        public void OnLanded(GenericShip ship)
        {
            if (Editions.Edition.Current.RuleSet.GetType() == typeof(Editions.RuleSets.RuleSet25) || this is Asteroid)
            {
                ship.OnTryPerformAttack += DenyAttack;
            }

            if (Editions.Edition.Current.RuleSet.GetType() == typeof(Editions.RuleSets.RuleSet25))
            {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " landed on an obstacle during movement, their action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public abstract void OnShotObstructedExtra(GenericShip attacker, GenericShip defender);

        public void Spawn(string name, Transform obstacleHolder)
        {
            GameObject obstacleModelPrefab = Resources.Load<GameObject>(string.Format("Prefabs/Obstacles/{0}/{1}", GetTypeName, Name));
            ObstacleGO = GameObject.Instantiate<GameObject>(obstacleModelPrefab, obstacleHolder);
            collider = ObstacleGO.GetComponentInChildren<MeshCollider>();
            Name = name;
            ObstacleGO.name = name;
            ObstacleGO.transform.Find("default").name = name;
            Board.RegisterObstacle(this);
        }

        // ITargetLockable

        public int GetRangeToShip(GenericShip fromShip)
        {
            ShipObstacleDistance dist = new ShipObstacleDistance(fromShip, this);
            return dist.Range;
        }

        public void AssignToken(RedTargetLockToken token, Action callback)
        {
            Tokens.Add(token);
            //TODO: Show token on obstacle
            callback();
        }

        public List<char> GetTargetLockLetterPairsOn(ITargetLockable targetShip)
        {
            return Tokens
                .Where(t => (t as RedTargetLockToken).OtherTargetLockTokenOwner == targetShip)
                .Select(t => (t as RedTargetLockToken).Letter)
                .ToList();
        }

        public GenericTargetLockToken GetAnotherToken(Type oppositeType, char letter)
        {
            return Tokens.FirstOrDefault(t => (t as RedTargetLockToken).Letter == letter) as GenericTargetLockToken;
        }

        public void RemoveToken(GenericToken token)
        {
            Tokens.Remove(token);
            //TODO: Hide token from obstacle
        }

        public abstract void AfterObstacleRoll(GenericShip ship, DieSide side, Action callback);

        private void DenyAttack(ref bool result, List<string> stringList)
        {
            if (Selection.ThisShip.ObstaclesLanded.Contains(this) && !Selection.ThisShip.CanAttackWhileLandedOnObstacle())
            {
                result = false;
                Selection.ThisShip.CallCheckObstacleDenyAttack(this, ref result);
                if (!result) stringList.Add(Selection.ThisShip.PilotInfo.PilotName + " landed on an obstacle and cannot attack");
            }
        }
    }
}
