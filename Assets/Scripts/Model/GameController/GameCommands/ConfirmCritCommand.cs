using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class ConfirmCritCommand : GameCommand
    {
        public ConfirmCritCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {
            
        }

        public override void Execute()
        {
            InformCrit.ConfirmCrit();
        }
    }

}
