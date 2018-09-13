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

        Phases.CurrentSubPhase = null;

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
        }
    }

    public static void RecordCommand(GameCommand command)
    {
        if (ShouldBeRecorded(command))
        {
            File.AppendAllText(FilePath, command.ToString() + "\n");
        }
    }

    public static bool ShouldBeRecorded(GameCommand command)
    {
        if (DebugManager.NoReplayCreation) return false;

        bool result = true;

        switch (command.Type)
        {
            case GameCommandTypes.PressSkip:
                if (command.SubPhase == typeof(SubPhases.ObstaclesPlacementSubPhase)) result = false;
                break;
            default:
                break;
        }

        return result;
    }

    public static void ExecuteWithDelay(Action callback, int seconds = 1)
    {
        if (Mode == ReplaysMode.Write)
        {
            callback();
        }
        else
        {
            GameManagerScript.Wait(seconds, delegate { callback(); });
        }
    }
}

