using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class BlackSquadronPilot: TIEFighter
        {
            public BlackSquadronPilot(Players.GenericPlayer owner, int shipId, Vector3 position) : base(owner, shipId, position)
            {
                PilotName = "Black Squadron Pilot";
                PilotSkill = 4;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }
        }
    }
}
