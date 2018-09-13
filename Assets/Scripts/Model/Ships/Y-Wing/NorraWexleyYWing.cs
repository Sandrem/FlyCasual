using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Abilities;
using Mods.ModsList;
using Abilities.SecondEdition;
using RuleSets;

/*
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
} */

namespace Ship
{
    namespace YWing
    {
        public class NorraWexleyYWing : YWing, ISecondEditionPilot
        {
            public NorraWexleyYWing() : base()
            {
                PilotName = "Norra Wexley";
                PilotSkill = 5;
                Cost = 43;

                IsUnique = true;

                faction = Faction.Rebel;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new NorraWexleyPilotAbilitySE());

                SEImageNumber = 13;
            }

            public void AdaptPilotToSecondEdition()
            {
                // empty unneeded bam
            }
        }
    }
}
