using Players;
using SquadBuilderNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class SyncPlayerWithInitiativeCommand : GameCommand
    {
        public SyncPlayerWithInitiativeCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void TryExecute()
        {
            GameInitializer.TryExecute(this);
        }

        public override void Execute()
        {
            Phases.PlayerWithInitiative = (PlayerNo)Enum.Parse(typeof(PlayerNo), GetString("player"));
            Triggers.FinishTrigger();
        }
    }

}
