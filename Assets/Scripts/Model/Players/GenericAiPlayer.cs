using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                Selection.ThisShip.IsManeuverPerformed = false;
                shipHolder.Value.AssignedManeuver = Game.Movement.ManeuverFromString("2.F.S");
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
                Debug.Log("GO NEXT!!!");
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
                action.ActionTake();
                Selection.ThisShip.AddAlreadyExecutedAction(action);
            }
        }

        public override void PerformAttack()
        {
            bool attackPerformed = false;

            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    if (!shipHolder.Value.IsAttackPerformed)
                    {
                        Selection.ChangeActiveShip("ShipId:" + shipHolder.Value.ShipId);
                        //Selection.ThisShip = shipHolder.Value;
                        break;
                    }
                }
            }

            if (Selection.ThisShip != null)
            {
                Ship.GenericShip enemyShip = FindNearestEnemyShip(Selection.ThisShip, ignoreCollided: true, inArcAndRange: true);

                if (enemyShip != null)
                {
                    //TODO: Biggs
                    Selection.TryToChangeAnotherShip("ShipId:" + enemyShip.ShipId);
                    Combat.SelectWeapon();
                    attackPerformed = true;
                    Actions.TryPerformAttack();
                }
                else
                {
                    Selection.ThisShip.IsAttackPerformed = true;
                }
            }

            if (!attackPerformed) Phases.Next();

        }

        public Ship.GenericShip FindNearestEnemyShip(Ship.GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Ship.GenericShip result = null;
            float distance = float.MaxValue;
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
                    if ((Actions.GetFiringRange(thisShip, shipHolder.Value) > 3) || (!Actions.InArcCheck(thisShip, shipHolder.Value)))
                    {
                        continue;
                    }
                }

                float newDistance = Vector3.Distance(thisShip.GetCenter(), shipHolder.Value.GetCenter());
                if (newDistance < distance)
                {
                    distance = newDistance;
                    result = shipHolder.Value;
                }
            }
            return result;
        }

        public override void UseDiceModifications()
        {
            if (Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                Selection.ThisShip.GetAvailableActionsList()[0].ActionEffect();
                Game.Wait(Combat.ConfirmDiceResults);
            }
            else
            {
                Combat.ConfirmDiceResults();
            }
        }

    }

}
