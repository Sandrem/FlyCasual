using Players;
using SquadBuilderNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class SquadsSyncCommand : GameCommand
    {
        public SquadsSyncCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void TryExecute()
        {
            GameInitializer.TryExecute(this);
        }

        public override void Execute()
        {
            PlayerNo playerNo = (PlayerNo)Enum.Parse(typeof(PlayerNo), GetString("player"));
            string playerType = GetString("type");

            Console.Write($"Squad for Player {Tools.PlayerToInt(playerNo)} ({playerType}) is ready");

            SquadList playerList = Global.SquadBuilder.SquadLists[playerNo];
            playerList.SavedConfiguration = (JSONObject)GetParameter("list");
            playerList.PlayerType = System.Type.GetType(playerType);
        }
    }

}
