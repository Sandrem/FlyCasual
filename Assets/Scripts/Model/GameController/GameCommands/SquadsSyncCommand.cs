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

        public override void Execute()
        {
            GameController.ConfirmCommand();
            SquadList playerList = SquadBuilder.SquadLists.First(n => n.PlayerNo == (PlayerNo)Enum.Parse(typeof(PlayerNo), GetString("player")));
            playerList.SavedConfiguration = (JSONObject)GetParameter("list");
            playerList.PlayerType = System.Type.GetType(GetString("type"));
        }
    }

}
