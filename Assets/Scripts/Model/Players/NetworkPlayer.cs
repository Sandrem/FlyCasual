using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class NetworkPlayer : GenericPlayer
    {

        public NetworkPlayer() : base()
        {
            Type = PlayerType.Network;
            Name = "Network";
        }

    }

}
