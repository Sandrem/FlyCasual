using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.Z95Headhunter
    {
        public class BanditSquadronPilot : Z95Headhunter
        {
            public BanditSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bandit Squadron Pilot",
                    2,
                    12
                );
            }
        }
    }
}
