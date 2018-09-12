using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class SyncDiceResultsCommand : GameCommand
    {
        public SyncDiceResultsCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            List<DieSide> correctSides = new List<DieSide>();
            JSONObject jsonHolder = (JSONObject)GetParameter("sides");
            Console.Write("Sides: " + jsonHolder.ToString());
            foreach (var dieInfo in jsonHolder.list)
            {
                DieSide side = (DieSide)Enum.Parse(typeof(DieSide), dieInfo["side"].str);
                correctSides.Add(side);
            }
            DiceRoll.SyncDiceResults(correctSides);
        }
    }

}
