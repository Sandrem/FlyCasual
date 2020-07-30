﻿using ActionsList;
using GameCommands;
using GameModes;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class ReplayPlayer : GenericPlayer
    {
        public ReplayPlayer() : base()
        {
            PlayerType = PlayerType.Replay;
            Name = "Replay";
        }

        public override void ChangeManeuver(Action<string> doWithManeuverString, Action callback, Func<string, bool> filter = null)
        {
            DirectionsMenu.Show(doWithManeuverString, callback, filter);
        }
    }

}
