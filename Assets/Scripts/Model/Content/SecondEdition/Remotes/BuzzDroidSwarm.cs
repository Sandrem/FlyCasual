using BoardTools;
using Players;
using Remote;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Remote
{
    public class BuzzDroidSwarm : GenericRemote
    {
        public BuzzDroidSwarm(GenericPlayer owner) : base(owner)
        {
            RemoteInfo = new RemoteInfo(
                "Buzz Droid Swarm",
                0, 3, 1,
                "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/38/Remote_BuzzDroidSwarm.png",
                typeof(Abilities.SecondEdition.BuzzDroidSwarmAbiliy)
            );
        }

        public override Dictionary<string, Vector3> BaseEdges
        {
            get
            {
                return new Dictionary<string, Vector3>()
                {
                    { "R0", new Vector3(-1.03f, 0f, 4.11f) },
                    { "R1", new Vector3(-1.44f, 0f, 3.77f) },
                    { "R2", new Vector3(-1.58f, 0f, 3.69f) },
                    { "R3", new Vector3(-1.67f, 0f, 3.53f) },
                    { "R4", new Vector3(-1.67f, 0f, 0f) },
                    { "R5", new Vector3(-1.61f, 0f, -0.17f) },
                    { "R6", new Vector3(-1.42f, 0f, -0.26f) },
                    { "R7", new Vector3(-1.21f, 0f, -0.2f) },
                    { "R8", new Vector3(-1.13f, 0f, 0f) },
                    { "R9", new Vector3(1.03f, 0f, 4.11f) },
                    { "R10", new Vector3(1.44f, 0f, 3.77f) },
                    { "R11", new Vector3(1.58f, 0f, 3.69f) },
                    { "R12", new Vector3(1.67f, 0f, 3.53f) },
                    { "R13", new Vector3(1.67f, 0f, 0f) },
                    { "R14", new Vector3(1.61f, 0f, -0.17f) },
                    { "R15", new Vector3(1.42f, 0f, -0.26f) },
                    { "R16", new Vector3(1.21f, 0f, -0.2f) },
                    { "R17", new Vector3(1.13f, 0f, 0f) },
                };
            }
        }

        public override bool HasCombatActivation
        {
            get
            {
                return !IsAttackPerformed && Board.GetShipsAtRange(this, new Vector2(0, 0), Team.Type.Enemy).Count > 0;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BuzzDroidSwarmAbiliy : GenericAbility
    {
        private Dictionary<Renderer, bool> RenderersOldState;
        private ShipPositionInfo OldPosition;
        private GenericShip SufferedShip;

        public override void ActivateAbility()
        {
            GenericShip.OnPositionFinishGlobal += CheckRemoteOverlapping;
            HostShip.OnCombatActivation += RegisterDealDamageToEnemyShipsAtRange;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnPositionFinishGlobal -= CheckRemoteOverlapping;
            HostShip.OnCombatActivation -= RegisterDealDamageToEnemyShipsAtRange;
        }

        private void RegisterDealDamageToEnemyShipsAtRange(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, DealDamageToEnemyShipsAtRange);
        }

        private void DealDamageToEnemyShipsAtRange(object sender, EventArgs e)
        {
            List<GenericShip> enemyShipsAtRange = Board.GetShipsAtRange(HostShip, new Vector2(0, 0), Team.Type.Enemy);

            DealCritsRecursive(enemyShipsAtRange);
        }

        private void DealCritsRecursive(List<GenericShip> enemyShipsAtRange)
        {
            if (enemyShipsAtRange.Count > 0)
            {
                GenericShip shipToDealDamage = enemyShipsAtRange.First();
                enemyShipsAtRange.Remove(shipToDealDamage);

                shipToDealDamage.Damage.TryResolveDamage(
                    0, 1,
                    new DamageSourceEventArgs()
                    {
                        DamageType = DamageTypes.CardAbility,
                        Source = HostShip
                    },
                    delegate
                    {
                        DealCritsRecursive(enemyShipsAtRange);
                    }
                );
            }
            else
            {
                HostShip.IsAttackPerformed = true;
                Triggers.FinishTrigger();
            }
        }

        private void CheckRemoteOverlapping(GenericShip ship)
        {
            // Only for real ships
            if (ship is GenericRemote) return;

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            if (ship.RemotesOverlapped.Contains(HostShip) || ship.RemotesMovedThrough.Contains(HostShip))
            {
                RegisterAbilityTrigger(TriggerTypes.OnPositionFinish, delegate { AttachToShip(ship); });
            }
        }

        private void AttachToShip(GenericShip sufferedShip)
        {
            SufferedShip = sufferedShip;

            Rules.Collision.AddBump(HostShip, SufferedShip);

            GameManagerScript.Instance.StartCoroutine(CheckPositions());
        }

        private IEnumerator CheckPositions()
        {
            DisableRenderers();
            OldPosition = HostShip.GetPositionInfo();

            ObstaclesStayDetectorForced collisionDetector = HostShip.Model.GetComponentInChildren<ObstaclesStayDetectorForced>();

            RelocateToFrontGuides();

            collisionDetector.ReCheckCollisionsStart();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            bool canBePlacedFront = NoCollisionsWithObjects(collisionDetector);
            collisionDetector.ReCheckCollisionsFinish();

            RelocateToRearGuides();

            collisionDetector.ReCheckCollisionsStart();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            bool canBePlacedRear = NoCollisionsWithObjects(collisionDetector);
            collisionDetector.ReCheckCollisionsFinish();

            HostShip.SetPositionInfo(OldPosition);
            RestoreRenderers();

            if (!canBePlacedFront && !canBePlacedRear)
            {
                Messages.ShowInfo("Buzz Droid Swarm cannot be relocated without overlapping");
                DealDamageToBothShips();
            }
            else if (canBePlacedFront && canBePlacedRear)
            {
                StartDecisionSubphase();
            }
            else if (canBePlacedFront && !canBePlacedRear)
            {
                RelocateToFrontGuidesAndFinish(Triggers.FinishTrigger);
            }
            else if (!canBePlacedFront && canBePlacedRear)
            {
                RelocateToRearGuidesAndFinish(Triggers.FinishTrigger);
            }
        }

        private void StartDecisionSubphase()
        {
            RelocationDecisionSubPhase subphase = (RelocationDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(RelocationDecisionSubPhase),
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = "Buzz Droids Swarm";
            subphase.DescriptionLong = "After an enemy ship moves through or overlaps you, relocate to its front or rear guides";
            subphase.ImageSource = HostShip;

            subphase.AddDecision("Front", delegate { RelocateToFrontGuidesAndFinish(DecisionSubPhase.ConfirmDecision); }, isCentered: true);
            subphase.AddDecision("Rear", delegate { RelocateToRearGuidesAndFinish(DecisionSubPhase.ConfirmDecision); }, isCentered: true);

            subphase.DefaultDecisionName = "Front";

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;
            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private void RelocateToRearGuidesAndFinish(Action callback)
        {
            Messages.ShowInfo("Buzz Droid Swarm is relocated to rear guides");
            RelocateToRearGuides();
            callback();
        }

        private void RelocateToFrontGuidesAndFinish(Action callback)
        {
            Messages.ShowInfo("Buzz Droid Swarm is relocated to front guides");
            RelocateToFrontGuides();
            callback();
        }

        private static bool NoCollisionsWithObjects(ObstaclesStayDetectorForced collisionDetector)
        {
            return !collisionDetector.OverlapsAsteroidNow
                && !collisionDetector.OverlapsShipNow
                && collisionDetector.OverlapedMinesNow.Count == 0;
        }

        private void RestoreRenderers()
        {
            foreach (var rendererData in RenderersOldState)
            {
                rendererData.Key.enabled = rendererData.Value;
            }
        }

        private void RelocateToRearGuides()
        {
            HostShip.SetAngles(SufferedShip.GetAngles());
            HostShip.SetPosition(
                SufferedShip.GetPosition()
                + SufferedShip.Model.transform.TransformVector(new Vector3(0, 0, -Board.BoardIntoWorld(4.65f * Board.DISTANCE_1)))
            );
        }

        private void RelocateToFrontGuides()
        {
            HostShip.SetAngles(SufferedShip.GetAngles() + new Vector3(0, 180, 0));
            HostShip.SetPosition(
                SufferedShip.GetPosition()
                + SufferedShip.Model.transform.TransformVector(new Vector3(0, 0, Board.BoardIntoWorld(2.35f * Board.DISTANCE_1)))
            );
        }

        private void DisableRenderers()
        {
            RenderersOldState = new Dictionary<Renderer, bool>();
            foreach (Renderer renderer in HostShip.Model.GetComponentsInChildren<Renderer>())
            {
                RenderersOldState.Add(renderer, renderer.enabled);
                renderer.enabled = false;
            }
        }

        private void DealDamageToBothShips()
        {
            HostShip.Damage.TryResolveDamage(
                1,
                new DamageSourceEventArgs()
                {
                    DamageType = DamageTypes.CardAbility,
                    Source = HostShip
                },
                DealDamageToAnotherShip
            );
        }

        private void DealDamageToAnotherShip()
        {
            SufferedShip.Damage.TryResolveDamage(
                1,
                new DamageSourceEventArgs()
                {
                    DamageType = DamageTypes.CardAbility,
                    Source = HostShip
                },
                Triggers.FinishTrigger
            );
        }

        private class RelocationDecisionSubPhase : DecisionSubPhase { }
    }

}