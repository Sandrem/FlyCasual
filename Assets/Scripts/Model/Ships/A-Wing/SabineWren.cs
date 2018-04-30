using Mods.ModsList;

namespace Ship
{
    namespace AWing
    {
        public class SabineWren : AWing
        {
            public SabineWren() : base()
            {
                RequiredMods.Add(typeof(PhoenixSquadronMod));

                PilotName = "Sabine Wren";
                PilotSkill = 5;
                Cost = 23;

                ImageUrl = "https://i.imgur.com/yRrheRR.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Green";

                PilotAbilities.Add(new Abilities.SabineWrenPilotAbility());
            }
        }
    }
}
