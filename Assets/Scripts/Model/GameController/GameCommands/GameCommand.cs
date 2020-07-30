﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum GameCommandTypes
{
    DamageDecksSync,
    SquadsSync,
    Decision,
    ObstaclePlacement,
    ShipPlacement,
    AssignManeuver,
    PressNext,
    ActivateAndMove,
    DeclareAttack,
    DiceModification,
    SelectShip,
    SyncDiceResults,
    SyncDiceRerollSelected,
    ConfirmCrit,
    ConfirmDiceCheck,
    PressSkip,
    HotacSwerve,
    HotacFreeTargetLock,
    SyncPlayerWithInitiative,
    SystemActivation,
    CombatActivation,
    SelectObstacle,
    BombPlacement,
    MoveObstacle,
    SelectShipToAssignManeuver,
    CancelShipSelection
}

namespace GameCommands
{
    public abstract class GameCommand
    {
        public GameCommandTypes Type { get; private set; }
        public Type SubPhase { get; private set; }
        public string RawParameters { get; private set; }
        public JSONObject Parameters { get; private set; }
        protected virtual bool IsPreparationCommand { get; }

        public GameCommand(GameCommandTypes type, Type subPhase, string rawParameters)
        {
            Type = type;
            SubPhase = subPhase;
            RawParameters = rawParameters;
            Parameters = new JSONObject(rawParameters);
        }

        public object GetParameter(string key)
        {
            if (!Parameters.HasField(key)) Debug.Log("No field: " + key);
            if (Parameters[key].IsString) GetString(key);
            if (Parameters[key].IsNumber) GetFloat(key);
            return Parameters[key];
        }

        public string GetString(string key)
        {
            if (!Parameters.HasField(key)) Debug.Log("No field: " + key);
            return Parameters[key].str;
        }

        public float GetFloat(string key)
        {
            if (!Parameters.HasField(key)) Debug.Log("No field: " + key);
            return Parameters[key].f;
        }

        public override string ToString()
        {
            JSONObject json = new JSONObject();
            json.AddField("command", Type.ToString());
            json.AddField("subphase", (SubPhase != null) ? SubPhase.ToString() : null);
            json.AddField("parameters", new JSONObject(RawParameters));
            return json.ToString();
        }

        public virtual void TryExecute()
        {
            SubPhases.GenericSubPhase subphase = Phases.CurrentSubPhase;

            if (!IsPreparationCommand || ReplaysManager.Mode == ReplaysMode.Read)
            {
                if (subphase == null)
                {
                    //Debug.Log(Type + " command is skipped (subphase is null)");
                    return;
                }
                else if (subphase.GetType() != SubPhase)
                {
                    //Debug.Log(Type + " command is skipped: subphase is " + subphase + " instead of " + SubPhase);
                    return;
                }
                else if (!subphase.AllowedGameCommandTypes.Contains(Type) && Type != GameCommandTypes.ConfirmCrit)
                {
                    //Debug.Log(Type + " command is skipped: " + subphase + " doesn't support this type of commands");
                    return;
                }
                else if (!subphase.IsReadyForCommands)
                {
                    //Debug.Log(Type + " command is skipped: " + subphase + " is not ready for commands");
                    return;
                }
            }

            Console.Write("Command is executed: " + Type, LogTypes.GameCommands, true, "aqua");

            GameController.ConfirmCommand();
            Execute();
        }

        public abstract void Execute();
    }

}
