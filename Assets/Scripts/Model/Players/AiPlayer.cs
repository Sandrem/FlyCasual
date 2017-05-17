using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class AiPlayer : GenericPlayer
    {

        public AiPlayer(): base() {
            Type = PlayerType.Ai;
            Name = "AI";
        }

        public override void SetupShip()
        {
            Game.Phases.Next();
        }

        public override void AssignManeuver()
        {
            foreach (var shipHolder in Ships)
            {
                shipHolder.Value.AssignedManeuver = Game.Movement.ManeuverFromString("2.F.S");
            }
            Game.Phases.Next();
        }

        public override void PerformManeuver()
        {
            foreach (var shipHolder in Ships)
            {
                if (shipHolder.Value.PilotSkill == Game.Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    Game.Selection.ThisShip = shipHolder.Value;
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
                Game.Phases.Next();
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
            foreach (var shipHolder in Ships)
            {
                if (shipHolder.Value.PilotSkill == Game.Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    Game.Selection.ThisShip = shipHolder.Value;
                    break;
                }
            }

            bool attackPerformed = false;
            foreach (var shipHolder in Game.Roster.GetPlayer(Game.Roster.AnotherPlayer(PlayerNo)).Ships)
            {
                Game.Selection.AnotherShip = Game.Roster.AllShips[shipHolder.Key];
                if (Game.Actions.CheckShot())
                {
                    attackPerformed = true;
                    Game.Actions.PerformAttack();
                }
            }
            if (!attackPerformed) Game.Phases.Next();

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
