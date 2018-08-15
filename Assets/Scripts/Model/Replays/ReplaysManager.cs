using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquadBuilderNS;
using Players;
using UnityEngine;
using RuleSets;
using System.IO;
using GameCommands;

public enum ReplaysMode
{
    Write,
    Read
}

public static class ReplaysManager
{
    public static ReplaysMode Mode { get; private set; }
    private static string FilePath;

    public static void Initialize(ReplaysMode mode)
    {
        Mode = mode;

        FilePath = Application.persistentDataPath + "/" + RuleSet.Instance.Name + "/Replays";
        if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
        FilePath += "/LastReplay.replay";

        if (Mode == ReplaysMode.Write)
        {
            File.Delete(FilePath);
        }
        else if (Mode == ReplaysMode.Read)
        {
            string[] commands = File.ReadAllLines(FilePath);
            
            foreach (var line in commands)
            {
                JSONObject json = new JSONObject(line);
                GameController.SendCommand(
                    (GameCommandTypes) Enum.Parse(typeof(GameCommandTypes), json["command"].str),
                    System.Type.GetType(json["subphase"].str),
                    json["parameters"].ToString()
                );
            }

            //ReadTest();
        }
    }

    public static void RecordCommand(GameCommand command)
    {
        File.AppendAllText(FilePath, command.ToString() + "\n");
    }

    public static void StartBattle()
    {
        GameController.Initialize();
        ReplaysManager.Initialize(ReplaysMode.Read);

        SquadBuilder.StartLocalGame();
    }

    public static void WriteHotacAiSwerve(int shipId, string maneuverCode)
    {
        JSONObject parameters = new JSONObject();
        parameters.AddField("id", shipId.ToString());
        parameters.AddField("maneuver", maneuverCode);
        GameCommand command = new GameCommand(
            GameCommandTypes.AssignManeuver,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        RecordCommand(command);

        parameters = new JSONObject();
        parameters.AddField("id", shipId.ToString());
        command = new GameCommand(
            GameCommandTypes.ActivateAndMove,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        RecordCommand(command);
    }

}

