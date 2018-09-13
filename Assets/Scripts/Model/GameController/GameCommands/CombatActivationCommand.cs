using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class CombatActivationCommand : GameCommand
    {
        public CombatActivationCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            CombatSubPhase.DoCombatActivation(int.Parse(GetString("id")));
        }
    }

}
