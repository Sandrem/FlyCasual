using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionsList;
using GameCommands;
using SubPhases;
using UnityEngine;

public static class GameController
{
    private static List<GameCommand> CommandsReceived;

    public static void Initialize()
    {
        CommandsReceived = new List<GameCommand>();
    }

    public static void SendCommand(GameCommandTypes commandType, Type subPhase, string parameters = null)
    {
        GameCommand command = new GameCommand(commandType, subPhase, parameters);
        CommandsReceived.Add(command);

        if (ReplaysManager.Mode == ReplaysMode.Write)
        {
            ReplaysManager.RecordCommand(command);
        }

        TryExecuteCommand(command);
    }

    private static void TryExecuteCommand(GameCommand command)
    {
        if (Phases.CurrentSubPhase !=null && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
        {
            switch (command.Type)
            {
                case GameCommandTypes.Decision:
                    DecisionSubPhase.ExecuteDecision(command.GetString("name"));
                    break;
                case GameCommandTypes.ObstaclePlacement:
                   ObstaclesPlacementSubPhase.PlaceObstacle(
                        command.GetString("name"),
                        new Vector3(command.GetFloat("positionX"), command.GetFloat("positionY"), command.GetFloat("positionZ")),
                        new Vector3(command.GetFloat("rotationX"), command.GetFloat("rotationY"), command.GetFloat("rotationZ"))
                    );
                    break;
                case GameCommandTypes.ShipPlacement:
                    SetupSubPhase.PlaceShip(
                        int.Parse(command.GetString("id")),
                        new Vector3(command.GetFloat("positionX"), command.GetFloat("positionY"), command.GetFloat("positionZ")),
                        new Vector3(command.GetFloat("rotationX"), command.GetFloat("rotationY"), command.GetFloat("rotationZ"))
                    );
                    break;
                case GameCommandTypes.AssignManeuver:
                    ShipMovementScript.AssignManeuver(
                        int.Parse(command.GetString("id")),
                        command.GetString("maneuver")
                    );
                    break;
                case GameCommandTypes.PressNext:
                    UI.NextButtonEffect();
                    break;
                case GameCommandTypes.ActivateAndMove:
                    ShipMovementScript.ActivateAndMove(
                        int.Parse(command.GetString("id"))
                    );
                    break;
                case GameCommandTypes.DeclareAttack:
                    Combat.DeclareIntentToAttack(
                        int.Parse(command.GetString("id")),
                        int.Parse(command.GetString("target"))
                    );
                    break;
                case GameCommandTypes.DiceModification:
                    string diceModificationName = command.GetString("name");
                    if (diceModificationName == "OK")
                    {
                        Combat.ConfirmDiceResultsClient();
                    }
                    else
                    {
                        Combat.UseDiceModification(diceModificationName);
                    }
                    break;
                case GameCommandTypes.SelectShip:
                    SelectShipSubPhase.SelectShip(int.Parse(command.GetString("id")));
                    break;
                case GameCommandTypes.SyncDiceResults:
                    List<DieSide> correctSides = new List<DieSide>();
                    JSONObject jsonHolder = (JSONObject) command.GetParameter("sides");
                    foreach (var dieInfo in jsonHolder.list)
                    {
                        DieSide side = (DieSide)Enum.Parse(typeof(DieSide), dieInfo["side"].str);
                        correctSides.Add(side);
                    }
                    DiceRollCombatSubPhase.SyncDiceResults(correctSides);
                    break;
                default:
                    break;
            }
        }
    }

    public static GameCommand GetCommand()
    {
        return (CommandsReceived.Count > 0) ? CommandsReceived.First() : null;
    }

    public static void ConfirmCommand()
    {
        CommandsReceived.RemoveAt(0);
    }

    public static void Next()
    {
        GameCommand command = CommandsReceived.FirstOrDefault();
        if (command != null)
        {
            switch (command.Type)
            {
                case GameCommandTypes.AssignManeuver:
                    Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).AssignManeuver();
                    break;
                case GameCommandTypes.PressNext:
                    Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).PressNext();
                    break;
                default:
                    break;
            }
        }
    }

}