﻿using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class PressNextCommand : GameCommand
    {
        public PressNextCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            Console.Write("Button \"Next\" is used");
            UI.NextButtonEffect();
        }
    }

}
