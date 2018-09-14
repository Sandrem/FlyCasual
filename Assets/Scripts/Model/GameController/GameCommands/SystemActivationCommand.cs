using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class SystemActivationCommand : GameCommand
    {
        public SystemActivationCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            SystemsSubPhase.DoSystemActivation(int.Parse(GetString("id")));
        }
    }

}
