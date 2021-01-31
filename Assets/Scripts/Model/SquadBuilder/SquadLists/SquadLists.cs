using Players;
using System;
using System.Collections.Generic;

namespace SquadBuilderNS
{
    public class SquadLists
    {
        //TODO: Hide
        public Dictionary<PlayerNo, SquadList> Squads = new Dictionary<PlayerNo, SquadList>()
        {
            { PlayerNo.Player1, new SquadList(PlayerNo.Player1) },
            { PlayerNo.Player2, new SquadList(PlayerNo.Player2) }
        };

        public SquadList this[PlayerNo playerNo] => Squads[playerNo];

        public void SwitchPlayers()
        {
            SquadList player1SquadList = this[PlayerNo.Player1];
            SquadList player2SquadList = this[PlayerNo.Player2];

            player1SquadList.PlayerNo = PlayerNo.Player2;
            player2SquadList.PlayerNo = PlayerNo.Player1;
        }

        public void ReGenerateSquads()
        {
            Squads[PlayerNo.Player1].ReGenerateSquad();
            Squads[PlayerNo.Player2].ReGenerateSquad();
        }
    }
}
