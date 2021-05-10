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

        public DamageDeckSyncCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void TryExecute()
        {
            GameInitializer.TryExecute(this);
        }

        public override void Execute()
        {
            int seed = int.Parse(GetString("seed"));
            PlayerNo playerNo = (PlayerNo)Enum.Parse(typeof(PlayerNo), GetString("player"));

            Console.Write($"Damage deck of Player {Tools.PlayerToInt(playerNo)} is shuffled (Seed: {seed})");

            DamageDecks.GetDamageDeck(playerNo).ShuffleDeck(seed);
        }
    }

}
