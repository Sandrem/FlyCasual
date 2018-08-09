using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum GameCommandTypes
{
    SquadsSync,
    Decision,
    ObstaclePlacement,
    ShipPlacement
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

        public object GetParameter(string key)
        {
            if (Parameters[key].IsString) return Parameters[key].str;
            if (Parameters[key].IsNumber) return Parameters[key].f;
            return null;
        }
    }

}
