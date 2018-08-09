using ActionsList;
using GameCommands;
using GameModes;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class ReplayPlayer : GenericPlayer
    {
        public ReplayPlayer() : base()
        {
            Type = PlayerType.Replay;
            Name = "Replay";
        }

        public override void TakeDecision()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.Decision && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Saved Decision is executed", color: "aqua");
                GameController.ConfirmCommand();

                SubPhases.DecisionSubPhase.ExecuteDecision((string) command.GetParameter("Name"));
            }
            else
            {
                Console.Write("Replay: No saved Decision", color: "aqua");
            }
        }

        public override void PlaceObstacle()
        {
            base.PlaceObstacle();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ObstaclePlacement && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Obstacle Placement is executed", color: "aqua");
                GameController.ConfirmCommand();

                GameMode.CurrentGameMode.PlaceObstacle(
                    (string) command.GetParameter("Name"),
                    new Vector3((float) command.GetParameter("PositionX"), (float)command.GetParameter("PositionY"), (float)command.GetParameter("PositionZ")),
                    new Vector3((float) command.GetParameter("RotationX"), (float)command.GetParameter("RotationY"), (float)command.GetParameter("RotationZ"))
                );
            }
            else
            {
                Console.Write("Replay: No saved Obstacle Placement", color: "aqua");
            }
        }

        public override void SetupShip()
        {
            base.SetupShip();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ShipPlacement && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Ship Setup is executed", color: "aqua");
                GameController.ConfirmCommand();

                GameMode.CurrentGameMode.ConfirmShipSetup(
                    (int) (float) command.GetParameter("Id"),
                    new Vector3((float)command.GetParameter("PositionX"), (float)command.GetParameter("PositionY"), (float)command.GetParameter("PositionZ")),
                    new Vector3((float)command.GetParameter("RotationX"), (float)command.GetParameter("RotationY"), (float)command.GetParameter("RotationZ"))
                );
            }
            else
            {
                Console.Write("Replay: No saved Ship Setup", color: "aqua");
            }
            
        }

        public override void AssignManeuver()
        {
            base.AssignManeuver();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.AssignManeuver && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Assign Maneuver is executed", color: "aqua");
                GameController.ConfirmCommand();

                Selection.ChangeActiveShip("ShipId:" + (int) (float) command.GetParameter("Id"));
                Selection.ThisShip.SetAssignedManeuver(ShipMovementScript.MovementFromString((string) command.GetParameter("ManeuverCode")));

                //Check next commands
                GameCommand nextCommand = GameController.GetCommand();
                if (nextCommand != null && nextCommand.SubPhase == command.SubPhase) GameController.Next();
            }
            else
            {
                Console.Write("Replay: No saved Assign Maneuver", color: "aqua");
            }
        }

        public override void PressNext()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.PressNext && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Press Next is executed", color: "aqua");
                GameController.ConfirmCommand();

                UI.NextButtonEffect();

                //Check next commands
                GameCommand nextCommand = GameController.GetCommand();
                if (nextCommand != null && nextCommand.SubPhase == command.SubPhase) GameController.Next();
            }
            else
            {
                Console.Write("Replay: No saved Press Next", color: "aqua");
            }
        }

        public override void PerformManeuver()
        {
            base.PerformManeuver();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ActiveShipMovement && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Activate Ship Movement is executed", color: "aqua");
                GameController.ConfirmCommand();

                Selection.ChangeActiveShip("ShipId:" + (int)(float) command.GetParameter("Id"));
                ShipMovementScript.ActivateAndMove((int)(float)command.GetParameter("Id"));
            }
            else
            {
                Console.Write("Replay: No saved Activate Ship Movement", color: "aqua");
            }
        }

        public override void PerformAttack()
        {
            base.PerformManeuver();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.DeclareAttack && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Declare Attack is executed", color: "aqua");
                GameController.ConfirmCommand();

                Selection.ChangeActiveShip("ShipId:" + (int)(float)command.GetParameter("Id"));
                Selection.ThisShip.CallCombatActivation(
                    delegate {
                        Selection.ThisShip.CallAfterAttackWindow();
                        Selection.ThisShip.IsAttackPerformed = true;

                        Selection.TryToChangeAnotherShip("ShipId:" + (int)(float)command.GetParameter("TargetId"));
                        Combat.DeclareIntentToAttack((int)(float)command.GetParameter("Id"), (int)(float)command.GetParameter("TargetId"));
                    }
                );
            }
            else
            {
                Console.Write("Replay: No saved Declare Attack Movement", color: "aqua");
            }
        }

        public override void UseDiceModifications(DiceModificationTimingType type)
        {
            base.UseDiceModifications(type);
            Combat.ShowDiceModificationButtons(type);

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.DiceModification && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Dice Modification is executed", color: "aqua");
                GameController.ConfirmCommand();

                string diceModificationName = (string) command.GetParameter("Name");
                if (diceModificationName == "OK")
                {
                    Combat.ConfirmDiceResultsClient();
                }
                else
                {
                    Combat.UseDiceModification(diceModificationName);
                }
            }
            else
            {
                Console.Write("Replay: No saved Dice Modification", color: "aqua");
            }
        }
    }

}
