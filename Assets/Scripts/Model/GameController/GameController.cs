using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCommands;

public static class GameController
{
    private static List<GameCommand> CommandsReceived;

    public static void Initialize()
    {
        CommandsReceived = new List<GameCommand>();
    }

    public static void SendCommand(GameCommandTypes commandType, Type subPhase, string parameters = null)
    {
        CommandsReceived.Add(new GameCommand(commandType, subPhase, parameters));
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