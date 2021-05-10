using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class ConfirmDiceCheckCommand : GameCommand
    {
        public ConfirmDiceCheckCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            string diceResults = "";
            foreach (DieSide dieSide in DiceRoll.CurrentDiceRoll.ResultsArray)
            {
                switch (dieSide)
                {
                    case DieSide.Blank:
                        diceResults += "blank";
                        break;
                    case DieSide.Focus:
                        diceResults += "eye ";
                        break;
                    case DieSide.Success:
                        diceResults += (DiceRoll.CurrentDiceRoll.Type == DiceKind.Attack) ? "hit " : "evade ";
                        break;
                    case DieSide.Crit:
                        diceResults += "crit ";
                        break;
                    default:
                        break;
                }
            }


            Console.Write($"Dice results are confirmed: {diceResults}");
            (Phases.CurrentSubPhase as DiceRollCheckSubPhase).Confirm();
        }
    }

}
