using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Abilities;
using Mods.ModsList;

namespace Ship
{
    namespace YWing
    {
        public class NorraWexleyYWing : YWing
        {
            public NorraWexleyYWing() : base()
            {
                PilotName = "Norra Wexley";
                PilotSkill = 7;
                Cost = 24;

                ImageUrl = "https://i.imgur.com/5HBK61g.png";

                IsUnique = true;

                faction = Faction.Rebel;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new NorraWexleyPilotAbility());

                RequiredMods.Add(typeof(MyOtherRideIsMod));
            }
        }
    }
}