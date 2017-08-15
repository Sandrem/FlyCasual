using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Players
{

    public partial class GenericAiPlayer : GenericPlayer
    {

        public GenericAiPlayer() : base() {
            Type = PlayerType.Ai;
            Name = "AI";
        }

        public override void SetupShip()
        {
            foreach (var shipHolder in Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    int direction = (Phases.CurrentSubPhase.RequiredPlayer == PlayerNo.Player1) ? -1 : 1;
                    shipHolder.Value.SetPosition(shipHolder.Value.GetPosition() + new Vector3(0, 0, direction * 1.2f));

                    shipHolder.Value.IsSetupPerformed = true;
                }
            }
            Phases.Next();
        }

        public override void AssignManeuver()
        {
            foreach (var shipHolder in Ships)
            {
                Selection.ChangeActiveShip("ShipId:" + shipHolder.Value.ShipId);
                shipHolder.Value.AssignedManeuver = new Movement.StraightMovement(2, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.White);
            }
            Phases.Next();
        }

        public override void PerformManeuver()
        {
            bool foundToActivate = false;
            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    if (!shipHolder.Value.IsManeuverPerformed)
                    {
                        foundToActivate = true;
                        Selection.ChangeActiveShip("ShipId:" + shipHolder.Value.ShipId);
                        ActivateShip(shipHolder.Value);
                        break;
                    }
                }
            }

            if (!foundToActivate)
            {
                Phases.Next();
            }
        }

        public virtual void ActivateShip(Ship.GenericShip ship)
        {
            Selection.ChangeActiveShip("ShipId:" + ship.ShipId);
            PerformManeuverOfShip(ship);
        }

        protected void PerformManeuverOfShip(Ship.GenericShip ship)
        {
            Selection.ChangeActiveShip("ShipId:" + ship.ShipId);
            Game.Movement.PerformStoredManeuver();
        }

        public override void PerformAction()
        {
            //Stub
            Phases.Next();
        }

        public override void PerformFreeAction()
        {
            if (Selection.ThisShip.GetAvailableFreeActionsList().Count > 0)
            {
                ActionsList.GenericAction action = Selection.ThisShip.GetAvailableFreeActionsList()[0];
                Selection.ThisShip.AddAlreadyExecutedAction(action);
                action.ActionTake();
            }
            else
            {
                Phases.Next();
            }
        }

        public override void PerformAttack()
        {
            if (DebugManager.DebugAI) Debug.Log("AI wants to attack!");

            bool attackPerformed = false;

            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    if (!shipHolder.Value.IsAttackPerformed)
                    {
                        Selection.ChangeActiveShip("ShipId:" + shipHolder.Value.ShipId);
                        break;
                    }
                }
            }

            if (Selection.ThisShip != null)
            {
                Dictionary<Ship.GenericShip, float> enemyShips = GetEnemyShipsAndDistance(Selection.ThisShip, ignoreCollided: true, inArcAndRange: true);
                foreach (var shipHolder in enemyShips)
                {
                    if (DebugManager.DebugAI) Debug.Log("AI wants to attack: " + shipHolder.Key);
                    Selection.TryToChangeAnotherShip("ShipId:" + shipHolder.Key.ShipId);
                    Combat.SelectWeapon();

                    if (Rules.TargetIsLegalForShot.IsLegal())
                    {
                        if (DebugManager.DebugAI) Debug.Log("AI target legal: " + Selection.AnotherShip);
                        attackPerformed = true;
                        Combat.TryPerformAttack();
                        break;
                    }
                }
                Selection.ThisShip.IsAttackPerformed = true;
            }

            if (!attackPerformed)
            {
                if (DebugManager.DebugAI) Debug.Log("AI didn't performed attack and goes NEXT");
                Phases.Next();
            }

        }

        public Ship.GenericShip FindNearestEnemyShip(Ship.GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Dictionary<Ship.GenericShip, float> results = GetEnemyShipsAndDistance(thisShip, ignoreCollided, inArcAndRange);
            Ship.GenericShip result = null;
            if (results.Count != 0)
            {
                result = results.OrderBy(n => n.Value).First().Key;
            }
            return result;
        }

        public Dictionary<Ship.GenericShip, float> GetEnemyShipsAndDistance(Ship.GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Dictionary<Ship.GenericShip, float> results = new Dictionary<Ship.GenericShip, float>();

            foreach (var shipHolder in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
            {
                if (ignoreCollided)
                {
                    if (thisShip.LastShipCollision != null)
                    {
                        if (thisShip.LastShipCollision.ShipId == shipHolder.Value.ShipId)
                        {
                            continue;
                        }
                    }
                    if (shipHolder.Value.LastShipCollision != null)
                    {
                        if (shipHolder.Value.LastShipCollision.ShipId == thisShip.ShipId)
                        {
                            continue;
                        }
                    }
                }

                if (inArcAndRange)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(thisShip, shipHolder.Value);
                    if ((shotInfo.Range > 3) || (!shotInfo.InArc))
                    {
                        continue;
                    }
                }

                float distance = Vector3.Distance(thisShip.GetCenter(), shipHolder.Value.GetCenter());
                results.Add(shipHolder.Value, distance);
            }
            results = results.OrderBy(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            return results;
        }

        public override void UseDiceModifications()
        {
            Selection.ActiveShip.GenerateAvailableActionEffectsList();
            List<ActionsList.GenericAction> availableActionEffectsList = Selection.ActiveShip.GetAvailableActionEffectsList();

            Dictionary<ActionsList.GenericAction, int> actionsPriority = new Dictionary<ActionsList.GenericAction, int>();

            foreach (var actionEffect in availableActionEffectsList)
            {
                int priority = actionEffect.GetActionEffectPriority();
                actionsPriority.Add(actionEffect, priority);
            }

            actionsPriority = actionsPriority.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            bool isActionEffectTaken = false;

            if (actionsPriority.Count > 0)
            {
                if (actionsPriority.First().Value > 0)
                {
                    isActionEffectTaken = true;
                    Messages.ShowInfo("AI uses \"" + actionsPriority.First().Key.Name + "\"");
                    Game.Wait(1, delegate { actionsPriority.First().Key.ActionEffect(UseDiceModifications); });                    
                }
            }

            if (!isActionEffectTaken)
            {
                Game.Wait(2, delegate { Phases.CurrentSubPhase.CallBack(); });
            }
        }

        public override void TakeDecision()
        {
            Phases.CurrentSubPhase.DoDefault();
        }

    }

}
