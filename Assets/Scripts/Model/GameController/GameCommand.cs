using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum GameCommandTypes
{
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
    ConfirmDiceCheck
}

namespace GameCommands
{
    public class GameCommand
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

        public GameCommand(string textjson)
        {
            JSONObject json = new JSONObject(textjson);
            Type = (GameCommandTypes) Enum.Parse(typeof(GameCommandTypes), json["command"].str);
            SubPhase = System.Type.GetType(json["subphase"].str);
            RawParameters = json["parameters"].str.Replace("###", "\n");
            Parameters = new JSONObject(RawParameters);
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
    }

}
