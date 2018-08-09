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
                Console.Write("Replay: No saved Decision", color: "blue");
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
                Console.Write("Replay: No saved Obstacle Placement", color: "blue");
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
                    (int)(float)command.GetParameter("Id"),
                    new Vector3((float)command.GetParameter("PositionX"), (float)command.GetParameter("PositionY"), (float)command.GetParameter("PositionZ")),
                    new Vector3((float)command.GetParameter("RotationX"), (float)command.GetParameter("RotationY"), (float)command.GetParameter("RotationZ"))
                );
            }
            else
            {
                Console.Write("Replay: No saved Ship Setup", color: "blue");
            }
            
        }
    }

}
