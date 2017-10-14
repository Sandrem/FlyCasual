using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class NetworkOpponentPlayer : GenericPlayer
    {

        public NetworkOpponentPlayer() : base()
        {
            Type = PlayerType.Network;
            Name = "Network";
        }

        public override void TakeDecision()
        {
            Messages.ShowInfo("Network Player is asked to take decision");
        }

    }

}
