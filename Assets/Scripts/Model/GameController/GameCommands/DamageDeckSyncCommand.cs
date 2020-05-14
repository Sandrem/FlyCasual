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
        protected override bool IsPreparationCommand => true;

        public DamageDeckSyncCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void TryExecute()
        {
            GameInitializer.TryExecute(this);
        }

        public override void Execute()
        {
            DamageDecks.GetDamageDeck((PlayerNo)Enum.Parse(typeof(PlayerNo), GetString("player"))).ShuffleDeck(int.Parse(GetString("seed")));
        }
    }

}
