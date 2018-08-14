using ActionsList;
using GameCommands;
using GameModes;
using Ship;
using SubPhases;
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

                DecisionSubPhase.ExecuteDecision(command.GetString("name"));
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

                ObstaclesPlacementSubPhase.PlaceObstacle(
                    command.GetString("name"),
                    new Vector3(command.GetFloat("positionX"), command.GetFloat("positionY"), command.GetFloat("positionZ")),
                    new Vector3(command.GetFloat("rotationX"), command.GetFloat("rotationY"), command.GetFloat("rotationZ"))
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

                SetupSubPhase.PlaceShip(
                    int.Parse(command.GetString("id")),
                    new Vector3(command.GetFloat("positionX"), command.GetFloat("positionY"), command.GetFloat("positionZ")),
                    new Vector3(command.GetFloat("rotationX"), command.GetFloat("rotationY"), command.GetFloat("rotationZ"))
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

                ShipMovementScript.AssignManeuver(int.Parse(command.GetString("id")), command.GetString("maneuver"));

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

                UI.SendNextButtonCommand();

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

            if (command.Type == GameCommandTypes.ActivateAndMove && Phases.CurrentSubPhase.GetType() == command.SubPhase)
            {
                Console.Write("Replay: Activate Ship Movement is executed", color: "aqua");
                GameController.ConfirmCommand();

                Selection.ChangeActiveShip("ShipId:" + int.Parse(command.GetString("id")));
                ShipMovementScript.ActivateAndMove(int.Parse(command.GetString("id")));
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
                DiceModificationTimingType diceModificationType = (DiceModificationTimingType)Enum.Parse(typeof(DiceModificationTimingType), (string)command.GetParameter("Type"));
                if (type == diceModificationType)
                {
                    Console.Write("Replay: Dice Modification is executed", color: "aqua");
                    GameController.ConfirmCommand();

                    string diceModificationName = (string)command.GetParameter("Name");
                    if (diceModificationName == "OK")
                    {
                        Combat.ConfirmDiceResultsClient();
                    }
                    else
                    {
                        Combat.UseDiceModification(diceModificationName);
                    }
                }
            }
            else
            {
                Console.Write("Replay: No saved Dice Modification", color: "aqua");
            }
        }
    }

}
