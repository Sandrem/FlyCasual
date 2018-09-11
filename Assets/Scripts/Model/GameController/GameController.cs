using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCommands;
using SquadBuilderNS;
using UnityEngine;

public static class GameController
{
    private static List<GameCommand> CommandsReceived;

    public static void Initialize()
    {
        CommandsReceived = new List<GameCommand>();
    }

    public static void StartBattle(ReplaysMode mode = ReplaysMode.Write)
    {
        GameController.Initialize();
        ReplaysManager.Initialize(mode);

        if (mode == ReplaysMode.Read) MainMenu.CurrentMainMenu.InitializeSquadBuilder("Replay");

        Console.Write("Game is started", LogTypes.GameCommands, true, "aqua");
        SquadBuilder.StartLocalGame();
    }

    public static void SendCommand(GameCommand command)
    {
        Console.Write("Command is received: " + command.Type, LogTypes.GameCommands, false, "aqua");

        CommandsReceived.Add(command);

        if (ReplaysManager.Mode == ReplaysMode.Write)
        {
            ReplaysManager.RecordCommand(command);
        }

        command.TryExecute();
    }

    public static void SendCommand(GameCommandTypes commandType, Type subPhase, string parameters = null)
    {
        SendCommand(GenerateGameCommand(commandType, subPhase, parameters));
    }

    public static GameCommand GenerateGameCommand(string textjson)
    {
        JSONObject json = new JSONObject(textjson);
        GameCommandTypes commandType = (GameCommandTypes)Enum.Parse(typeof(GameCommandTypes), json["command"].str);
        Type subPhase = System.Type.GetType(json["subphase"].str);
        string parameters = json["parameters"].ToString();

        return GenerateGameCommand(commandType, subPhase, parameters);
    }

    public static GameCommand GenerateGameCommand(GameCommandTypes commandType, Type subPhase, string parameters = null)
    {
        GameCommand command = null;

        switch (commandType)
        {
            case GameCommandTypes.DamageDecksSync:
                command = new DamageDeckSyncCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.SquadsSync:
                command = new SquadsSyncCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.Decision:
                command = new DecisionCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.ObstaclePlacement:
                command = new ObstaclePlacementCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.ShipPlacement:
                command = new ShipPlacementCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.AssignManeuver:
                command = new AssignManeuverCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.PressNext:
                command = new PressNextCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.ActivateAndMove:
                command = new ActIvateAndMoveCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.DeclareAttack:
                command = new DeclareAttackCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.DiceModification:
                command = new DiceModificationCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.SelectShip:
                command = new SelectShipCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.SyncDiceResults:
                command = new SyncDiceResultsCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.SyncDiceRerollSelected:
                command = new SyncDiceRerollSelectedCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.ConfirmCrit:
                command = new ConfirmCritCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.ConfirmDiceCheck:
                command = new ConfirmDiceCheckCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.PressSkip:
                command = new PressSkipCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.HotacSwerve:
                command = new HotacSwerveCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.HotacFreeTargetLock:
                command = new HotacFreeTargetLockCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.SyncPlayerWithInitiative:
                command = new SyncPlayerWithInitiativeCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.SystemActivation:
                command = new SystemActivationCommand(commandType, subPhase, parameters);
                break;
            case GameCommandTypes.CombatActivation:
                command = new CombatActivationCommand(commandType, subPhase, parameters);
                break;
            default:
                Console.Write("Constructor for GameCommand is not found", LogTypes.Errors, true, "red");
                break;
        }

        Console.Write("Command is generated: " + command.Type, LogTypes.GameCommands, false, "aqua");

        return command;
    }

    public static void CheckExistingCommands()
    {
        GameCommand command = GameController.GetCommand();
        if (command != null) command.TryExecute();
    }

    public static GameCommand GetCommand()
    {
        return (CommandsReceived.Count > 0) ? CommandsReceived.First() : null;
    }

    public static void ConfirmCommand()
    {
        CommandsReceived.RemoveAt(0);
    }

}