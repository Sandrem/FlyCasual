using Mods.ModsList;
using Ship;
using System;
using System.Collections.Generic;
 
namespace Ship
{
    namespace XWing
    {
        public class IbtisamXWing : XWing
        {
            public IbtisamXWing() : base()
            {
                PilotName = "Ibtisam";
                PilotSkill = 6;
                Cost = 26;

                ImageUrl = "https://i.imgur.com/UteVMCP.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
 
                PilotAbilities.Add(new Abilities.IbtisamAbiliity());

                RequiredMods.Add(typeof(MyOtherRideIsMod));
            }
        }
    }
}