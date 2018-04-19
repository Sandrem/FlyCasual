using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;

namespace Ship
{
    namespace BWing
    {
        public class BraylenStrammBWing : BWing
        {
            public BraylenStrammBWing() : base()
            {
                RequiredMods.Add(typeof(MyOtherRideIsMod));

                PilotName = "Braylen Stramm";
                PilotSkill = 3;
                Cost = 24;

                ImageUrl = "https://i.imgur.com/V6m7JN9.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.BraylenStrammPilotAbility());
            }
        }
    }
}
