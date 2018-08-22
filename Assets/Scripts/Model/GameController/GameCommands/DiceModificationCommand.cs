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
        public DiceModificationCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            string diceModificationName = GetString("name");
            if (diceModificationName == "OK")
            {
                Combat.ConfirmDiceResultsClient();
            }
            else
            {
                Combat.UseDiceModification(diceModificationName);
            }
        }
    }

}
