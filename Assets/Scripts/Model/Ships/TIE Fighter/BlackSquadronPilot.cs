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
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/b/b2/Black_Squadron_Pilot.jpg";
                PilotSkill = 4;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);

                InitializePilot();
            }
        }
    }
}
