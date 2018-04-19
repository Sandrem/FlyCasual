using Mods.ModsList;

namespace Ship
{
    namespace XWing
    {
        public class TychoCelchuXWing : XWing
        {
            public TychoCelchuXWing() : base()
            {
                PilotName = "Tycho Celchu";
                PilotSkill = 8;
                Cost = 28;

                ImageUrl = "https://i.imgur.com/imayMBg.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.TychoCelchuAbility());

                RequiredMods.Add(typeof(MyOtherRideIsMod));
            }
        }
    }
}