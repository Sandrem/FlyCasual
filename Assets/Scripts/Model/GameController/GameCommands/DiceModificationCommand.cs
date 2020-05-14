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
            string diceModificationNameFixed = GetString("name");
            string diceModificationName = diceModificationNameFixed.Replace('_', '"');

            if (diceModificationName != "OK") Messages.ShowInfoToOpponent(diceModificationName, allowCopies:true);
            Combat.DiceModifications.UseDiceModification(diceModificationName);
        }
    }

}
