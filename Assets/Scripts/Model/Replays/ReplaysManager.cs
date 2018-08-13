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
    private static readonly string TestSquad1 = "{\"name\":\"X-Wing\",\"faction\":\"rebel\",\"points\":52,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"wedgeantilles\",\"points\":52,\"ship\":\"xwing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"description\":\"Wedge Antilles\"}";
    private static readonly string TestSquad2 = "{\"name\":\"TIE Fighters\",\"faction\":\"imperial\",\"points\":50,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"blacksquadronace\",\"points\":26,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":26,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"description\":\"Black Squadron Ace\nBlack Squadron Ace\"}";

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

    private static void TestBattle()
    {
        JSONObject parameters = null;

        // INITIATIVE

        parameters = new JSONObject();
        parameters.AddField("Name", "Me");
        GameController.SendCommand(
            GameCommandTypes.Decision,
            typeof(SubPhases.InitialiveDecisionSubPhase),
            parameters.ToString()
        );

        // OBSTACLES

        parameters = new JSONObject();
        parameters.AddField("Name", "A1");
        parameters.AddField("PositionX", 2.5f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", 0f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 0f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ObstaclePlacement,
            typeof(SubPhases.ObstaclesPlacementSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "A2");
        parameters.AddField("PositionX", 1f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", -1f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 0f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ObstaclePlacement,
            typeof(SubPhases.ObstaclesPlacementSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "A3");
        parameters.AddField("PositionX", -2f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", 1f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 0f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ObstaclePlacement,
            typeof(SubPhases.ObstaclesPlacementSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "A4");
        parameters.AddField("PositionX", -1f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", 2f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 0f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ObstaclePlacement,
            typeof(SubPhases.ObstaclesPlacementSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "A5");
        parameters.AddField("PositionX", -1f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", -1.5f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 0f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ObstaclePlacement,
            typeof(SubPhases.ObstaclesPlacementSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "A6");
        parameters.AddField("PositionX", 2f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", 1.5f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 0f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ObstaclePlacement,
            typeof(SubPhases.ObstaclesPlacementSubPhase),
            parameters.ToString()
        );

        // SHIP SETUP

        parameters = new JSONObject();
        parameters.AddField("Id", 2);
        parameters.AddField("PositionX", -0.5f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", 4f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 180f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ShipPlacement,
            typeof(SubPhases.SetupSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 3);
        parameters.AddField("PositionX", 0.5f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", 4f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 180f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ShipPlacement,
            typeof(SubPhases.SetupSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 1);
        parameters.AddField("PositionX", 0f); parameters.AddField("PositionY", 0f); parameters.AddField("PositionZ", -4f);
        parameters.AddField("RotationX", 0f); parameters.AddField("RotationY", 0f); parameters.AddField("RotationZ", 0f);
        GameController.SendCommand(
            GameCommandTypes.ShipPlacement,
            typeof(SubPhases.SetupSubPhase),
            parameters.ToString()
        );

        // AsSIGN MANEUVERS

        // Player With Initialive

        parameters = new JSONObject();
        parameters.AddField("Id", 2);
        parameters.AddField("ManeuverCode", "5.F.S");
        GameController.SendCommand(
            GameCommandTypes.AssignManeuver,
            typeof(SubPhases.PlanningSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 3);
        parameters.AddField("ManeuverCode", "5.F.S");
        GameController.SendCommand(
            GameCommandTypes.AssignManeuver,
            typeof(SubPhases.PlanningSubPhase),
            parameters.ToString()
        );

        GameController.SendCommand(
            GameCommandTypes.PressNext,
            typeof(SubPhases.PlanningSubPhase)
        );

        // Player Without Initialive

        parameters = new JSONObject();
        parameters.AddField("Id", 1);
        parameters.AddField("ManeuverCode", "4.F.S");
        GameController.SendCommand(
            GameCommandTypes.AssignManeuver,
            typeof(SubPhases.PlanningSubPhase),
            parameters.ToString()
        );

        GameController.SendCommand(
            GameCommandTypes.PressNext,
            typeof(SubPhases.PlanningSubPhase)
        );

        // MOVEMENT

        parameters = new JSONObject();
        parameters.AddField("Id", 2);
        GameController.SendCommand(
            GameCommandTypes.ActiveShipMovement,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "Focus");
        GameController.SendCommand(
            GameCommandTypes.Decision,
            typeof(SubPhases.ActionDecisonSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 3);
        GameController.SendCommand(
            GameCommandTypes.ActiveShipMovement,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "Focus");
        GameController.SendCommand(
            GameCommandTypes.Decision,
            typeof(SubPhases.ActionDecisonSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 1);
        GameController.SendCommand(
            GameCommandTypes.ActiveShipMovement,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "Focus");
        GameController.SendCommand(
            GameCommandTypes.Decision,
            typeof(SubPhases.ActionDecisonSubPhase),
            parameters.ToString()
        );

        // COMBAT

        // 1 => 2

        parameters = new JSONObject();
        parameters.AddField("Id", 1);
        parameters.AddField("TargetId", 2);
        GameController.SendCommand(
            GameCommandTypes.DeclareAttack,
            typeof(SubPhases.CombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.AttackDiceRollCombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.DefenceDiceRollCombatSubPhase),
            parameters.ToString()
        );

        // 2 => 1

        parameters = new JSONObject();
        parameters.AddField("Id", 2);
        parameters.AddField("TargetId", 1);
        GameController.SendCommand(
            GameCommandTypes.DeclareAttack,
            typeof(SubPhases.CombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.AttackDiceRollCombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.DefenceDiceRollCombatSubPhase),
            parameters.ToString()
        );

        // 3 => 1

        parameters = new JSONObject();
        parameters.AddField("Id", 3);
        parameters.AddField("TargetId", 1);
        GameController.SendCommand(
            GameCommandTypes.DeclareAttack,
            typeof(SubPhases.CombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.AttackDiceRollCombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.DefenceDiceRollCombatSubPhase),
            parameters.ToString()
        );

        // SECOND ROUND

        // AsSIGN MANEUVERS

        // Player With Initialive

        parameters = new JSONObject();
        parameters.AddField("Id", 2);
        parameters.AddField("ManeuverCode", "2.F.S");
        GameController.SendCommand(
            GameCommandTypes.AssignManeuver,
            typeof(SubPhases.PlanningSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 3);
        parameters.AddField("ManeuverCode", "2.F.S");
        GameController.SendCommand(
            GameCommandTypes.AssignManeuver,
            typeof(SubPhases.PlanningSubPhase),
            parameters.ToString()
        );

        GameController.SendCommand(
            GameCommandTypes.PressNext,
            typeof(SubPhases.PlanningSubPhase)
        );

        // Player Without Initialive

        parameters = new JSONObject();
        parameters.AddField("Id", 1);
        parameters.AddField("ManeuverCode", "1.F.S");
        GameController.SendCommand(
            GameCommandTypes.AssignManeuver,
            typeof(SubPhases.PlanningSubPhase),
            parameters.ToString()
        );

        GameController.SendCommand(
            GameCommandTypes.PressNext,
            typeof(SubPhases.PlanningSubPhase)
        );

        // MOVEMENT

        parameters = new JSONObject();
        parameters.AddField("Id", 2);
        GameController.SendCommand(
            GameCommandTypes.ActiveShipMovement,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "Focus");
        GameController.SendCommand(
            GameCommandTypes.Decision,
            typeof(SubPhases.ActionDecisonSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 3);
        GameController.SendCommand(
            GameCommandTypes.ActiveShipMovement,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "Focus");
        GameController.SendCommand(
            GameCommandTypes.Decision,
            typeof(SubPhases.ActionDecisonSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Id", 1);
        GameController.SendCommand(
            GameCommandTypes.ActiveShipMovement,
            typeof(SubPhases.ActivationSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "Focus");
        GameController.SendCommand(
            GameCommandTypes.Decision,
            typeof(SubPhases.ActionDecisonSubPhase),
            parameters.ToString()
        );

        // COMBAT

        // 1 => 2

        parameters = new JSONObject();
        parameters.AddField("Id", 1);
        parameters.AddField("TargetId", 2);
        GameController.SendCommand(
            GameCommandTypes.DeclareAttack,
            typeof(SubPhases.CombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.AttackDiceRollCombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.DefenceDiceRollCombatSubPhase),
            parameters.ToString()
        );

        // 2 => 1

        parameters = new JSONObject();
        parameters.AddField("Id", 2);
        parameters.AddField("TargetId", 1);
        GameController.SendCommand(
            GameCommandTypes.DeclareAttack,
            typeof(SubPhases.CombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.AttackDiceRollCombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.DefenceDiceRollCombatSubPhase),
            parameters.ToString()
        );

        // 3 => 1

        parameters = new JSONObject();
        parameters.AddField("Id", 3);
        parameters.AddField("TargetId", 1);
        GameController.SendCommand(
            GameCommandTypes.DeclareAttack,
            typeof(SubPhases.CombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.AttackDiceRollCombatSubPhase),
            parameters.ToString()
        );

        parameters = new JSONObject();
        parameters.AddField("Name", "OK");
        parameters.AddField("Type", DiceModificationTimingType.Normal.ToString());
        GameController.SendCommand(
            GameCommandTypes.DiceModification,
            typeof(SubPhases.DefenceDiceRollCombatSubPhase),
            parameters.ToString()
        );
    }

}

