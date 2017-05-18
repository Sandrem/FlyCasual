using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class BlackSquadronPilot: TIEFighter
        {
            public BlackSquadronPilot(Players.PlayerNo playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Black Squadron Pilot";
                PilotSkill = 4;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }
        }
    }
}
