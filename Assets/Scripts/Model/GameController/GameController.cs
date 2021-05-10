using System;
using System.Collections.Generic;
using System.Linq;
using GameCommands;

public static class GameController
{
    private static readonly float COMMANDS_CHECK_DELAY = 0.01f;
    public static List<GameCommand> CommandsReceived { get; private set; }
    public static string LastMessage { get; set; }

    public static void Initialize()
    {
        CommandsReceived = new List<GameCommand>();
    }

    public static void StartBattle(ReplaysMode mode = ReplaysMode.Write)
    {
        GameController.Initialize();
        if (ReplaysManager.TryInitialize(mode))
        {
            if (mode == ReplaysMode.Read) MainMenu.CurrentMainMenu.InitializeSquadBuilder("Replay");

            Global.StartLocalGame();
        }
    }

    public static void SendCommand(GameCommand command)
    {
        Console.Write($"Command is received: {command.GetType().ToString().Replace("GameCommands.","")}", isBold: true, color: "cyan");
        CommandsReceived.Add(command);

        if (ReplaysManager.Mode == ReplaysMode.Write)
        {
            ReplaysManager.RecordCommand(command);
        }
    }

    public static void SendCommand(GameCommandTypes commandType, Type subPhase, int subPhaseId, string parameters = null)
    {
        SendCommand(GenerateGameCommand(commandType, subPhase, subPhaseId, parameters));
    }

    public static GameCommand GenerateGameCommand(string textjson, bool isRpc = false)
    {
        JSONObject json = new JSONObject(textjson);
        GameCommandTypes commandType = (GameCommandTypes)Enum.Parse(typeof(GameCommandTypes), json["command"].str);
        Type subPhase = Type.GetType(json["subphase"].str);
        int subPhaseId = (int) json["subphaseId"].i;
        string parameters = json["parameters"].ToString();

        return GenerateGameCommand(commandType, subPhase, subPhaseId, parameters, isRpc);
    }

    public static GameCommand GenerateGameCommand(GameCommandTypes commandType, Type subPhase, int subPhaseId, string parameters = null, bool isRpc = false)
    {
        GameCommand command = null;

        switch (commandType)
        {
            case GameCommandTypes.DamageDecksSync:
                command = new DamageDeckSyncCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SquadsSync:
                command = new SquadsSyncCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.Decision:
                command = new DecisionCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.ObstaclePlacement:
                command = new ObstaclePlacementCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.ShipPlacement:
                command = new ShipPlacementCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.AssignManeuver:
                command = new AssignManeuverCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.PressNext:
                command = new PressNextCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.ActivateAndMove:
                command = new ActivateAndMoveCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SelectShipToAssignManeuver:
                command = new SelectShipToAssignManeuverCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.DeclareAttack:
                command = new DeclareAttackCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.DiceModification:
                command = new DiceModificationCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SelectShip:
                command = new SelectShipCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SyncDiceResults:
                command = new SyncDiceResultsCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SyncDiceRerollSelected:
                command = new SyncDiceRerollSelectedCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.ConfirmCrit:
                command = new ConfirmCritCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.ConfirmDiceCheck:
                command = new ConfirmDiceCheckCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.PressSkip:
                command = new PressSkipCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SyncPlayerWithInitiative:
                command = new SyncPlayerWithInitiativeCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SystemActivation:
                command = new SystemActivationCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.CombatActivation:
                command = new CombatActivationCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.SelectObstacle:
                command = new SelectObstacleCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.BombPlacement:
                command = new BombPlacementCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.MoveObstacle:
                command = new MoveObstacleCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            case GameCommandTypes.CancelShipSelection:
                command = new CancelShipSelectionCommand(commandType, subPhase, subPhaseId, parameters);
                break;
            default:
                break;
        }

        return command;
    }

    public static GameCommand GetCommand()
    {
        return (CommandsReceived.Count > 0) ? CommandsReceived.First() : null;
    }

    public static void ConfirmCommand()
    {
        CommandsReceived.RemoveAt(0);
    }

    public static void WaitForCommand()
    {
        GameCommand command = GameController.GetCommand();
        if (command != null)
        {
            // Console.Write("=> Execute");
            command.TryExecute();
        }
        else
        {
            // Console.Write("=> Wait");
        }

        GameManagerScript.Wait(COMMANDS_CHECK_DELAY, WaitForCommand);
    }

}