using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace GameCommands
{
    public class SyncDiceResultsCommand : GameCommand
    {
        public SyncDiceResultsCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            string diceToText = "";

            List<DieSide> correctSides = new List<DieSide>();
            JSONObject jsonHolder = (JSONObject)GetParameter("sides");
            foreach (var dieInfo in jsonHolder.list)
            {
                DieSide side = (DieSide)Enum.Parse(typeof(DieSide), dieInfo["side"].str);
                correctSides.Add(side);

                diceToText += side + " ";
            }

            DiceRoll.SyncDiceResults(correctSides);

            Console.Write($"Dice results are synchronized: {diceToText}");

            Phases.FinishSubPhase(typeof(DiceSyncSubphase));
        }
    }

}
