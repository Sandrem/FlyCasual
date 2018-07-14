using Mods.ModsList;

namespace Ship
{
    namespace AWing
    {
        public class HeraSyndulla : AWing
        {
            public HeraSyndulla() : base()
            {
                RequiredMods.Add(typeof(PhoenixSquadronMod));

                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 25;

                ImageUrl = "https://i.imgur.com/4zfSMcc.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.HeraSyndullaAbility());
            }
        }
    }
}
