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
        public class DutchVanderElite : DutchVander
        {
            public DutchVanderElite() : base()
            {
                PilotName = "\"Dutch\" Vander (Elite)";

                ImageUrl = "https://i.imgur.com/aakKh9D.png";

                UpgradeBar.AddSlot(Upgrade.UpgradeType.Elite);

                RequiredMods.Add(typeof(EliteYWingPilotsMod));
            }
        }
    }
}