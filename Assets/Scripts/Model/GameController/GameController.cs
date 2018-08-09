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

    public static void SendCommand(GameCommandTypes commandType, Type subPhase, string parameters)
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
}