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
            Phases.Next();
        }

        public override void AssignManeuver()
        {
            foreach (var shipHolder in Ships)
            {
                //Temporary
                Selection.ThisShip = shipHolder.Value;

                shipHolder.Value.AssignedManeuver = Game.Movement.ManeuverFromString("2.F.S");
            }
            Phases.Next();
        }

        public override void PerformManeuver()
        {
            foreach (var shipHolder in Roster.AllShips)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    
                    Selection.ThisShip = shipHolder.Value;
                    ActivateShip(shipHolder.Value);
                    break;
                }
            }
        }

        public virtual void ActivateShip(Ship.GenericShip ship)
        {
            PerformManeuverOfShip(ship);
        }

        protected void PerformManeuverOfShip(Ship.GenericShip ship)
        {
            Game.Movement.PerformStoredManeuver();
        }

        public override void PerformAction()
        {
            if (Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                ActionsList.GenericAction action = Selection.ThisShip.GetAvailableActionsList()[0];
                action.ActionTake();
                Selection.ThisShip.AddAlreadyExecutedAction(action);
            }
            //test
            Phases.FinishSubPhase(typeof(SubPhases.ActionSubPhase));
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
            foreach (var shipHolder in Roster.AllShips)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    Selection.ThisShip = shipHolder.Value;
                    break;
                }
            }

            bool attackPerformed = false;
            foreach (var shipHolder in Roster.GetPlayer(Roster.AnotherPlayer(PlayerNo)).Ships)
            {
                Selection.AnotherShip = Roster.AllShips[shipHolder.Key];
                if (Actions.TargetIsLegal())
                {
                    attackPerformed = true;
                    Game.Wait(Actions.TryPerformAttack);
                }
            }
            if (!attackPerformed) Phases.Next();

        }

        public Ship.GenericShip FindNearestEnemyShip(Ship.GenericShip thisShip)
        {
            Ship.GenericShip result = null;
            float distance = float.MaxValue;
            foreach (var shipHolder in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
            {
                float newDistance = Vector3.Distance(thisShip.GetPosition(), shipHolder.Value.GetPosition());
                if (newDistance < distance)
                {
                    distance = newDistance;
                    result = shipHolder.Value;
                    Debug.Log("Enenmy: " + shipHolder.Value);
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
