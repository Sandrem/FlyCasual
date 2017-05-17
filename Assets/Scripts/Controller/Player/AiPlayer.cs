using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class AiPlayer : GenericPlayer
    {

        public AiPlayer(int id): base(id) {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Type = PlayerType.Ai;
            Name = "AI";
            Id = id;

            Debug.Log("Internal id = " + Id);
        }

        public override void SetupShip()
        {
            Game.PhaseManager.Next();
        }

        public override void AssignManeuver()
        {
            foreach (var shipId in Game.Roster.GetTeam(Id))
            {
                Game.Roster.AllShips[shipId].AssignedManeuver = Game.Movement.ManeuverFromString("2.F.S");
            }
            Game.PhaseManager.Next();
        }

        public override void PerformManeuver()
        {
            foreach (var shipId in Game.Roster.GetTeam(Id))
            {
                if (Game.Roster.AllShips[shipId].PilotSkill == Game.PhaseManager.CurrentSubPhase.RequiredPilotSkill)
                {
                    Game.Selection.ThisShip = Game.Roster.AllShips[shipId];
                    Game.Movement.PerformStoredManeuver();
                    break;
                }
            }
        }

        public override void PerformAction()
        {
            if (Game.Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                Actions.GenericAction action = Game.Selection.ThisShip.GetAvailableActionsList()[0];
                action.ActionTake();
                Game.Selection.ThisShip.AddAlreadyExecutedAction(action);
                Game.PhaseManager.Next();
            }
        }

        public override void PerformFreeAction()
        {
            if (Game.Selection.ThisShip.GetAvailableFreeActionsList().Count > 0)
            {
                Actions.GenericAction action = Game.Selection.ThisShip.GetAvailableFreeActionsList()[0];
                action.ActionTake();
                Game.Selection.ThisShip.AddAlreadyExecutedAction(action);
            }
        }

        public override void PerformAttack()
        {
            foreach (var shipId in Game.Roster.GetTeam(Id))
            {
                if (Game.Roster.AllShips[shipId].PilotSkill == Game.PhaseManager.CurrentSubPhase.RequiredPilotSkill)
                {
                    Game.Selection.ThisShip = Game.Roster.AllShips[shipId];
                    break;
                }
            }

            bool attackPerformed = false;
            foreach (var shipId in Game.Roster.GetTeam(Game.Roster.AnotherPlayer(Id)))
            {
                Game.Selection.AnotherShip = Game.Roster.AllShips[shipId];
                if (Game.Actions.CheckShot())
                {
                    attackPerformed = true;
                    Game.Actions.PerformAttack();
                }
            }
            if (!attackPerformed) Game.PhaseManager.Next();

        }

        public override void UseDiceModifications()
        {
            if (Game.Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                Game.Selection.ThisShip.GetAvailableActionsList()[0].ActionEffect();
                Game.StartCoroutine(Wait());
            }
            else
            {
                Game.UI.DiceResults.ConfirmDiceResult();
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(3);
            Game.UI.DiceResults.ConfirmDiceResult();
        }

    }

}
