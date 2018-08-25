using Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class DamageDeckSyncCommand : GameCommand
    {
        public DamageDeckSyncCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            GameController.ConfirmCommand();
            DamageDecks.GetDamageDeck((PlayerNo)Enum.Parse(typeof(PlayerNo), GetString("player"))).ShuffleDeck(int.Parse(GetString("seed")));
        }
    }

}
