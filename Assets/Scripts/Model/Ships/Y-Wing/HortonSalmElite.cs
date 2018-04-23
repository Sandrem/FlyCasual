using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Mods.ModsList;

namespace Ship
{
    namespace YWing
    {
        public class HortonSalmElite : HortonSalm
        {
            public HortonSalmElite() : base()
            {
                PilotName = "Horton Salm (Elite)";

                ImageUrl = "https://i.imgur.com/iOArz8A.png";

                UpgradeBar.AddSlot(Upgrade.UpgradeType.Elite);

                RequiredMods.Add(typeof(EliteYWingPilotsMod));
            }
        }
    }
}