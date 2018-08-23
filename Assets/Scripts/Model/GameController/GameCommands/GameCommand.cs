using System;
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
    SyncPlayerWithInitiative
}

namespace GameCommands
{
    public abstract class GameCommand
    {
        public GameCommandTypes Type { get; private set; }
        public Type SubPhase { get; private set; }
        public string RawParameters { get; private set; }
        public JSONObject Parameters { get; private set; }

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

        public void TryExecute()
        {
            SubPhases.GenericSubPhase subphase = Phases.CurrentSubPhase;
            if (subphase != null && subphase.AllowedGameCommandTypes.Contains(Type) && subphase.GetType() == SubPhase && subphase.IsReadyForCommands)
            {
                GameController.ConfirmCommand();
                Console.Write("Command is executed: " + Type, LogTypes.GameCommands, true, "aqua");
                Execute();
                GameController.CheckExistingCommands();
            }
            else
            {
                if (SubPhase != null) Console.Write("Command is skipped: " + Type, LogTypes.GameCommands, false, "aqua");
            }
        }

        public abstract void Execute();
    }

}
