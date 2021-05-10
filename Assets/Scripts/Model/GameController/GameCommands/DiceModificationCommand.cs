using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class DiceModificationCommand : GameCommand
    {
        public DiceModificationCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            string diceModificationNameFixed = GetString("name");
            string diceModificationName = diceModificationNameFixed.Replace('_', '"');

            if (diceModificationName != "OK")
            {
                Console.Write($"Dice are modified by {diceModificationName}");
                Messages.ShowInfoToOpponent(diceModificationName, allowCopies: true);
            }
            else
            {
                Console.Write("Dice results are confirmed");
            }

            Combat.DiceModifications.UseDiceModification(diceModificationName);
        }
    }

}
