using Mods.ModsList;

namespace Ship
{
    namespace AWing
    {
        public class HeraSyndulla : AWing
        {
            public HeraSyndulla() : base()
            {
                FromMod = typeof(PhoenixSquadronMod);

                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 25;

                ImageUrl = "https://i.imgur.com/zKSEmAL.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.HeraSyndullaAbility());
            }
        }
    }
}
